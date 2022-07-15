using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Moq;
using ThomasMathers.Common.IAM.Data;
using ThomasMathers.Common.IAM.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace ThomasMathers.Common.IAM.Tests
{
    public class AuthServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IAccessTokenGenerator> _accessTokenGeneratorMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly User _user = new() { UserName = _username };
        private readonly AuthService _sut;
        private const string _username = "USERNAME_001";
        private const string _password = "PASSWORD_001";
        private const string _accessToken = "ACCESS_TOKEN_001";
        private const string _passwordResetToken = "PASSWORD_RESET_TOKEN_001";
        private static readonly IdentityError[] _errors = new[]
        {
            new IdentityError { Code = "CODE_001", Description = "DESCRIPTION_001" },
            new IdentityError { Code = "CODE_002", Description = "DESCRIPTION_002" },
            new IdentityError { Code = "CODE_003", Description = "DESCRIPTION_003" },
        };

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

            _mediatorMock = new Mock<IMediator>();

            _sut = new AuthService(_signInManagerMock.Object, _userManagerMock.Object, _accessTokenGeneratorMock.Object, _mediatorMock.Object);
        }

        [Fact]
        public async Task Login_UserDoesNotExist_ReturnsNotFound()
        {
            // Act
            var result = await _sut.Login(_username, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT0);
        }

        [Fact]
        public async Task Login_LockedOut_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(_user, _password, It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.LockedOut);

            // Act
            var result = await _sut.Login(_username, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT1);
        }

        [Fact]
        public async Task Login_TwoFactorRequired_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(_user, _password, It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.TwoFactorRequired);

            // Act
            var result = await _sut.Login(_username, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT2);
        }

        [Fact]
        public async Task Login_NotAllowed_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(_user, _password, It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.NotAllowed);

            // Act
            var result = await _sut.Login(_username, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT3);
        }

        [Fact]
        public async Task Login_Failed_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(_user, _password, It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Failed);

            // Act
            var result = await _sut.Login(_username, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT4);
        }

        [Fact]
        public async Task Login_Success_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);

            _signInManagerMock
                .Setup(x => x.CheckPasswordSignInAsync(_user, _password, It.IsAny<bool>()))
                .ReturnsAsync(SignInResult.Success);

            _accessTokenGeneratorMock.Setup(x => x.GenerateAccessToken(It.IsAny<IEnumerable<Claim>>())).Returns(_accessToken);

            // Act
            var result = await _sut.Login(_username, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT5);
            Assert.Equal(_username, result.AsT5.UserName);
            Assert.Equal(_accessToken, result.AsT5.AccessToken);
        }

        [Fact]
        public async Task ChangePassword_UserDoesNotExist_ReturnsCorrectResponseType()
        {
            // Act
            var result = await _sut.ChangePassword(_username, _password, string.Empty, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT0);
        }

        [Fact]
        public async Task ChangePassword_IdentityErrors_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);
            _userManagerMock
                .Setup(x => x.ChangePasswordAsync(_user, _password, _password))
                .ReturnsAsync(IdentityResult.Failed(_errors));

            // Act
            var result = await _sut.ChangePassword(_username, _password, string.Empty, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT1);
            Assert.Equal(_errors, result.AsT1.Errors);
        }

        [Fact]
        public async Task ChangePassword_Success_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);
            _userManagerMock
                .Setup(x => x.ChangePasswordAsync(_user, _password, _password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.ChangePassword(_username, _password, string.Empty, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT2);
        }

        [Fact]
        public async Task GeneratePasswordResetToken_UserDoesNotExist_ReturnsCorrectResponseType()
        {
            // Act
            var token = await _sut.ResetPassword(_user);

            // Assert
            Assert.Null(token);
        }

        [Fact]
        public async Task GeneratePasswordResetToken_UserExists_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);
            _userManagerMock
                .Setup(x => x.GeneratePasswordResetTokenAsync(_user))
                .ReturnsAsync(_passwordResetToken);

            // Act
            var token = await _sut.ResetPassword(_user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);
        }

        [Fact]
        public async Task ResetPassword_UserDoesNotExist_ReturnsCorrectResponseType()
        {
            // Act
            var result = await _sut.ChangePassword(_username, string.Empty, _passwordResetToken, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT0);
        }

        [Fact]
        public async Task ResetPassword_IdentityError_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);
            _userManagerMock
                .Setup(x => x.ResetPasswordAsync(_user, _passwordResetToken, _password))
                .ReturnsAsync(IdentityResult.Failed(_errors));

            // Act
            var result = await _sut.ChangePassword(_username, string.Empty, _passwordResetToken, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT1);
            Assert.Equal(_errors, result.AsT1.Errors);
        }

        [Fact]
        public async Task ResetPassword_Success_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByNameAsync(_username)).ReturnsAsync(_user);
            _userManagerMock
                .Setup(x => x.ResetPasswordAsync(_user, _passwordResetToken, _password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.ChangePassword(_username, string.Empty, _passwordResetToken, _password);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.IsT2);
        }
    }
}