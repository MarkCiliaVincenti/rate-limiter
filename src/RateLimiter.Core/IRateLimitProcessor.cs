using RateLimiter.Core.Models;
using System.Threading;
using System.Threading.Tasks;

namespace RateLimiter.Core
{
    public interface IRateLimitProcessor
    {
        Task<RateLimitResponse> ProcessAsync(string endpoint, string identifier, CancellationToken cancellationToken = default);
    }
}
