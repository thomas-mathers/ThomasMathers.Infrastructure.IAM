using System.Text;

using Microsoft.Extensions.Configuration;

namespace ThomasMathers.Infrastructure.IAM.Emails.Tests.Helpers;

public static class ConfigurationBuilder
{
    public static IConfiguration Build(string json)
    {
        using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var configurationBuilder = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .AddJsonStream(memoryStream).Build();
        return configurationBuilder;
    }
}