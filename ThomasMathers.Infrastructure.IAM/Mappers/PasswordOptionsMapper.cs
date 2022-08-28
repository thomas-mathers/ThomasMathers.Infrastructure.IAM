using Microsoft.AspNetCore.Identity;

using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Mappers;

public static class PasswordOptionsMapper
{
    public static PasswordOptions Map(PasswordSettings source) => new()
    {
        RequireDigit = source.RequireDigit,
        RequiredLength = source.RequiredLength,
        RequiredUniqueChars = source.RequiredUniqueChars,
        RequireLowercase = source.RequireLowercase,
        RequireNonAlphanumeric = source.RequireNonAlphanumeric,
        RequireUppercase = source.RequireUppercase
    };
}