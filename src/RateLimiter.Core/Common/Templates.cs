namespace RateLimiter.Core.Common
{
    public class Templates
    {
        public static string POLICY_ID_FORMAT(string endpoint, string identifier) => string.Format("policy-{0}-{1}", endpoint, identifier);
        public static string COUNTER_ID_FORMAT(string endpoint, string identifier) => string.Format("counter-{0}-{1}", endpoint, identifier);
    }
}
