using System.Text.Json;

using AutoFixture;

using ThomasMathers.Infrastructure.IAM.Builders;
using ThomasMathers.Infrastructure.IAM.Settings;
using ThomasMathers.Infrastructure.IAM.Tests.Helpers;

using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests.Builders;

public class IamSettingsBuilderTests
{
    private readonly Fixture _fixture;

    public IamSettingsBuilderTests() => _fixture = new Fixture();

    [Fact]
    public void Build_ReturnsCorrectValue()
    {
        var iamSettings = _fixture.Create<IamSettings>();
        var json = JsonSerializer.Serialize(new
        {
            IamSettings = iamSettings
        });
        var configuration = ConfigurationBuilder.Build(json);

        // Act
        var actual = IamSettingsBuilder.Build(configuration.GetSection("IamSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(iamSettings.ConnectionString, actual.ConnectionString);
        Assert.Equal(iamSettings.MigrationsAssembly, actual.MigrationsAssembly);
        Assert.Equal(iamSettings.JwtTokenSettings, actual.JwtTokenSettings);
        Assert.Equal(iamSettings.PasswordSettings, actual.PasswordSettings);
        Assert.Equal(iamSettings.UserSettings, actual.UserSettings);
    }
}