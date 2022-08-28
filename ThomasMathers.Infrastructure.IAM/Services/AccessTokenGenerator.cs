using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Services;

public interface IAccessTokenGenerator
{
    public Task<string> GenerateAccessToken(User user);
}

public class AccessTokenGenerator : IAccessTokenGenerator
{
    private readonly SymmetricSecurityKey _key;
    private readonly JwtTokenSettings _settings;
    private readonly UserManager<User> _userManager;

    public AccessTokenGenerator(UserManager<User> userManager, JwtTokenSettings settings)
    {
        _userManager = userManager;
        _settings = settings;
        _key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_settings.Key));
    }

    public async Task<string> GenerateAccessToken(User user)
    {
        var userRoles = await _userManager.GetRolesAsync(user); 

        var claims = new List<Claim>
        {
            new Claim("iss", _settings.Issuer),
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        };

        if (!string.IsNullOrEmpty(user.Email))
        {
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
        }

        if (!string.IsNullOrEmpty(user.PhoneNumber))
        {
            claims.Add(new Claim(ClaimTypes.MobilePhone, user.PhoneNumber));
        }

        foreach (var role in userRoles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(_settings.LifespanInDays),
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            SigningCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}