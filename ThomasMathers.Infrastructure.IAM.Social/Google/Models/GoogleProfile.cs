namespace ThomasMathers.Infrastructure.IAM.Social.Google.Models
{
    internal class GoogleProfile
    {
        public string Sub { get; init; } = string.Empty;
        public string Name { get; init; } = string.Empty;
        public string GivenName { get; init; } = string.Empty;
        public string FamilyName { get; init; } = string.Empty;
        public string Picture { get; init; } = string.Empty;
        public string Email { get; internal set; } = string.Empty;
        public string Locale { get; init; } = string.Empty;
    }
}
