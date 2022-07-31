using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Services;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class UserServiceTests
{
    private const string Password = "PASSWORD001";
    private const string UserName = "USERNAME001";
    private static readonly Guid Id = Guid.Parse("390562cf-e1af-4d4f-9aaf-da5b4a6163bf");

    private readonly UserService _sut;
    private readonly User _user = new() { Id = Id, UserName = UserName };
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly Mock<DatabaseContext> _databaseContextMock;

    public UserServiceTests()
    {
        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
        );
        _databaseContextMock = new Mock<DatabaseContext>();

        _sut = new UserService(_databaseContextMock.Object, _userManagerMock.Object, Mock.Of<IMediator>(), Mock.Of<ILogger<UserService>>());
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

    [Fact]
    public async Task GetUserById_UserExists_ReturnsUser()
    {
        // Arrange
        _databaseContextMock.Setup(x => x.Users.FindAsync(Id)).ReturnsAsync(_user);

        // Act
        var actual = await _sut.GetUserById(Id);

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(Id, actual.Id);
        Assert.Equal(UserName, actual.UserName);
    }

    [Fact]
    public async Task GetUserById_UserDoesNotExists_ReturnsNull()
    {
        // Arrange
        _databaseContextMock.Setup(x => x.Users.FindAsync(Id));

        // Act
        var actual = await _sut.GetUserById(Id);

        // Assert
        Assert.Null(actual);
    }
}