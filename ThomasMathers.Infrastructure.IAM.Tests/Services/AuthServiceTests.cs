using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
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
    private readonly Fixture _fixture;
    private readonly User _user;

    private readonly Mock<SignInManager<User>> _signInManagerMock;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<IAccessTokenGenerator> _accessTokenGeneratorMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _fixture = new Fixture();
        _user = _fixture.Create<User>();

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
        // Arrange
        var username = _fixture.Create<string>();
        var password = _fixture.Create<string>();

        // Act
        var result = await _sut.Login(username, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task Login_LockedOut_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.LockedOut);

        // Act
        var result = await _sut.Login(_user.UserName, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT1);
    }

    [Fact]
    public async Task Login_TwoFactorRequired_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.TwoFactorRequired);

        // Act
        var result = await _sut.Login(_user.UserName, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT2);
    }

    [Fact]
    public async Task Login_NotAllowed_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.NotAllowed);

        // Act
        var result = await _sut.Login(_user.UserName, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT3);
    }

    [Fact]
    public async Task Login_Failed_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Failed);

        // Act
        var result = await _sut.Login(_user.UserName, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT4);
    }

    [Fact]
    public async Task Login_Success_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var accessToken = _fixture.Create<string>();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);

        _signInManagerMock
            .Setup(x => x.CheckPasswordSignInAsync(_user, password, It.IsAny<bool>()))
            .ReturnsAsync(SignInResult.Success);

        _accessTokenGeneratorMock.Setup(x => x.GenerateAccessToken(_user))
            .ReturnsAsync(accessToken);

        // Act
        var result = await _sut.Login(_user.UserName, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT5);
        Assert.Equal(_user.UserName, result.AsT5.User.UserName);
        Assert.Equal(accessToken, result.AsT5.AccessToken);
    }

    [Fact]
    public async Task ChangePassword_UserDoesNotExist_ReturnsCorrectResponseType()
    {
        // Arrange
        var username = _fixture.Create<string>();
        var password = _fixture.Create<string>();

        // Act
        var result = await _sut.ChangePassword(username, password, string.Empty, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task ChangePassword_IdentityErrors_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var errors = _fixture.CreateMany<IdentityError>().ToArray();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.ChangePasswordAsync(_user, password, password))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _sut.ChangePassword(_user.UserName, password, string.Empty, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT1);
        Assert.Equal(errors, result.AsT1.Errors);
    }

    [Fact]
    public async Task ChangePassword_Success_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.ChangePasswordAsync(_user, password, password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.ChangePassword(_user.UserName, password, string.Empty, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT2);
    }

    [Fact]
    public async Task GeneratePasswordResetToken_UserDoesNotExist_ReturnsCorrectResponseType()
    {
        // Arrange
        var username = _fixture.Create<string>();

        // Act
        var resetPasswordResponse = await _sut.ResetPassword(username);

        // Assert
        Assert.NotNull(resetPasswordResponse);
        Assert.True(resetPasswordResponse.IsT0);
    }

    [Fact]
    public async Task GeneratePasswordResetToken_UserExists_ReturnsCorrectResponseType()
    {
        // Arrange
        var passwordResetToken = _fixture.Create<string>();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.GeneratePasswordResetTokenAsync(_user))
            .ReturnsAsync(passwordResetToken);

        // Act
        var resetPasswordResponse = await _sut.ResetPassword(_user.UserName);

        // Assert
        Assert.NotNull(resetPasswordResponse);
        Assert.True(resetPasswordResponse.IsT1);
        Assert.Equal(passwordResetToken, resetPasswordResponse.AsT1.Token);
    }

    [Fact]
    public async Task ResetPassword_UserDoesNotExist_ReturnsCorrectResponseType()
    {
        // Arrange
        var username = _fixture.Create<string>();
        var password = _fixture.Create<string>();
        var passwordResetToken = _fixture.Create<string>();

        // Act
        var result = await _sut.ChangePassword(username, string.Empty, passwordResetToken, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT0);
    }

    [Fact]
    public async Task ResetPassword_IdentityError_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var passwordResetToken = _fixture.Create<string>();
        var errors = _fixture.CreateMany<IdentityError>().ToArray();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.ResetPasswordAsync(_user, passwordResetToken, password))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _sut.ChangePassword(_user.UserName, string.Empty, passwordResetToken, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT1);
        Assert.Equal(errors, result.AsT1.Errors);
    }

    [Fact]
    public async Task ResetPassword_Success_ReturnsCorrectResponseType()
    {
        // Arrange
        var password = _fixture.Create<string>();
        var passwordResetToken = _fixture.Create<string>();

        _userManagerMock.Setup(x => x.FindByNameAsync(_user.UserName)).ReturnsAsync(_user);
        _userManagerMock
            .Setup(x => x.ResetPasswordAsync(_user, passwordResetToken, password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.ChangePassword(_user.UserName, string.Empty, passwordResetToken, password);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsT2);
    }
}