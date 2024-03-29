﻿using System;
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

public class AuthServiceIntegrationTests
{
    private const string Password1 = "P@sSw0rd1!";
    private const string Password2 = "P@sSw0rd2!";
    private readonly Fixture _fixture;
    private readonly User _user;
    private readonly UserManager<User> _userManager;
    private readonly IAuthService _sut;

    public AuthServiceIntegrationTests()
    {
        _fixture = new Fixture();
        _fixture.Customize<User>(composer =>
            composer.OmitAutoProperties()
            .With(u => u.Id)
            .With(u => u.UserName)
            .With(u => u.Email, () => $"{Guid.NewGuid()}@{Guid.NewGuid()}.com")
        );

        _user = _fixture.Create<User>();

        var serviceProvider = ServiceProviderBuilder.Build();

        _userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        _sut = serviceProvider.GetRequiredService<IAuthService>();
    }

    [Fact]
    public async Task Login_UserDoesNotExist_ReturnsNotFoundResponse()
    {
        // Act
        var loginResponse = await _sut.Login(_user.UserName, Password1);

        // Assert
        Assert.NotNull(loginResponse);
        Assert.True(loginResponse.IsT0);
    }

    [Fact]
    public async Task Login_WrongPassword_ReturnsFailureResponse()
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password2);

        // Act
        var loginResponse = await _sut.Login(_user.UserName, Password1);

        // Assert
        Assert.NotNull(loginResponse);
        Assert.True(loginResponse.IsT4);
    }

    [Fact]
    public async Task Login_MultipleWrongPasswords_ReturnsLockedOutResponse()
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password2);

        for (var i = 0; i < 5; i++)
        {
            _ = await _sut.Login(_user.UserName, Password1);
        }

        // Act
        var loginResponse = await _sut.Login(_user.UserName, Password1);

        // Assert
        Assert.NotNull(loginResponse);
        Assert.True(loginResponse.IsT1);
    }

    [Fact]
    public async Task Login_CorrectPassword_ReturnsSuccessResponse()
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password1);

        // Act
        var loginResponse = await _sut.Login(_user.UserName, Password1);

        // Assert
        Assert.NotNull(loginResponse);
        Assert.True(loginResponse.IsT5);
    }

    [Fact]
    public async Task ChangePassword_UserDoesNotExist_ReturnsNotFoundResponse()
    {
        // Act
        var changePassword = await _sut.ChangePassword(
            _user.UserName,
            Password1,
            string.Empty,
            Password2);

        // Assert
        Assert.NotNull(changePassword);
        Assert.True(changePassword.IsT0);
    }

    [Fact]
    public async Task ChangePassword_WrongPassword_ReturnsIdentityErrorResponse()
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password1);

        // Act
        var changePassword = await _sut.ChangePassword(_user.UserName, Password2, string.Empty, Password2);

        // Assert
        Assert.NotNull(changePassword);
        Assert.True(changePassword.IsT1);
    }

    [Theory]
    [InlineData("aB(1", "PasswordTooShort")]
    [InlineData("aB(12", "PasswordTooShort")]
    [InlineData("aB(def", "PasswordRequiresDigit")]
    [InlineData("a2345@", "PasswordRequiresUpper")]
    [InlineData("A2345@", "PasswordRequiresLower")]
    [InlineData("aB3456", "PasswordRequiresNonAlphanumeric")]
    public async Task ChangePassword_InvalidPassword_ReturnsIdentityErrorResponse(string newPassword,
        string expectedValidationError)
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password1);

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = expectedValidationError
            }
        };

        // Act
        var changePassword = await _sut.ChangePassword(_user.UserName, Password1, string.Empty, newPassword);

        // Assert
        Assert.NotNull(changePassword);
        Assert.True(changePassword.IsT1);
        Assert.Equal(expectedErrors, changePassword.AsT1.Errors, new IdentityErrorComparer());
    }

    [Theory]
    [InlineData("aB(123")]
    public async Task ChangePassword_ValidNewPassword_ReturnsSuccessResponse(string newPassword)
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password1);

        // Act
        var changePassword = await _sut.ChangePassword(_user.UserName, Password1, string.Empty, newPassword);

        // Assert
        Assert.NotNull(changePassword);
        Assert.True(changePassword.IsT2);
    }

    [Fact]
    public async Task GenerateResetPasswordToken_UserExists_ReturnsSuccessResponse()
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password1);

        // Act
        var resetPasswordResponse = await _sut.ResetPassword(_user.UserName);

        // Assert
        Assert.NotNull(resetPasswordResponse);
        Assert.True(resetPasswordResponse.IsT1);
        Assert.NotNull(resetPasswordResponse.AsT1.Token);
        Assert.NotEmpty(resetPasswordResponse.AsT1.Token);
    }

    [Fact]
    public async Task ResetPassword_UserDoesNotExist_ReturnsNotFoundResponse()
    {
        // Act
        var resetPassword = await _sut.ChangePassword(_user.UserName, string.Empty, string.Empty, Password2);

        // Assert
        Assert.NotNull(resetPassword);
        Assert.True(resetPassword.IsT0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc123")]
    public async Task ResetPassword_InvalidToken_ReturnsIdentityErrorResponse(string token)
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password1);

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = "InvalidToken"
            }
        };

        // Act
        var resetPassword = await _sut.ChangePassword(_user.UserName, string.Empty, token, Password2);

        // Assert
        Assert.NotNull(resetPassword);
        Assert.True(resetPassword.IsT1);
        Assert.Equal(expectedErrors, resetPassword.AsT1.Errors, new IdentityErrorComparer());
    }

    [Theory]
    [InlineData("aB(1", "PasswordTooShort")]
    [InlineData("aB(12", "PasswordTooShort")]
    [InlineData("aB(def", "PasswordRequiresDigit")]
    [InlineData("a2345@", "PasswordRequiresUpper")]
    [InlineData("A2345@", "PasswordRequiresLower")]
    [InlineData("aB3456", "PasswordRequiresNonAlphanumeric")]
    public async Task ResetPassword_InvalidPassword_ReturnsIdentityErrorResponse(string newPassword,
        string expectedValidationError)
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password1);

        var token = await _userManager.GeneratePasswordResetTokenAsync(_user);

        var expectedErrors = new List<IdentityError>
        {
            new()
            {
                Code = expectedValidationError
            }
        };

        // Act
        var resetPassword = await _sut.ChangePassword(_user.UserName, string.Empty, token, newPassword);

        // Assert
        Assert.NotNull(resetPassword);
        Assert.True(resetPassword.IsT1);
        Assert.Equal(expectedErrors, resetPassword.AsT1.Errors, new IdentityErrorComparer());
    }

    [Fact]
    public async Task ResetPassword_ValidTokenValidPassword_ReturnsSuccessResponse()
    {
        // Arrange
        _ = await _userManager.CreateAsync(_user, Password1);
        var token = await _userManager.GeneratePasswordResetTokenAsync(_user);

        // Act
        var resetPassword = await _sut.ChangePassword(_user.UserName, string.Empty, token, Password2);

        // Assert
        Assert.NotNull(resetPassword);
        Assert.True(resetPassword.IsT2);
    }
}