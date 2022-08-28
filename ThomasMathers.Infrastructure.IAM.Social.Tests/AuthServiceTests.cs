using AutoFixture;

using Microsoft.Extensions.Logging;

using Moq;

using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Services;

using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Social.Tests;

public class AuthServiceTests
{
    private readonly Fixture _fixture;
    private readonly string _userId;
    private readonly string _accessToken;
    private readonly string _roleName;
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAccessTokenGenerator> _accessTokenGeneratorMock;
    private readonly Mock<ISocialMediaProfileService> _facebookProfileServiceMock;
    private readonly Mock<ISocialMediaProfileService> _googleProfileServiceMock;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _fixture = new Fixture();
        _userId = _fixture.Create<string>();
        _accessToken = _fixture.Create<string>();
        _roleName = _fixture.Create<string>();

        _userServiceMock = new Mock<IUserService>();
        _accessTokenGeneratorMock = new Mock<IAccessTokenGenerator>();

        _facebookProfileServiceMock = new Mock<ISocialMediaProfileService>();
        _facebookProfileServiceMock.SetupGet(x => x.Name).Returns("Facebook");

        _googleProfileServiceMock = new Mock<ISocialMediaProfileService>();
        _googleProfileServiceMock.SetupGet(x => x.Name).Returns("Google");

        _sut = new AuthService(
            _userServiceMock.Object,
            _accessTokenGeneratorMock.Object,
            new[]
            {
                _facebookProfileServiceMock.Object,
                _googleProfileServiceMock.Object
            },
            Mock.Of<ILogger<AuthService>>());
    }

    [Fact]
    public async Task ExternalLogin_ProviderDoesNotExist_ReturnsUnsupported()
    {
        // Act
        var response = await _sut.ExternalLogin("Twitter", _userId, _accessToken, _roleName);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsT0);
    }

    [Fact]
    public async Task ExternalLogin_ProviderExistsUserDoesNotExist_ReturnsNewUser()
    {
        // Arrange
        var userProfile = _fixture.Create<SocialMediaProfile>();
        var jwt = _fixture.Create<string>();

        _facebookProfileServiceMock
            .Setup(x => x.GetSocialMediaProfile(_userId, _accessToken, default))
            .ReturnsAsync(userProfile);

        _accessTokenGeneratorMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).ReturnsAsync(jwt);

        // Act
        var response = await _sut.ExternalLogin("Facebook", _userId, _accessToken, _roleName);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsT1);
        Assert.NotNull(response.AsT1.User);
        Assert.Equal(userProfile.Email, response.AsT1.User.Email);
        Assert.NotNull(response.AsT1.User.Profiles);
        Assert.Single(response.AsT1.User.Profiles);
        Assert.Equal(response.AsT1.User.Id, response.AsT1.User.Profiles[0].UserId);
        Assert.Equal(userProfile.Provider, response.AsT1.User.Profiles[0].Provider);
        Assert.Equal(userProfile.ProviderUserId, response.AsT1.User.Profiles[0].ProviderUserId);
        Assert.Equal(userProfile.Name, response.AsT1.User.Profiles[0].Name);
        Assert.Equal(userProfile.Email, response.AsT1.User.Profiles[0].Email);
        Assert.Equal(userProfile.ProfilePictureUrl, response.AsT1.User.Profiles[0].ProfilePictureUrl);
        Assert.Equal(jwt, response.AsT1.AccessToken);
    }

    [Fact]
    public async Task ExternalLogin_ProviderExistsUserExists_ReturnsExistingUser()
    {
        // Arrange
        var userProfile = _fixture.Create<SocialMediaProfile>();
        var user = _fixture.Create<User>();
        var jwt = _fixture.Create<string>();

        _facebookProfileServiceMock
            .Setup(x => x.GetSocialMediaProfile(_userId, _accessToken, default))
            .ReturnsAsync(userProfile);

        _userServiceMock
            .Setup(x => x.GetUserByEmail(userProfile.Email, default))
            .ReturnsAsync(user);

        _accessTokenGeneratorMock.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).ReturnsAsync(jwt);

        // Act
        var response = await _sut.ExternalLogin("Facebook", _userId, _accessToken, _roleName);

        // Assert
        Assert.NotNull(response);
        Assert.True(response.IsT1);
        Assert.NotNull(response.AsT1.User);
        Assert.Equal(user.Id, response.AsT1.User.Id);
        Assert.Equal(user.UserName, response.AsT1.User.UserName);
        Assert.Equal(user.Email, response.AsT1.User.Email);
        Assert.Equal(jwt, response.AsT1.AccessToken);
    }
}