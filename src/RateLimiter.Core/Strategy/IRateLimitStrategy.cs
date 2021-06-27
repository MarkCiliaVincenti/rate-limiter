using RateLimiter.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Core.Strategy
{
    /// <summary>
    /// Defines strategy for applying rate limit
    /// </summary>
    /// <param name="endpoint">Resource endpoint to apply rate limit against</param>
    /// <param name="identifier">Identity of the requester e.g. IP address</param>
    /// <param name="rule">Rate limiting configuration</param>
    /// <Returns></Returns>
    public interface IRateLimitStrategy
    {
        /// <summary>
        /// Applies rate limit for specified endpoint based on rate limiting rule;
        /// also responsible for updating any data/state after accepting a request 
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="identifier"></param>
        /// <param name="rule"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<RateLimitResponse> ApplyRateLimitAsync(string endpoint, string identifier, RateLimitRule rule, CancellationToken cancellationToken = default);
    }
}
