using ThomasMathers.Common.IAM.Mappers;
using ThomasMathers.Common.IAM.Settings;
using Xunit;

namespace ThomasMathers.Common.IAM.Tests
{
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
                AllowedUserNameCharacters = allowedUserNameCharacters,
            };

            // Act
            var actual = UserSettingsMapper.Map(userSettings);

            // Assert
            Assert.NotNull(actual);
            Assert.NotNull(actual);
            Assert.Equal(requireUniqueEmail, actual.RequireUniqueEmail);
            Assert.Equal(allowedUserNameCharacters, actual.AllowedUserNameCharacters);
        }
    }
}
