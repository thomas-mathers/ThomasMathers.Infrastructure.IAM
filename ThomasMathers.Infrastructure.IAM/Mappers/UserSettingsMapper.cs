using Microsoft.AspNetCore.Identity;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Mappers;

public class UserSettingsMapper
{
    public static UserOptions Map(UserSettings source)
    {
        return new UserOptions
        {
            RequireUniqueEmail = source.RequireUniqueEmail,
            AllowedUserNameCharacters = source.AllowedUserNameCharacters
        };
    }
}