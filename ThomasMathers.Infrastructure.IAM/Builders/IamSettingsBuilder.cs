using Microsoft.Extensions.Configuration;

using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Builders;

public static class IamSettingsBuilder
{
    public static IamSettings Build(IConfigurationSection section)
    {
        var settings = new IamSettings();
        section.Bind(settings);
        return settings;
    }
}
