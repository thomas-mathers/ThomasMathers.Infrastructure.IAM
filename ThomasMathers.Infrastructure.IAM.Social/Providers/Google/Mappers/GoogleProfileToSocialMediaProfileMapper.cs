using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Social.Google.Models;

namespace ThomasMathers.Infrastructure.IAM.Social.Providers.Google.Mappers
{
    internal class GoogleProfileToSocialMediaProfileMapper
    {
        public static SocialMediaProfile Map(GoogleProfile googleProfile)
        {
            return new SocialMediaProfile
            {
                Provider = "Google",
                ProviderUserId = googleProfile.Sub,
                Name = googleProfile.Name,
                Email = googleProfile.Email,
                ProfilePictureUrl = googleProfile.Picture
            };
        }
    }
}
