namespace ThomasMathers.Infrastructure.IAM.Social.Facebook.Models
{
    internal class FacebookProfile
    {
        public string Id { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public FacebookPicture Picture { get; init; } = new FacebookPicture();
    }
}
