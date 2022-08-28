using ThomasMathers.Infrastructure.IAM.Data.EF;

namespace ThomasMathers.Infrastructure.IAM.Social
{
    public interface ISocialMediaProfileService
    {
        public string Name { get; }
        public Task<SocialMediaProfile> GetSocialMediaProfile(string userId, string accessToken, CancellationToken cancellationToken = default);
    }
}
