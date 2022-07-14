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
            services.AddIAM(new IAMSettings());

            var serviceProvider = services.BuildServiceProvider();

            return serviceProvider;
        }
    }
}
