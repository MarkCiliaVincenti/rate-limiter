using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using RateLimiter.Core;
using RateLimiter.Core.Models;
using RateLimiter.Core.Store;
using System;
using System.Collections.Generic;

namespace RateLimit.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            IHost webHost = CreateHostBuilder(args).Build();

            // Seed initial test data
            Seed(webHost.Services);            

            // Start the application
            webHost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                                .UseDefaultServiceProvider(options => options.ValidateScopes = false);
                });

        private static void Seed(IServiceProvider services)
        {
            var store = (IRateLimitStore<RateLimitPolicy>)services.GetService(typeof(IRateLimitStore<RateLimitPolicy>));
            store.SaveAsync(RateLimiter.Core.Common.Templates.POLICY_ID_FORMAT("/test", "::1"), new RateLimitPolicy
            {
                Rules = new List<RateLimitRule> {
                    new RateLimitRule { Capacity = 5, WindowSize = new TimeInterval(TimeUnit.Minute, 1) }
                }

            }).Wait();
        }
    }
}
