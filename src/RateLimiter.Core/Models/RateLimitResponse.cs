namespace RateLimiter.Core.Models
{
    public class RateLimitResponse
    {
        /// <summary>
        /// Indicates if incoming request falls within configured rate limit
        /// </summary>
        public bool IsWithinRateLimit { get; private set; }

        /// <summary>
        /// Total number of seconds to wait before another request can be accepted
        /// </summary>
        public long WaitingPeriod { get; private set; }

        public static RateLimitResponse Accepted()
        {
            return new RateLimitResponse
            {
                IsWithinRateLimit = true
            };
        }

        public static RateLimitResponse Denied(long waitingPeriod)
        {
            return new RateLimitResponse
            {
                IsWithinRateLimit = false,
                WaitingPeriod = waitingPeriod
            };
        }                 
    }
}
