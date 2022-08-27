using ThomasMathers.Infrastructure.IAM.Social.Facebook.Models;
using ThomasMathers.Infrastructure.IAM.Social.Google.Models;
using ThomasMathers.Infrastructure.IAM.Social.Models;

namespace ThomasMathers.Infrastructure.IAM.Social.Mappers
{
    internal class SocialMediaProfileMapper
    {
        public static SocialMediaProfile Map(FacebookProfile facebookProfile) 
        {
            return new SocialMediaProfile
            {
                Provider = "Facebook",
                ProviderUserId = facebookProfile.Id,
                Name = facebookProfile.Name,
                Email = facebookProfile.Email,
                ProfilePictureUrl = facebookProfile.Picture.Data.Url
            };
        }

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
