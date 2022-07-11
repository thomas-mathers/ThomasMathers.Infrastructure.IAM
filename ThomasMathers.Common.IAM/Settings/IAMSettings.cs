using Microsoft.Extensions.Configuration;

namespace ThomasMathers.Common.IAM.Settings
{
    public class IAMSettings
    {
        public string ConnectionString { get; set; }
        public string SendGridKey { get; set; }
        public JwtTokenSettings Jwt { get; set; }

        public static IAMSettings FromConfigurationSection(IConfigurationSection section)
        {
            var settings = new IAMSettings();
            section.Bind(settings);
            return settings;
        }
    }
}
