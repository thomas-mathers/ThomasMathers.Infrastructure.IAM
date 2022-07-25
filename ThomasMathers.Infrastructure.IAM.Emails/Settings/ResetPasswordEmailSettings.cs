using ThomasMathers.Infrastructure.Email;

namespace ThomasMathers.Infrastructure.IAM.Emails.Settings;

public record ResetPasswordEmailSettings
{
    public string ChangePasswordBaseUri { get; init; } = string.Empty;
    public EmailAddress From { get; init; } = new();
    public string TemplateId { get; init; } = string.Empty;
}