using System.Collections.Generic;

namespace RateLimiter.Core.Models
{
    /// <summary>
    /// Configuration for applying rate limit
    /// </summary>
    public class RateLimitPolicy
    {
        /// <summary>
        /// Collection of config rules to apply
        /// </summary>
        public List<RateLimitRule> Rules { get; set; }
    }

    public class RateLimitRule
    {
        /// <summary>
        /// Time interval for counting incoming requests
        /// </summary>
        public TimeInterval WindowSize { get; set; }

        /// <summary>
        /// Allowed request count within <see cref="WindowSize"/>
        /// </summary>
        public int Capacity { get; set; }
    }
}