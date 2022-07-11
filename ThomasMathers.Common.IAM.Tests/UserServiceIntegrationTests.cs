using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Common.IAM.Data;
using ThomasMathers.Common.IAM.Requests;
using ThomasMathers.Common.IAM.Services;
using ThomasMathers.Common.IAM.Settings;
using ThomasMathers.Common.IAM.Tests.Comparers;
using ThomasMathers.Common.IAM.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace ThomasMathers.Common.IAM.Tests
{
    public class UserServiceIntegrationTests
    {
        private readonly IUserService _sut;
        private readonly UserManager<User> _userManager;
        private const string _username1 = "tmathers";
        private const string _username2 = "didymus";
        private const string _email1 = "thomas.mathers.pro@gmail.com";
        private const string _email2 = "mathers_thomas@hotmail.com";
        private const string _password = "P@sSw0rd1!";
        private readonly User _user = new() { UserName = _username1, Email = _email1 };

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
                new IdentityError
                {
                    Code = "InvalidEmail"
                }
            };

            // Act
            var registerResponse = await _sut.Register(new RegisterRequest
            {
                UserName = _username1,
                Email = email,
                Password = _password
            });

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
                new IdentityError
                {
                    Code = "InvalidUserName"
                }
            };

            // Act
            var registerResponse = await _sut.Register(new RegisterRequest
            {
                UserName = username,
                Email = _email1,
                Password = _password
            });

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
        public async Task Register_InvalidPassword_ReturnsIdentityErrorResponse(string password, string expectedValidationError)
        {
            // Arrange
            await _userManager.CreateAsync(_user, _password);

            var expectedErrors = new List<IdentityError>
            {
                new IdentityError
                {
                    Code = expectedValidationError
                }
            };

            // Act
            var registerResponse = await _sut.Register(new RegisterRequest
            {
                UserName = _username1,
                Email = _email1,
                Password = password
            });

            // Assert
            Assert.NotNull(registerResponse);
            Assert.True(registerResponse.IsT0);
            Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
        }

        [Fact]
        public async Task Register_DuplicateUserName_ReturnsIdentityErrorResponse()
        {
            // Arrange
            await _userManager.CreateAsync(_user, _password);

            var expectedErrors = new List<IdentityError>
            {
                new IdentityError
                {
                    Code = "DuplicateUserName"
                }
            };

            // Act
            var registerResponse = await _sut.Register(new RegisterRequest
            {
                UserName = _username1,
                Email = _email2,
                Password = _password
            });

            // Assert
            Assert.NotNull(registerResponse);
            Assert.True(registerResponse.IsT0);
            Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
        }

        [Fact]
        public async Task Register_DuplicateEmail_ReturnsIdentityErrorResponse()
        {
            // Arrange
            await _userManager.CreateAsync(_user, _password);

            var expectedErrors = new List<IdentityError>
            {
                new IdentityError
                {
                    Code = "DuplicateEmail"
                }
            };

            // Act
            var registerResponse = await _sut.Register(new RegisterRequest
            {
                UserName = _username2,
                Email = _email1,
                Password = _password
            });

            // Assert
            Assert.NotNull(registerResponse);
            Assert.True(registerResponse.IsT0);
            Assert.Equal(expectedErrors, registerResponse.AsT0.Errors, new IdentityErrorComparer());
        }

        [Fact]
        public async Task Register_Valid_ReturnsSuccessResponse()
        {
            // Act
            var registerResponse = await _sut.Register(new RegisterRequest
            {
                UserName = _username1,
                Email = _email1,
                Password = _password
            });

            // Assert
            Assert.NotNull(registerResponse);
            Assert.True(registerResponse.IsT1);
        }
    }
}
