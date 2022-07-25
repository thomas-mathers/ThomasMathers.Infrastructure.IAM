using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using ThomasMathers.Infrastructure.IAM.Services;
using ThomasMathers.Infrastructure.IAM.Settings;
using ThomasMathers.Infrastructure.IAM.Tests.Comparers;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class AccessTokenGeneratorTests
{
    private readonly string _audience = "share-my-calendar";
    private readonly string _issuer = "share-my-calendar";
    private readonly string _key = "super-secret-key-12345678";
    private readonly AccessTokenGenerator _sut;

    public AccessTokenGeneratorTests()
    {
        var options = new JwtTokenSettings
        {
            Key = _key,
            Audience = _audience,
            Issuer = _issuer,
            LifespanInDays = 1
        };
        _sut = new AccessTokenGenerator(options);
    }

    [Fact]
    public void GenerateAccessToken_NoClaims_EncodesCorrectly()
    {
        // Arrange
        var claims = Array.Empty<Claim>();

        // Act
        var encodedToken = _sut.GenerateAccessToken(claims);
        var decodedToken = DecodeToken(encodedToken);

        // Assert
        Assert.Equal(_issuer, decodedToken.Issuer);
        Assert.True(decodedToken.Audiences.Count() == 1);
        Assert.Equal(_audience, decodedToken.Audiences.First());
    }

    [Fact]
    public void GenerateAccessToken_Claims_EncodesCorrectly()
    {
        // Arrange
        var claims = new[]
        {
            new Claim("Id", "8dba1acd-1c48-436e-a1c3-836656251c3a"),
            new Claim("Name", "Thomas Mathers"),
            new Claim("Role", "SuperAdmin")
        };

        // Act
        var encodedToken = _sut.GenerateAccessToken(claims);
        var decodedToken = DecodeToken(encodedToken);

        // Assert
        Assert.Equal(_issuer, decodedToken.Issuer);
        Assert.True(decodedToken.Audiences.Count() == 1);
        Assert.Equal(_audience, decodedToken.Audiences.First());
        Assert.Contains(claims[0], decodedToken.Claims, new ClaimComparer());
        Assert.Contains(claims[1], decodedToken.Claims, new ClaimComparer());
        Assert.Contains(claims[2], decodedToken.Claims, new ClaimComparer());
    }

    private static JwtSecurityToken DecodeToken(string encodedToken)
    {
        return new JwtSecurityTokenHandler().ReadJwtToken(encodedToken);
    }
}