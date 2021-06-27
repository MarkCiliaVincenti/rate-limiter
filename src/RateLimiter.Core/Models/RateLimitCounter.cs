using System.Collections.Generic;

namespace RateLimiter.Core.Models
{
    /// <summary>
    /// Record of incoming request counts per timestamp
    /// </summary>
    public class RateLimitRequestCounter
    {
        public List<RequestCount> Records { get; set; }
    }

    public class RequestCount
    {
        /// <summary>
        /// Rounded timestamp (upto second) for request count
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Total number of accepted requests at specific <see cref="Timestamp"/>
        /// </summary>
        public int Count { get; set; }
    }
}
