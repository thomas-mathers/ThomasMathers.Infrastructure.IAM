using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
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
    private const string Password = "tH0m@s123!";
    private readonly Fixture _fixture;
    private readonly Role _role;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IUserService _sut;

    public UserServiceIntegrationTests()
    {
        _fixture = new Fixture();
        _fixture.Customize<User>(composer =>
            composer.OmitAutoProperties()
            .With(u => u.Id)
            .With(u => u.UserName)
            .With(u => u.Email, () => $"{Guid.NewGuid()}@{Guid.NewGuid()}.com")
        );

        _role = _fixture.Create<Role>();
        
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
        var user = _fixture.Create<User>();
        
        user.Email = email;

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "InvalidEmail"
            }
        };

        // Act
        var registerResponse = await _sut.Register(user, _role.Name, Password);

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
        var user = _fixture.Create<User>();
        
        user.UserName = username;

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "InvalidUserName"
            }
        };

        // Act
        var registerResponse = await _sut.Register(user, _role.Name, Password);

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
        var user = _fixture.Create<User>();

        await _userManager.CreateAsync(user, password);

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = expectedValidationError
            }
        };

        // Act
        var registerResponse = await _sut.Register(user, _role.Name, password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Fact]
    public async Task Register_DuplicateUserName_ReturnsIdentityErrorResponse()
    {
        // Arrange
        var user1 = _fixture.Create<User>();
        var user2 = _fixture.Create<User>();

        user2.UserName = user1.UserName;

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "DuplicateUserName"
            }
        };

        await _userManager.CreateAsync(user1, Password);

        // Act
        var registerResponse = await _sut.Register(user2, _role.Name, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Fact]
    public async Task Register_DuplicateEmail_ReturnsIdentityErrorResponse()
    {
        // Arrange
        var user1 = _fixture.Create<User>();
        var user2 = _fixture.Create<User>();
        
        user2.Email = user1.Email;

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "DuplicateEmail"
            }
        };

        await _userManager.CreateAsync(user1, Password);

        // Act
        var registerResponse = await _sut.Register(user2, _role.Name, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT0);
        Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
    }

    [Fact]
    public async Task Register_RoleDoesNotExist_ReturnsNotFoundResponse()
    {
        // Arrange
        var user = _fixture.Create<User>();

        // Act
        var registerResponse = await _sut.Register(user, _role.Name, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT1);
    }

    [Fact]
    public async Task Register_Valid_ReturnsSuccessResponse()
    {
        // Arrange
        var user = _fixture.Create<User>();

        await _roleManager.CreateAsync(_role);

        // Act
        var registerResponse = await _sut.Register(user, _role.Name, Password);

        // Assert
        Assert.NotNull(registerResponse);
        Assert.True(registerResponse.IsT2);
    }

    [Fact]
    public async Task GetUserById_UserExists_ReturnsUser()
    {
        // Arrange
        var user = _fixture.Create<User>();

        await _userManager.CreateAsync(user, Password);

        // Act
        var actual = await _sut.GetUserById(user.Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(user.Id, actual.Id);
        Assert.Equal(user.UserName, actual.UserName);
        Assert.Equal(user.Email, actual.Email);
    }

    [Fact]
    public async Task GetUserById_UserDoesNotExists_ReturnsNull()
    {
        // Arrange
        var id = _fixture.Create<Guid>();

        // Act
        var actual = await _sut.GetUserById(id);

        // Assert
        Assert.Null(actual);
    }

    [Fact]
    public async Task GetAllUsers_ReturnsAll()
    {
        // Arrange
        var user1 = _fixture.Create<User>();
        var user2 = _fixture.Create<User>();

        await _userManager.CreateAsync(user1, Password);
        await _userManager.CreateAsync(user2, Password);

        // Act
        var actual = await _sut.GetAllUsers();

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(2, actual.Count);
        Assert.Equal(user1.Id, actual[0].Id);
        Assert.Equal(user1.UserName, actual[0].UserName);
        Assert.Equal(user1.Email, actual[0].Email);
        Assert.Equal(user2.Id, actual[1].Id);
        Assert.Equal(user2.UserName, actual[1].UserName);
        Assert.Equal(user2.Email, actual[1].Email);
    }

    [Fact]
    public async Task DeleteUser_DeletesUser()
    {
        // Arrange
        var user = _fixture.Create<User>();
        await _userManager.CreateAsync(user, Password);

        // Act
        await _sut.DeleteUser(user);
        var deletedUser = await _userManager.FindByIdAsync(user.Id.ToString());

        // Assert
        Assert.Null(deletedUser);
    }
}