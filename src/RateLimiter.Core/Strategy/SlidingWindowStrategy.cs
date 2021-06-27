using RateLimiter.Core.Extensions;
using RateLimiter.Core.Lock;
using RateLimiter.Core.Models;
using RateLimiter.Core.Store;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Core.Strategy
{
    /// <inheritdoc cref="IRateLimitStrategy"/>
    public class SlidingWindowStrategy : IRateLimitStrategy
    {
        private readonly IRateLimitStore<RateLimitRequestCounter> _store;
        private readonly AsyncLocker _locker;

        public SlidingWindowStrategy(IRateLimitStore<RateLimitRequestCounter> store) 
        {
            _store = store;
            _locker = new AsyncLocker();
        }

        public async Task<RateLimitResponse> ApplyRateLimitAsync(string endpoint, string identifier, RateLimitRule rule, CancellationToken cancellationToken = default)
        {
            var counterId = Common.Templates.COUNTER_ID_FORMAT(endpoint, identifier);

            using (await _locker.GetLockAsync(counterId))
            {
                var counter = await _store.GetAsync(counterId, cancellationToken);
                if (counter == null)
                {
                    counter = new RateLimitRequestCounter { Records = new List<RequestCount>() };
                }

                // Calculate current window boundary
                var windowSize = rule.WindowSize.GetTotalSeconds();
                var currentTimestamp = DateTime.Now.GetEpochTime();
                var windowStartTimestamp = currentTimestamp - windowSize;

                // Apply the rate limit for current window
                var ratelimitResponse = CheckRateLimit(windowStartTimestamp, currentTimestamp, rule.Capacity, counter.Records);
                if (ratelimitResponse.IsWithinRateLimit)
                {
                    // update request count for current request
                    var record = counter.Records.FirstOrDefault(x => x.Timestamp == currentTimestamp);
                    if (record == null)
                    {
                        record = new RequestCount { Timestamp = currentTimestamp };
                        counter.Records.Add(record);
                    }

                    record.Count += 1;
                }

                // Remove any old records outside current window
                counter.Records.RemoveAll(x => x.Timestamp <= windowStartTimestamp);

                // Update counter record for current request
                await _store.SaveAsync(counterId, counter, rule.WindowSize.ToTimespan(), cancellationToken);

                return ratelimitResponse;
            }            
        }
                        
        private RateLimitResponse CheckRateLimit(long windowStarts, long windowEnds, int capacity, List<RequestCount> records)
        {
            if (!records.Any())
            {
                // No recent incoming request
                return RateLimitResponse.Accepted();
            }

            // Get all counter records within current sliding window
            var requestsWithinCurrentWindow = records.Where(x => x.Timestamp >= windowStarts && x.Timestamp <= windowEnds);
            if (!requestsWithinCurrentWindow.Any())
            {
                // No incoming request in current window
                return RateLimitResponse.Accepted();
            }

            // Check remaining capacity within current window 
            var firstRequestTimestamp = requestsWithinCurrentWindow.First().Timestamp;
            var totalRequestsWithinCurrentWindow = requestsWithinCurrentWindow.Sum(x => x.Count);
            if(totalRequestsWithinCurrentWindow >= capacity)
            {
                return RateLimitResponse.Denied((firstRequestTimestamp - windowStarts) + 1);
            }

            return RateLimitResponse.Accepted();
        }        
    }
}
