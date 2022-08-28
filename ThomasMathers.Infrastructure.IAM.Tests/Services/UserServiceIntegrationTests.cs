using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Services;
using ThomasMathers.Infrastructure.IAM.Tests.Comparers;
using ThomasMathers.Infrastructure.IAM.Tests.Helpers;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests.Services;

public class UserServiceIntegrationTests
{
    private const string Username1 = "tmathers";
    private const string Username2 = "didymus";
    private const string Email1 = "thomas.mathers.pro@gmail.com";
    private const string Email2 = "mathers_thomas@hotmail.com";
    private const string Password = "P@sSw0rd1!";

    private static readonly Guid Id1 = Guid.Parse("40bc3980-1aa3-4e22-84c9-10b7bf0970bb");
    private static readonly Guid Id2 = Guid.Parse("30e9b028-2293-4814-8196-2f71341fc274");
    private static readonly Guid Id3 = Guid.Parse("9c2939f4-9790-49c4-ac41-58a443211043");

    private static readonly Role Role = new()
    {
        Id = Guid.Parse("00d1d816-2c26-4207-a7e7-4972d75bd59f"),
        Name = "admin",
        NormalizedName = "ADMIN",
    };

    private readonly IUserService _sut;
    private readonly User _user1 = new() { Id = Id1, UserName = Username1, Email = Email1 };
    private readonly User _user2 = new() { Id = Id2, UserName = Username1, Email = Email2 };
    private readonly User _user3 = new() { Id = Id3, UserName = Username2, Email = Email1 };
    private readonly User _user4 = new() { Id = Id2, UserName = Username2, Email = Email2 };
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;

    public UserServiceIntegrationTests()
    {
        var serviceProvider = ServiceProviderBuilder.Build();

        _roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
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
        var registerResponse = await _sut.Register(new User { UserName = Username1, Email = email }, Role.Name, Password);

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
        var registerResponse = await _sut.Register(new User { UserName = username, Email = Email1 }, Role.Name, Password);

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
        var registerResponse = await _sut.Register(_user1, Role.Name, password);

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
        var registerResponse = await _sut.Register(_user2, Role.Name, Password);

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
        var registerResponse = await _sut.Register(_user3, Role.Name, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Fact]
    public async Task Register_RoleDoesNotExist_ReturnsNotFoundResponse()
    {
        // Act
        var registerResponse = await _sut.Register(_user1, Role.Name, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT1);
    }

    [Fact]
    public async Task Register_Valid_ReturnsSuccessResponse()
    {
        // Arrange
        await _roleManager.CreateAsync(Role);

        // Act
        var registerResponse = await _sut.Register(_user1, Role.Name, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT2);
    }

    [Fact]
    public async Task GetUserById_UserExists_ReturnsUser()
    {
        // Arrange
        await _userManager.CreateAsync(_user1, Password);

        // Act
        var actual = await _sut.GetUserById(Id1);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(Id1, actual.Id);
        Assert.Equal(Username1, actual.UserName);
        Assert.Equal(Email1, actual.Email);
    }

    [Fact]
    public async Task GetUserById_UserDoesNotExists_ReturnsNull()
    {
        // Act
        var actual = await _sut.GetUserById(Id1);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsAll()
    {
        // Arrange
        await _userManager.CreateAsync(_user1, Password);
        await _userManager.CreateAsync(_user4, Password);

        // Act
        var actual = await _sut.GetAllUsers();

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(2, actual.Count);
        Assert.Equal(Id1, actual[0].Id);
        Assert.Equal(Username1, actual[0].UserName);
        Assert.Equal(Email1, actual[0].Email);
        Assert.Equal(Id2, actual[1].Id);
        Assert.Equal(Username2, actual[1].UserName);
        Assert.Equal(Email2, actual[1].Email);
    }

    [Fact]
    public async Task DeleteUser_DeletesUser()
    {
        // Arrange
        await _userManager.CreateAsync(_user1, Password);

        // Act
        await _sut.DeleteUser(_user1);
        var user = await _userManager.FindByIdAsync(Id1.ToString());

        // Assert
        Assert.Null(user);
    }
}