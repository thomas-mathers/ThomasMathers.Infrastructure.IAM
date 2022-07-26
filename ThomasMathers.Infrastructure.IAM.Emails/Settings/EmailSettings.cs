namespace ThomasMathers.Infrastructure.IAM.Emails.Settings;

public record EmailSettings
{
    public ConfirmEmailAddressEmailSettings ConfirmEmailAddressEmailSettings { get; init; } = new();
    public ResetPasswordEmailSettings ResetPasswordEmailSettings { get; init; } = new();
}