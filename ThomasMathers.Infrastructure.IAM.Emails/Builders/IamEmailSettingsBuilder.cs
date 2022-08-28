using Microsoft.Extensions.Configuration;

using ThomasMathers.Infrastructure.IAM.Emails.Settings;

namespace ThomasMathers.Infrastructure.IAM.Emails.Builders;

public static class IamEmailSettingsBuilder
{
    public static IamEmailSettings Build(IConfigurationSection section)
    {
        var settings = new IamEmailSettings();
        section.Bind(settings);
        return settings;
    }
}
