namespace ThomasMathers.Infrastructure.IAM.Settings;

public record UserSettings
{
    public string AllowedUserNameCharacters { get; init; } =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";

    public bool RequireUniqueEmail { get; init; } = true;
}