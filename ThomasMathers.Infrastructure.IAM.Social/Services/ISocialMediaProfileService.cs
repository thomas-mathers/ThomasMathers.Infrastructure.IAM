using ThomasMathers.Infrastructure.IAM.Social.Models;

namespace ThomasMathers.Infrastructure.IAM.Social.Services
{
    public interface ISocialMediaProfileService
    {
        public string Name { get; }
        public Task<SocialMediaProfile> GetSocialMediaProfile(string userId, string accessToken, CancellationToken cancellationToken = default);
    }
}
