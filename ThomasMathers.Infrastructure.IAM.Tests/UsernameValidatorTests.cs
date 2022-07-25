using ThomasMathers.Infrastructure.IAM.Settings;
using ThomasMathers.Infrastructure.IAM.Tests.MockObjects;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class UsernameValidatorTests
{
    private readonly UsernameValidator _sut;

    public UsernameValidatorTests()
    {
        var userSettings = new UserSettings();

        _sut = new UsernameValidator(userSettings);
    }

    [Theory]
    [InlineData("!", false)]
    [InlineData("#", false)]
    [InlineData("$", false)]
    [InlineData("%", false)]
    [InlineData("^", false)]
    [InlineData("&", false)]
    [InlineData("*", false)]
    [InlineData("(", false)]
    [InlineData(")", false)]
    [InlineData("tmathers!", false)]
    [InlineData("", false)]
    [InlineData("tmathers", true)]
    [InlineData("abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+", true)]
    public void Validate_ReturnsExpectedResult(string username, bool expectedIsValid)
    {
        var result = _sut.Validate(username);
        Assert.Equal(expectedIsValid, result.IsValid);
    }
}