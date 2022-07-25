using Microsoft.AspNetCore.Identity;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Mappers;

public static class PasswordSettingsMapper
{
    public static PasswordOptions Map(PasswordSettings source)
    {
        return new PasswordOptions
        {
            RequireDigit = source.RequireDigit,
            RequiredLength = source.RequiredLength,
            RequiredUniqueChars = source.RequiredUniqueChars,
            RequireLowercase = source.RequireLowercase,
            RequireNonAlphanumeric = source.RequireNonAlphanumeric,
            RequireUppercase = source.RequireUppercase
        };
    }
}