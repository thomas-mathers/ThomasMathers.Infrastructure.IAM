namespace ThomasMathers.Common.IAM.Settings
{
    public record JwtTokenSettings
    {
        public string Key { get; init; }
        public string Issuer { get; init; }
        public string Audience { get; init; }
        public int LifespanInDays { get; init; }
    }
}
