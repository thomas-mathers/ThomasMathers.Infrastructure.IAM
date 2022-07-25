using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Services;

public interface IAccessTokenGenerator
{
    public string GenerateAccessToken(IEnumerable<Claim> user);
}

public class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly SymmetricSecurityKey _key;
    private readonly JwtTokenSettings _settings;

    public AccessTokenGenerator(JwtTokenSettings settings)
    {
        _settings = settings;
        _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Key));
    }

    public string GenerateAccessToken(IEnumerable<Claim> claims)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_settings.LifespanInDays),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}