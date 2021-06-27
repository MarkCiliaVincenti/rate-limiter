using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Net;

namespace RateLimiter.Core
{
    /// <summary>
    /// Filters http requests to apply rate limiting policy based on configuration
    /// </summary>
    public class RateLimitFilter : IActionFilter
    {
        private readonly IRateLimitProcessor _processor;
        private readonly ILogger _logger;

        public RateLimitFilter(IRateLimitProcessor service, ILogger<RateLimitFilter> logger)
        {
            _processor = service;
            _logger = logger;
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Nothing to do;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var cancellationToken = context.HttpContext.RequestAborted;
            var remoteIpAddress = context.HttpContext.Connection.RemoteIpAddress.ToString();
            var ratelimitResponse = _processor.ProcessAsync(context.HttpContext.Request.Path, remoteIpAddress, cancellationToken).GetAwaiter().GetResult();

            if (!ratelimitResponse.IsWithinRateLimit)
            {
                _logger.LogInformation($"Rate limit exceeded for endpoint '{context.HttpContext.Request.Path}', Requester IP: {remoteIpAddress}.");
                context.Result = new ObjectResult($"Rate limit exceeded. Try again in #{ratelimitResponse.WaitingPeriod} seconds.")
                {
                    StatusCode = (int?)HttpStatusCode.TooManyRequests
                };
            }
        }
    }
}
