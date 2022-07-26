namespace ThomasMathers.Infrastructure.IAM.Emails.Settings;

public record IamEmailSettings
{
    public ConfirmEmailAddressEmailSettings ConfirmEmailAddressEmailSettings { get; init; } = new();
    public ResetPasswordEmailSettings ResetPasswordEmailSettings { get; init; } = new();
}