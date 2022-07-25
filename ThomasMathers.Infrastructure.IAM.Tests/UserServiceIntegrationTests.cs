using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Services;
using ThomasMathers.Infrastructure.IAM.Tests.Comparers;
using ThomasMathers.Infrastructure.IAM.Tests.Helpers;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class UserServiceIntegrationTests
{
    private const string Username1 = "tmathers";
    private const string Username2 = "didymus";
    private const string Email1 = "thomas.mathers.pro@gmail.com";
    private const string Email2 = "mathers_thomas@hotmail.com";
    private const string Password = "P@sSw0rd1!";
    private readonly IUserService _sut;
    private readonly User _user1 = new() { UserName = Username1, Email = Email1 };
    private readonly User _user2 = new() { UserName = Username1, Email = Email2 };
    private readonly User _user3 = new() { UserName = Username2, Email = Email1 };
    private readonly UserManager<User> _userManager;

    public UserServiceIntegrationTests()
    {
        var serviceProvider = ServiceProviderBuilder.Build();

        _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        _sut = serviceProvider.GetRequiredService<IUserService>();
    }

    [Theory]
    [InlineData("@")]
    [InlineData("a@")]
    [InlineData("@a")]
    [InlineData("a")]
    public async Task Register_InvalidEmail_ReturnsIdentityErrorResponse(string email)
    {
        // Arrange
        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "InvalidEmail"
            }
        };

        // Act
        var registerResponse = await _sut.Register(new User { UserName = Username1, Email = email }, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Theory]
    [InlineData("")]
    [InlineData("!@#$%^&*()_+")]
    [InlineData("Thomas!")]
    public async Task Register_InvalidUsername_ReturnsIdentityErrorResponse(string username)
    {
        // Arrange
        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "InvalidUserName"
            }
        };

        // Act
        var registerResponse = await _sut.Register(new User { UserName = username, Email = Email1 }, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Theory]
    [InlineData("aB(1", "PasswordTooShort")]
    [InlineData("aB(12", "PasswordTooShort")]
    [InlineData("aB(def", "PasswordRequiresDigit")]
    [InlineData("a2345@", "PasswordRequiresUpper")]
    [InlineData("A2345@", "PasswordRequiresLower")]
    [InlineData("aB3456", "PasswordRequiresNonAlphanumeric")]
    public async Task Register_InvalidPassword_ReturnsIdentityErrorResponse(string password,
        string expectedValidationError)
    {
        // Arrange
        await _userManager.CreateAsync(_user1, Password);

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = expectedValidationError
            }
        };

        // Act
        var registerResponse = await _sut.Register(_user1, password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Fact]
    public async Task Register_DuplicateUserName_ReturnsIdentityErrorResponse()
    {
        // Arrange
        await _userManager.CreateAsync(_user1, Password);

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "DuplicateUserName"
            }
        };

        // Act
        var registerResponse = await _sut.Register(_user2, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsIdentityErrorResponse()
    {
        // Arrange
        await _userManager.CreateAsync(_user1, Password);

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "DuplicateEmail"
            }
        };

        // Act
        var registerResponse = await _sut.Register(_user3, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Fact]
    public async Task Register_Valid_ReturnsSuccessResponse()
    {
        // Act
        var registerResponse = await _sut.Register(_user1, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT1);
    }
}