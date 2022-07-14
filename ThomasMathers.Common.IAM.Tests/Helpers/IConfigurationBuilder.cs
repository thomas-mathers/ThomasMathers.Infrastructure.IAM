using Microsoft.Extensions.Configuration;
using System.IO;
using System.Text;

namespace ThomasMathers.Common.IAM.Tests.Helpers
{
    public static class IConfigurationBuilder
    {
        public static IConfiguration Build(string json)
        {
            using var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(json));
            var configurationBuilder = new ConfigurationBuilder().AddJsonStream(memoryStream).Build();
            return configurationBuilder;
        }
    }
}
