using ThomasMathers.Infrastructure.IAM.Mappers;
using ThomasMathers.Infrastructure.IAM.Settings;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class PasswordSettingsMapperTests
{
    [Theory]
    [InlineData(false, 2, 1, false, false, false)]
    [InlineData(false, 7, 3, false, false, true)]
    [InlineData(false, 8, 4, false, true, false)]
    [InlineData(false, 9, 6, false, true, true)]
    [InlineData(false, 14, 2, true, false, false)]
    [InlineData(false, 15, 3, true, false, true)]
    [InlineData(false, 7, 5, true, true, false)]
    [InlineData(false, 8, 9, true, true, true)]
    [InlineData(true, 9, 5, false, false, false)]
    [InlineData(true, 7, 4, false, false, true)]
    [InlineData(true, 3, 2, false, true, false)]
    [InlineData(true, 8, 5, false, true, true)]
    [InlineData(true, 6, 3, true, false, false)]
    [InlineData(true, 7, 2, true, false, true)]
    [InlineData(true, 5, 3, true, true, false)]
    [InlineData(true, 6, 1, true, true, true)]
    public void Map_MapsCorrectly(bool requireDigit, int requiredLength, int requiredUniqueChars, bool requireLowercase,
        bool requireNonAlphanumeric, bool requireUppercase)
    {
        // Arrange
        var source = new PasswordSettings
        {
            RequireDigit = requireDigit,
            RequiredLength = requiredLength,
            RequiredUniqueChars = requiredUniqueChars,
            RequireLowercase = requireLowercase,
            RequireNonAlphanumeric = requireNonAlphanumeric,
            RequireUppercase = requireUppercase
        };

        // Act
        var actual = PasswordOptionsMapper.Map(source);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(requireDigit, actual.RequireDigit);
        Assert.Equal(requiredLength, actual.RequiredLength);
        Assert.Equal(requiredUniqueChars, actual.RequiredUniqueChars);
        Assert.Equal(requireLowercase, actual.RequireLowercase);
        Assert.Equal(requireNonAlphanumeric, actual.RequireNonAlphanumeric);
        Assert.Equal(requireUppercase, actual.RequireUppercase);
    }
}