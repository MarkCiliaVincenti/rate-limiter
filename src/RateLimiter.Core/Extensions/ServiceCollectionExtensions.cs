using AsyncKeyedLock;
using Microsoft.Extensions.DependencyInjection;
using RateLimiter.Core.Models;
using RateLimiter.Core.Store;
using RateLimiter.Core.Strategy;

namespace RateLimiter.Core.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRateLimit(this IServiceCollection services)
        {
            services.AddMemoryCache();
            services.AddSingleton(typeof(IRateLimitStrategy), typeof(SlidingWindowStrategy));
            services.AddSingleton(new AsyncKeyedLocker<string>(o =>
            {
                o.PoolSize = 20;
                o.PoolInitialFill = 1;
            }));
            services.AddSingleton(typeof(IRateLimitStore<RateLimitPolicy>), typeof(InMemoryStore<RateLimitPolicy>));
            services.AddSingleton(typeof(IRateLimitStore<RateLimitRequestCounter>), typeof(InMemoryStore<RateLimitRequestCounter>));
            services.AddSingleton(typeof(IRateLimitProcessor), typeof(RateLimitProcessor));
            services.AddSingleton<RateLimitFilter>();

            return services;
        }
    }
}
