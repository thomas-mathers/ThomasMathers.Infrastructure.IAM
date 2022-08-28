using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

using AutoFixture;

using Microsoft.AspNetCore.Identity;

using Moq;

using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Services;
using ThomasMathers.Infrastructure.IAM.Settings;

using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests.Services;

public class AccessTokenGeneratorTests
{
    private readonly Fixture _fixture;
    private readonly JwtTokenSettings _jwtTokenSettings;
    private readonly Mock<UserManager<User>> _userManagerMock;
    private readonly AccessTokenGenerator _sut;

    public AccessTokenGeneratorTests()
    {
        _fixture = new Fixture();
        _jwtTokenSettings = _fixture.Create<JwtTokenSettings>();

        _userManagerMock = new Mock<UserManager<User>>(
            Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null
        );

        _sut = new AccessTokenGenerator(_userManagerMock.Object, _jwtTokenSettings);
    }

    [Fact]
    public async Task GenerateAccessToken_EncodesCorrectly()
    {
        // Arrange
        var user = _fixture.Create<User>();
        var userRoles = _fixture.CreateMany<string>().ToList();

        _ = _userManagerMock.Setup(x => x.GetRolesAsync(user)).ReturnsAsync(userRoles);

        // Act
        var encodedToken = await _sut.GenerateAccessToken(user);
        var decodedToken = new JwtSecurityTokenHandler().ReadJwtToken(encodedToken);

        // Assert
        Assert.Equal(_jwtTokenSettings.Issuer, decodedToken.Issuer);
        Assert.True(decodedToken.Audiences.Count() == 1);
        Assert.Equal(_jwtTokenSettings.Audience, decodedToken.Audiences.First());

        var nameClaim = decodedToken.Claims.FirstOrDefault(x => x.Type == "name");

        Assert.NotNull(nameClaim);
        Assert.Equal(user.UserName, nameClaim.Value);

        var nameidClaim = decodedToken.Claims.FirstOrDefault(x => x.Type == "nameid");

        Assert.NotNull(nameidClaim);
        Assert.Equal(user.Id.ToString(), nameidClaim.Value);

        var roleClaimValues = decodedToken.Claims.Where(x => x.Type == "role").Select(x => x.Value).ToList();

        Assert.Equal(userRoles, roleClaimValues);

        var emailClaim = decodedToken.Claims.FirstOrDefault(x => x.Type == "email");

        Assert.NotNull(emailClaim);
        Assert.Equal(user.Email, emailClaim.Value);
    }
}