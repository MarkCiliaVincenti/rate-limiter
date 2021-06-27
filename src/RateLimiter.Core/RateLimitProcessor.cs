using RateLimiter.Core.Models;
using RateLimiter.Core.Store;
using RateLimiter.Core.Strategy;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Core
{
    public class RateLimitProcessor : IRateLimitProcessor
    {
        private IRateLimitStrategy _strategey;
        private IRateLimitStore<RateLimitPolicy> _policyStore;

        public RateLimitProcessor(IRateLimitStrategy strategy, IRateLimitStore<RateLimitPolicy> policyStore)
        {
            _strategey = strategy;
            _policyStore = policyStore;
        }

        public async Task<RateLimitResponse> ProcessAsync(string endpoint, string identifier, CancellationToken cancellationToken = default)
        {
            var policy = await _policyStore.GetAsync(Common.Templates.POLICY_ID_FORMAT(endpoint, identifier));
            if (policy == null)
            {
                // No rate limit applied
                return RateLimitResponse.Accepted();
            }
                                    
            foreach (var rule in policy.Rules)
            {
                var response = await _strategey.ApplyRateLimitAsync(endpoint, identifier, rule, cancellationToken);
                if(!response.IsWithinRateLimit)
                {
                    return response;
                }
            }
            
            return RateLimitResponse.Accepted();
        }             
    }
}
