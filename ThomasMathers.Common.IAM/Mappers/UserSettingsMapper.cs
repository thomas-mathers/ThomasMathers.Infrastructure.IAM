using Microsoft.AspNetCore.Identity;
using ThomasMathers.Common.IAM.Settings;

namespace ThomasMathers.Common.IAM.Mappers
{
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
}
