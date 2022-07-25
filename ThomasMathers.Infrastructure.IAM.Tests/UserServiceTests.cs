using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Moq;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Services;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class UserServiceTests
{
    private const string Password = "PASSWORD001";
    private readonly UserService _sut;
    private readonly User _user = new() { UserName = "USERNAME001" };
    private readonly Mock<UserManager<User>> _userManagerMock;

    public UserServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
        );

        _sut = new UserService(_userManagerMock.Object, Mock.Of<IMediator>());
    }

    [Fact]
    public async Task Register_Failure_ReturnsCorrectResponseType()
    {
        // Arrange
        var errors = new[]
        {
            new IdentityError { Code = "ERROR1", Description = "ERROR DESCRIPTION 1" },
            new IdentityError { Code = "ERROR2", Description = "ERROR DESCRIPTION 2" },
            new IdentityError { Code = "ERROR3", Description = "ERROR DESCRIPTION 3" }
        };

        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), Password))
            .ReturnsAsync(IdentityResult.Failed(errors));

        // Act
        var result = await _sut.Register(_user, Password);

        // Assert
        Assert.Equal(0, result.Index);
        Assert.Equal(errors, result.AsT0.Errors);
    }

    [Fact]
    public async Task Register_Success_ReturnsCorrectResponseType()
    {
        // Arrange
        _userManagerMock
            .Setup(x => x.CreateAsync(It.IsAny<User>(), Password))
            .ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await _sut.Register(_user, Password);

        // Assert
        Assert.Equal(1, result.Index);
    }
}