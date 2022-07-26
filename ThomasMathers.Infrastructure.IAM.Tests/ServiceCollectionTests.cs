using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Infrastructure.IAM.Extensions;
using ThomasMathers.Infrastructure.IAM.Services;
using ThomasMathers.Infrastructure.IAM.Settings;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class ServiceCollectionTests
{
    private readonly IServiceCollection _services;

    public ServiceCollectionTests()
    {
        _services = new ServiceCollection();
    }

    [Fact]
    public void AddIAM_RegistersRequiredServices()
    {
        // Arrange
        var settings = new IamSettings();

        // Act
        _services.AddIam(settings);
        var serviceProvider = _services.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetRequiredService<IUserService>());
        Assert.NotNull(serviceProvider.GetRequiredService<IAuthService>());
        Assert.NotNull(serviceProvider.GetRequiredService<IAccessTokenGenerator>());
        Assert.NotNull(serviceProvider.GetRequiredService<IamSettings>());
        Assert.NotNull(serviceProvider.GetRequiredService<UserSettings>());
        Assert.NotNull(serviceProvider.GetRequiredService<PasswordSettings>());
        Assert.NotNull(serviceProvider.GetRequiredService<JwtTokenSettings>());
    }
}