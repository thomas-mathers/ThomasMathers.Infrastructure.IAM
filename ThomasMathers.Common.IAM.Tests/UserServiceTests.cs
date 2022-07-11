using Microsoft.AspNetCore.Identity;
using Moq;
using ThomasMathers.Common.IAM.Data;
using ThomasMathers.Common.IAM.Requests;
using ThomasMathers.Common.IAM.Services;
using System.Threading.Tasks;
using Xunit;

namespace ThomasMathers.Common.IAM.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly UserService _sut;
        private const string username = "USERNAME001";
        private const string password = "PASSWORD001";

        public UserServiceTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
            );
            _sut = new UserService(_userManagerMock.Object);
        }

        [Fact]
        public async Task Register_Failure_ReturnsCorrectResponseType()
        {
            // Arrange
            var errors = new[]
            {
                new IdentityError { Code = "ERROR1", Description = "ERROR DESCRIPTION 1" },
                new IdentityError { Code = "ERROR2", Description = "ERROR DESCRIPTION 2" },
                new IdentityError { Code = "ERROR3", Description = "ERROR DESCRIPTION 3" },
            };

            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), password))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _sut.Register(new RegisterRequest
            {
                UserName = username,
                Password = password,
            });

            // Assert
            Assert.Equal(0, result.Index);
            Assert.Equal(errors, result.AsT0.Errors);
        }

        [Fact]
        public async Task Register_Success_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.Register(new RegisterRequest
            {
                UserName = username,
                Password = password,
            });

            // Assert
            Assert.Equal(1, result.Index);
        }
    }
}