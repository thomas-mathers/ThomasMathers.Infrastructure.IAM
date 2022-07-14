namespace ThomasMathers.Common.IAM.Settings
{
    public record JwtTokenSettings
    {
        public string Key { get; init; } = Guid.NewGuid().ToString();
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int LifespanInDays { get; init; } = 1;
    }
}
