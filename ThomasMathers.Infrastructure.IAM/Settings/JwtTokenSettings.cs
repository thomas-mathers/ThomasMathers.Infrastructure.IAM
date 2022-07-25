namespace ThomasMathers.Infrastructure.IAM.Settings;

public record JwtTokenSettings
{
    public string Audience { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Key { get; init; } = Guid.NewGuid().ToString();
    public int LifespanInDays { get; init; } = 1;
}