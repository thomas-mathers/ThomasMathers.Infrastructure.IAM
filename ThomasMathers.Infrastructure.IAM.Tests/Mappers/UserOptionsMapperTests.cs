using AutoFixture;

using ThomasMathers.Infrastructure.IAM.Mappers;
using ThomasMathers.Infrastructure.IAM.Settings;

using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests.Mappers;

public class UserOptionsMapperTests
{
    private readonly Fixture _fixture;

    public UserOptionsMapperTests() => _fixture = new Fixture();

    [Fact]
    public void Map_MapsCorrectly()
    {
        // Arrange
        var userSettings = _fixture.Create<UserSettings>();

        // Act
        var actual = UserOptionsMapper.Map(userSettings);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual);
        Assert.Equal(userSettings.RequireUniqueEmail, actual.RequireUniqueEmail);
        Assert.Equal(userSettings.AllowedUserNameCharacters, actual.AllowedUserNameCharacters);
    }
}