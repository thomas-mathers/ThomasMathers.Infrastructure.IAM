namespace ThomasMathers.Infrastructure.IAM.Data
{
    public class SocialMediaProfile
    {
        public Guid Id { get; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderUserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string ProfilePictureUrl { get; set; } = string.Empty;
    }
}
