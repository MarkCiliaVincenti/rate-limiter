using Moq;
using NUnit.Framework;
using RateLimiter.Core;
using RateLimiter.Core.Models;
using RateLimiter.Core.Store;
using RateLimiter.Core.Strategy;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimit.Core.Tests
{
    [TestFixture]
    public class RateLimitProcessorTests
    {
        private Mock<IRateLimitStore<RateLimitPolicy>> _store;
        private Mock<IRateLimitStrategy> _strategy;
        private RateLimitProcessor _service;

        private const string TestEndpoint = "test";
        private const string TestIdentifier = "test-ip-address";

        [SetUp]
        public void Init()
        {
            _store = new Mock<IRateLimitStore<RateLimitPolicy>>();
            _strategy = new Mock<IRateLimitStrategy>();
            _service = new RateLimitProcessor(_strategy.Object, _store.Object);
        }

        [Test]
        public async Task ProcessAsync_should_allow_request_if_no_rate_limit_policy()
        {
            _store.Setup(x => x.GetAsync(It.IsAny<string>(), default)).ReturnsAsync((RateLimitPolicy)null);

            var response = await _service.ProcessAsync(TestEndpoint, TestIdentifier);

            Assert.True(response.IsWithinRateLimit);
        }

        [Test]
        public async Task ProcessAsync_should_apply_all_policy_rules()
        {
            _strategy.Setup(x => x.ApplyRateLimitAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<RateLimitRule>(), default)).ReturnsAsync(RateLimitResponse.Accepted());
            _store.Setup(x => x.GetAsync(It.IsAny<string>(), default))
                .ReturnsAsync(new RateLimitPolicy
                {
                    Rules = new List<RateLimitRule>
                    {
                       new RateLimitRule { WindowSize = new TimeInterval(TimeUnit.Hour, 24), Capacity = 1000 },
                       new RateLimitRule { WindowSize = new TimeInterval(TimeUnit.Minute, 60), Capacity = 100 }
                    }
                });

            var response = await _service.ProcessAsync(TestEndpoint, TestIdentifier);

            _strategy.Verify(x => x.ApplyRateLimitAsync(It.IsAny<string>(), It.IsAny<string>(), It.Is<RateLimitRule>(r => r.WindowSize.Unit == TimeUnit.Hour), default));
            _strategy.Verify(x => x.ApplyRateLimitAsync(It.IsAny<string>(), It.IsAny<string>(), It.Is<RateLimitRule>(r => r.WindowSize.Unit == TimeUnit.Minute), default));
        }
    }
}
