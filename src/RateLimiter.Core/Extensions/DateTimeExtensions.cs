using RateLimiter.Core.Models;
using System;

namespace RateLimiter.Core.Extensions
{
    public static class DateTimeExtensions
    {
        private const long SecondsPerMinute = 60;
        private const long SecondsPerHour = 3600;

        public static long GetEpochTime(this DateTime time)
        {
            TimeSpan timespan = time - new DateTime(1970, 1, 1);
            return (long)timespan.TotalSeconds;
        }

        public static long GetTotalSeconds(this TimeInterval interval)
        {
            return interval.Unit switch
            {
                TimeUnit.Hour => interval.Value * SecondsPerHour,
                TimeUnit.Minute => interval.Value * SecondsPerMinute,
                _ => interval.Value,
            };
        }

        public static TimeSpan ToTimespan(this TimeInterval interval)
        {
            return TimeSpan.FromSeconds(interval.GetTotalSeconds());
        }
    }
}
