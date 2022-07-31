namespace ThomasMathers.Infrastructure.IAM.Settings;

public record IamSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public string MigrationsAssembly { get; init; } = string.Empty;
    public JwtTokenSettings JwtTokenSettings { get; init; } = new();
    public PasswordSettings PasswordSettings { get; init; } = new();
    public UserSettings UserSettings { get; init; } = new();
}