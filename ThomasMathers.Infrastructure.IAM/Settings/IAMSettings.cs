using Microsoft.Extensions.Configuration;

namespace ThomasMathers.Infrastructure.IAM.Settings;

public record IamSettings
{
    public string ConnectionString { get; init; } = string.Empty;
    public JwtTokenSettings JwtTokenSettings { get; init; } = new();
    public PasswordSettings PasswordSettings { get; init; } = new();
    public UserSettings UserSettings { get; init; } = new();

    public static IamSettings FromConfigurationSection(IConfigurationSection section)
    {
        var settings = new IamSettings();
        section.Bind(settings);
        return settings;
    }
}