using AsyncKeyedLock;
using Moq;
using NUnit.Framework;
using RateLimiter.Core.Extensions;
using RateLimiter.Core.Models;
using RateLimiter.Core.Store;
using RateLimiter.Core.Strategy;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RateLimit.Core.Tests.Strategy
{
    [TestFixture]
    public class SlidingWindowStrategyTests
    {
        private Mock<IRateLimitStore<RateLimitRequestCounter>> _store;
        private Mock<AsyncKeyedLocker<string>> _locker;
        private SlidingWindowStrategy _strategy;

        private const string TestEndpoint = "test";
        private const string TestIdentifier = "test-ip-address";
        private RateLimitRule TestRule;

        [SetUp]
        public void Init()
        {
            _store = new Mock<IRateLimitStore<RateLimitRequestCounter>>();
            _locker = new Mock<AsyncKeyedLocker<string>>();
            _strategy = new SlidingWindowStrategy(_store.Object, _locker.Object);

            TestRule = new RateLimitRule
            {
                WindowSize = new TimeInterval(TimeUnit.Minute, 1),
                Capacity = 5
            };
        }

        [Test]
        public async Task ApplyRateLimitAsync_should_accept_and_record_request_if_no_counter_persists()
        {
            _store.Setup(x => x.GetAsync(It.IsAny<string>(), default)).ReturnsAsync((RateLimitRequestCounter)null);

            var response = await _strategy.ApplyRateLimitAsync(TestEndpoint, TestIdentifier, TestRule);

            Assert.True(response.IsWithinRateLimit);
            _store.Verify(x => x.SaveAsync(It.Is<string>(k => k.Contains(TestEndpoint) && k.Contains(TestIdentifier)), 
                It.IsAny<RateLimitRequestCounter>(), It.IsAny<TimeSpan?>(), default));
        }

        [Test]
        public async Task ApplyRateLimitAsync_should_accept_and_record_request_if_no_recent_record_persists()
        {
            _store.Setup(x => x.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(
                new RateLimitRequestCounter 
                { 
                    Records = new List<RequestCount> { new RequestCount { Timestamp = DateTime.Now.AddDays(-100).GetEpochTime() } }
                });

            var response = await _strategy.ApplyRateLimitAsync(TestEndpoint, TestIdentifier, TestRule);

            Assert.True(response.IsWithinRateLimit);
            _store.Verify(x => x.SaveAsync(It.Is<string>(k => k.Contains(TestEndpoint) && k.Contains(TestIdentifier)),
                It.IsAny<RateLimitRequestCounter>(), It.IsAny<TimeSpan?>(), default));
        }

        [Test]
        public async Task ApplyRateLimitAsync_should_accept_request_if_total_request_count_is_within_capacity()
        {
            _store.Setup(x => x.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(
                new RateLimitRequestCounter 
                {
                    Records = new List<RequestCount> { new RequestCount { Timestamp = DateTime.Now.GetEpochTime(), Count = TestRule.Capacity - 1 } }
                });            

            var response = await _strategy.ApplyRateLimitAsync(TestEndpoint, TestIdentifier, TestRule);

            Assert.True(response.IsWithinRateLimit);
        }

        [Test]
        public async Task ApplyRateLimitAsync_should_deny_request_if_no_capacity()
        {
            _store.Setup(x => x.GetAsync(It.IsAny<string>(), default)).ReturnsAsync(
              new RateLimitRequestCounter
              {
                  Records = new List<RequestCount> { new RequestCount { Timestamp = DateTime.Now.GetEpochTime(), Count = TestRule.Capacity } }
              });

            var response = await _strategy.ApplyRateLimitAsync(TestEndpoint, TestIdentifier, TestRule);

            Assert.False(response.IsWithinRateLimit);
        }
    }
}
