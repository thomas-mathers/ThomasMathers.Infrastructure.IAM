using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Social.Providers.Google.Models;

namespace ThomasMathers.Infrastructure.IAM.Social.Providers.Google.Mappers;

internal class GoogleProfileToSocialMediaProfileMapper
{
    public static SocialMediaProfile Map(GoogleProfile googleProfile) => new()
    {
        Provider = "Google",
        ProviderUserId = googleProfile.Sub,
        Name = googleProfile.Name,
        Email = googleProfile.Email,
        ProfilePictureUrl = googleProfile.Picture
    };
}
