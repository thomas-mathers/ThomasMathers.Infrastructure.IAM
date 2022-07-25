using Microsoft.Extensions.Configuration;

namespace ThomasMathers.Infrastructure.IAM.Settings;

public record IAMSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public JwtTokenSettings JwtTokenSettings { get; init; } = new();
    public PasswordSettings PasswordSettings { get; init; } = new();
    public UserSettings UserSettings { get; init; } = new();

    public static IAMSettings FromConfigurationSection(IConfigurationSection section)
    {
        var settings = new IAMSettings();
        section.Bind(settings);
        return settings;
    }
}