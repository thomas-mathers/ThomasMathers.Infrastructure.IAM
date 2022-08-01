using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Services;
using ThomasMathers.Infrastructure.IAM.Settings;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class AccessTokenGeneratorTests
{
    private readonly string _audience = "share-my-calendar";
    private readonly string _issuer = "share-my-calendar";
    private readonly string _key = "super-secret-key-12345678";
    private readonly AccessTokenGenerator _sut;
    private readonly Mock<UserManager<User>> _userManagerMock;

    public AccessTokenGeneratorTests()
    {
        var options = new JwtTokenSettings
        {
            Key = _key,
            Audience = _audience,
            Issuer = _issuer,
            LifespanInDays = 1
        };

        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
        );

        _sut = new AccessTokenGenerator(_userManagerMock.Object, options);
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("", "tmathers@gmail.com", "")]
    [InlineData("", "", "123-123-1234")]
    [InlineData("", "tmathers@gmail.com", "123-123-1234")]
    [InlineData("admin", "", "")]
    [InlineData("admin", "tmathers@gmail.com", "")]
    [InlineData("admin", "", "123-123-1234")]
    [InlineData("admin", "tmathers@gmail.com", "123-123-1234")]
    [InlineData("user", "", "")]
    [InlineData("user", "tmathers@gmail.com", "")]
    [InlineData("user", "", "123-123-1234")]
    [InlineData("user", "tmathers@gmail.com", "123-123-1234")]
    public async Task GenerateAccessToken_EncodesCorrectly(string role, string email, string phoneNumber)
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = "tmathers",
            Email = email,
            PhoneNumber = phoneNumber
        };

        var roles = new List<string>();

        if (!string.IsNullOrEmpty(role))
        {
            roles.Add(role);
        }

        _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(roles);

        // Act
        var encodedToken = await _sut.GenerateAccessToken(user);
        var decodedToken = DecodeToken(encodedToken);

        // Assert
        Assert.Equal(_issuer, decodedToken.Issuer);
        Assert.True(decodedToken.Audiences.Count() == 1);
        Assert.Equal(_audience, decodedToken.Audiences.First());

        var nameClaim = decodedToken.Claims.FirstOrDefault(x => x.Type == "name");
        Assert.NotNull(nameClaim);
        Assert.Equal(user.UserName, nameClaim.Value);

        var nameidClaim = decodedToken.Claims.FirstOrDefault(x => x.Type == "nameid");
        Assert.NotNull(nameidClaim);
        Assert.Equal(user.Id.ToString(), nameidClaim.Value);

        var roleClaim = decodedToken.Claims.FirstOrDefault(x => x.Type == "role");

        if (string.IsNullOrEmpty(role))
        {
            Assert.Null(roleClaim);
        }
        else
        {
            Assert.NotNull(roleClaim);
            Assert.Equal(role, roleClaim.Value);
        }

        var emailClaim = decodedToken.Claims.FirstOrDefault(x => x.Type == "email");

        if (string.IsNullOrEmpty(email))
        {
            Assert.Null(emailClaim);
        }
        else
        {
            Assert.NotNull(emailClaim);
            Assert.Equal(email, emailClaim.Value);
        }
    }

    private static JwtSecurityToken DecodeToken(string encodedToken)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(encodedToken);
    }
}