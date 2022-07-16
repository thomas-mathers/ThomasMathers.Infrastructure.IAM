namespace ThomasMathers.Infrastructure.IAM.Settings
{
    public record UserSettings
    {
        public bool RequireUniqueEmail { get; init; } = true;
        public string AllowedUserNameCharacters { get; init; } = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    }
}