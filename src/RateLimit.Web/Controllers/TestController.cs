using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RateLimiter.Core;

namespace RateLimit.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {
        private readonly ILogger<TestController> _logger;

        public TestController(ILogger<TestController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        [ServiceFilter(typeof(RateLimitFilter))]
        public string Get()
        {
            return "Hello World!";
        }
    }
}
