using Microsoft.AspNetCore.Identity;
using Moq;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Services;
using System.Threading.Tasks;
using Xunit;
using MediatR;

namespace ThomasMathers.Infrastructure.IAM.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<IMediator> _mediatorMock;
        private readonly UserService _sut;
        private readonly User _user = new() { UserName = "USERNAME001" };
        private const string _password = "PASSWORD001";

        public UserServiceTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
            );

            _mediatorMock = new Mock<IMediator>();

            _sut = new UserService(_userManagerMock.Object, _mediatorMock.Object);
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
                .Setup(x => x.CreateAsync(It.IsAny<User>(), _password))
                .ReturnsAsync(IdentityResult.Failed(errors));

            // Act
            var result = await _sut.Register(_user, _password);

            // Assert
            Assert.Equal(0, result.Index);
            Assert.Equal(errors, result.AsT0.Errors);
        }

        [Fact]
        public async Task Register_Success_ReturnsCorrectResponseType()
        {
            // Arrange
            _userManagerMock
                .Setup(x => x.CreateAsync(It.IsAny<User>(), _password))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _sut.Register(_user, _password);

            // Assert
            Assert.Equal(1, result.Index);
        }
    }
}