namespace ThomasMathers.Infrastructure.IAM.Settings
{
    public record PasswordSettings
    {
        public bool RequireDigit { get; init; } = true;
        public int RequiredLength { get; init; } = 6;
        public int RequiredUniqueChars { get; init; } = 1;
        public bool RequireLowercase { get; init; } = true;
        public bool RequireNonAlphanumeric { get; init; } = true;
        public bool RequireUppercase { get; init; } = true;
    }
}
