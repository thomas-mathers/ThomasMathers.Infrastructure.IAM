using AutoFixture;
using ThomasMathers.Infrastructure.IAM.Mappers;
using ThomasMathers.Infrastructure.IAM.Settings;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests.Mappers;

public class PasswordOptionsMapperTests
{
    private readonly Fixture _fixture;

    public PasswordOptionsMapperTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void Map_MapsCorrectly()
    {
        // Arrange
        var source = _fixture.Create<PasswordSettings>();

        // Act
        var actual = PasswordOptionsMapper.Map(source);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(source.RequireDigit, actual.RequireDigit);
        Assert.Equal(source.RequiredLength, actual.RequiredLength);
        Assert.Equal(source.RequiredUniqueChars, actual.RequiredUniqueChars);
        Assert.Equal(source.RequireLowercase, actual.RequireLowercase);
        Assert.Equal(source.RequireNonAlphanumeric, actual.RequireNonAlphanumeric);
        Assert.Equal(source.RequireUppercase, actual.RequireUppercase);
    }
}