using ThomasMathers.Infrastructure.IAM.Mappers;
using ThomasMathers.Infrastructure.IAM.Settings;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class UserSettingsMapperTests
{
    [Theory]
    [InlineData(true, "abcdefghijklmnopqrstuvwxyz")]
    [InlineData(false, "abcdefghijklmnopqrstuvwxyz")]
    public void Map_MapsCorrectly(bool requireUniqueEmail, string allowedUserNameCharacters)
    {
        var userSettings = new UserSettings
        {
            RequireUniqueEmail = requireUniqueEmail,
            AllowedUserNameCharacters = allowedUserNameCharacters
        };

        // Act
        var actual = UserOptionsMapper.Map(userSettings);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual);
        Assert.Equal(requireUniqueEmail, actual.RequireUniqueEmail);
        Assert.Equal(allowedUserNameCharacters, actual.AllowedUserNameCharacters);
    }
}