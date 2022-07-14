using Microsoft.Extensions.Configuration;

namespace ThomasMathers.Common.IAM.Settings
{
    public class IAMSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public PasswordSettings PasswordSettings { get; set; } = new PasswordSettings();
        public JwtTokenSettings Jwt { get; set; } = new JwtTokenSettings();

        public static IAMSettings FromConfigurationSection(IConfigurationSection section)
        {
            var settings = new IAMSettings();
            section.Bind(settings);
            return settings;
        }
    }
}
