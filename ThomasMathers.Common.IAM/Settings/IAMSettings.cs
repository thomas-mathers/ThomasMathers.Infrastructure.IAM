using Microsoft.Extensions.Configuration;

namespace ThomasMathers.Common.IAM.Settings
{
    public record IAMSettings
    {
        public string ConnectionString { get; init; } = string.Empty;
        public UserSettings UserSettings { get; init; } = new UserSettings();
        public PasswordSettings PasswordSettings { get; init; } = new PasswordSettings();
        public JwtTokenSettings JwtTokenSettings { get; init; } = new JwtTokenSettings();

        public static IAMSettings FromConfigurationSection(IConfigurationSection section)
        {
            var settings = new IAMSettings();
            section.Bind(settings);
            return settings;
        }
    }
}
