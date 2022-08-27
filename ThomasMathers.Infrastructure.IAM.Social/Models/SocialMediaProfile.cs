namespace ThomasMathers.Infrastructure.IAM.Social.Models
{
    public class SocialMediaProfile
    {
        public string Provider { get; init; } = string.Empty;
        public string ProviderUserId { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string ProfilePictureUrl { get; init; } = string.Empty;
    }
}
