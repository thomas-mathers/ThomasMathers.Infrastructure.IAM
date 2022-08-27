using System.Text.Json;
using ThomasMathers.Infrastructure.IAM.Social.Extensions;

namespace ThomasMathers.Infrastructure.IAM.Social
{
    internal class SnakeCaseNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name)
        {
            return name.ToSnakeCase();
        }
    }
}
