using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Services;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests.Services;

public class AuthServiceTests
{
    private const string Username = "USERNAME_001";
    private const string Password = "PASSWORD_001";
    private const string AccessToken = "ACCESS_TOKEN_001";
    private const string PasswordResetToken = "PASSWORD_RESET_TOKEN_001";

    private static readonly IdentityError[] Errors =
    {
        new() { Code = "CODE_001", Description = "DESCRIPTION_001" },
        new() { Code = "CODE_002", Description = "DESCRIPTION_002" },
        new() { Code = "CODE_003", Description = "DESCRIPTION_003" }
    };

    private readonly Mock<IAccessTokenGenerator> _accessTokenGeneratorMock;
    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly AuthService _sut;
    private readonly User _user = new() { UserName = Username };
    private readonly Mock<UserManager<User>> _userManagerMock;

    public AuthServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
        );

        _signInManagerMock = new Mock<SignInManager<User>>(_userManagerMock.Object,
            Mock.Of<IHttpContextAccessor>(),
            Mock.Of<IUserClaimsPrincipalFactory<User>>(),
            null,
            null,
            null,
            null);

        _accessTokenGeneratorMock = new Mock<IAccessTokenGenerator>();

        _sut = new AuthService(
            _signInManagerMock.Object,
            _userManagerMock.Object,
            _accessTokenGeneratorMock.Object,
            Mock.Of<IMediator>(),
            Mock.Of<ILogger<AuthService>>());
    }

    [Fact]
    public async Task Login_UserDoesNotExist_ReturnsNotFound()
    {
        // Act
        var result = await _sut.Login(Username, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task Login_LockedOut_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, Password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.LockedOut);

        // Act
        var result = await _sut.Login(Username, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT1);
    }

    [Fact]
    public async Task Login_TwoFactorRequired_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, Password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.TwoFactorRequired);

        // Act
        var result = await _sut.Login(Username, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT2);
    }

    [Fact]
    public async Task Login_NotAllowed_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, Password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.NotAllowed);

        // Act
        var result = await _sut.Login(Username, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT3);
    }

    [Fact]
    public async Task Login_Failed_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, Password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _sut.Login(Username, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT4);
    }

    [Fact]
    public async Task Login_Success_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, Password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        _accessTokenGeneratorMock.Setup(x => x.GenerateAccessToken(_user))
            .ReturnsAsync(AccessToken);

        // Act
        var result = await _sut.Login(Username, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT5);
        Assert.Equal(Username, result.AsT5.User.UserName);
        Assert.Equal(AccessToken, result.AsT5.AccessToken);
    }

    [Fact]
    public async Task ChangePassword_UserDoesNotExist_ReturnsCorrectResponseType()
    {
        // Act
        var result = await _sut.ChangePassword(Username, Password, string.Empty, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task ChangePassword_IdentityErrors_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.ChangePasswordAsync(_user, Password, Password))
            .ReturnsAsync(IdentityResult.Failed(Errors));

        // Act
        var result = await _sut.ChangePassword(Username, Password, string.Empty, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT1);
        Assert.Equal(Errors, result.AsT1.Errors);
    }

    [Fact]
    public async Task ChangePassword_Success_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.ChangePasswordAsync(_user, Password, Password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.ChangePassword(Username, Password, string.Empty, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT2);
    }

    [Fact]
    public async Task GeneratePasswordResetToken_UserDoesNotExist_ReturnsCorrectResponseType()
    {
        // Act
        var resetPasswordResponse = await _sut.ResetPassword(Username);

        // Assert
        Assert.NotNull(resetPasswordResponse);
        Assert.True(resetPasswordResponse.IsT0);
    }

    [Fact]
    public async Task GeneratePasswordResetToken_UserExists_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(_user))
            .ReturnsAsync(PasswordResetToken);

        // Act
        var resetPasswordResponse = await _sut.ResetPassword(Username);

        // Assert
        Assert.NotNull(resetPasswordResponse);
        Assert.True(resetPasswordResponse.IsT1);
        Assert.Equal(PasswordResetToken, resetPasswordResponse.AsT1.Token);
    }

    [Fact]
    public async Task ResetPassword_UserDoesNotExist_ReturnsCorrectResponseType()
    {
        // Act
        var result = await _sut.ChangePassword(Username, string.Empty, PasswordResetToken, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task ResetPassword_IdentityError_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.ResetPasswordAsync(_user, PasswordResetToken, Password))
            .ReturnsAsync(IdentityResult.Failed(Errors));

        // Act
        var result = await _sut.ChangePassword(Username, string.Empty, PasswordResetToken, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT1);
        Assert.Equal(Errors, result.AsT1.Errors);
    }

    [Fact]
    public async Task ResetPassword_Success_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock.Setup(x => x.FindByNameAsync(Username)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.ResetPasswordAsync(_user, PasswordResetToken, Password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.ChangePassword(Username, string.Empty, PasswordResetToken, Password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT2);
    }
}