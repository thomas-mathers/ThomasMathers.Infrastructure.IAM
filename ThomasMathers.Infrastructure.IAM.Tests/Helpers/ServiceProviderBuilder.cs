using System;
using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Infrastructure.IAM.Extensions;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Tests.Helpers;

internal static class ServiceProviderBuilder
{
    public static IServiceProvider Build()
    {
        var services = new ServiceCollection();

        services.AddIAM(new IAMSettings());

        var serviceProvider = services.BuildServiceProvider();

        return serviceProvider;
    }
}