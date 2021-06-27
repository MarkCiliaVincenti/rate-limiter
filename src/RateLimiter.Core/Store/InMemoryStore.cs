using Microsoft.Extensions.Caching.Memory;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Core.Store
{
    /// <inheritdoc cref="IRateLimitStore{T}"/>
    public class InMemoryStore<T> : IRateLimitStore<T>
    {
        private readonly IMemoryCache _local;

        public InMemoryStore(IMemoryCache cache)
        {
            _local = cache;
        }

        public Task<T> GetAsync(string id, CancellationToken cancellationToken = default)
        {
            if (_local.TryGetValue<T>(id, out T value))
            {
                return Task.FromResult(value);
            }

            return Task.FromResult(default(T));
        }        

        public Task SaveAsync(string id, T item, TimeSpan? expiry = null, CancellationToken cancellationToken = default)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                return Task.FromCanceled(cancellationToken);
            }
            
            var options = new MemoryCacheEntryOptions
            {
                SlidingExpiration = expiry
            };

            _local.Set(id, item, options);

            return Task.CompletedTask;
        }        
    }
}
