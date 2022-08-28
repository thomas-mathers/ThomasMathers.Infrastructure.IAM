using Microsoft.AspNetCore.Identity;

using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Mappers;

public class UserOptionsMapper
{
    public static UserOptions Map(UserSettings source) => new()
    {
        RequireUniqueEmail = source.RequireUniqueEmail,
        AllowedUserNameCharacters = source.AllowedUserNameCharacters
    };
}