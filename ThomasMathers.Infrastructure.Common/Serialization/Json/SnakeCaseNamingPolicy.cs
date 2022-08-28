using System.Text.Json;
using ThomasMathers.Infrastructure.Common.Extensions;

namespace ThomasMathers.Infrastructure.Common.Serialization.Json;

public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    public override string ConvertName(string name)
    {
        return name.ToSnakeCase();
    }
}