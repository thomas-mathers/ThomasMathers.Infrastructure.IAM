using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Common.IAM.Extensions;
using ThomasMathers.Common.IAM.Settings;
using System;

namespace ThomasMathers.Common.IAM.Tests.Helpers
{
    internal static class ServiceProviderBuilder
    {
        public static IServiceProvider Build()
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddIAM(new IAMSettings
            {
                Jwt = new JwtTokenSettings
                {
                    Key = "ABfTVaKf/VAZEY#U4wua1h,*tlfGG`",
                    Issuer = "localhost",
                    Audience = "localhost",
                    LifespanInDays = 1
                }
            });

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
