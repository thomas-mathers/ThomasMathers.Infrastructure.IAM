using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Social.Facebook.Models;

namespace ThomasMathers.Infrastructure.IAM.Social.Providers.Facebook.Mappers
{
    internal class FacebookProfileToSocialMediaProfileMapper
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
    }
}
