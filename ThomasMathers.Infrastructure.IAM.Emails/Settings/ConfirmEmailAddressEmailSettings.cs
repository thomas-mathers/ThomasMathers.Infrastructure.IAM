using ThomasMathers.Infrastructure.Email;

namespace ThomasMathers.Infrastructure.IAM.Emails.Settings;

public record ConfirmEmailAddressEmailSettings
{
    public string ConfirmEmailBaseUri { get; init; } = string.Empty;
    public EmailAddress From { get; init; } = new();
    public string TemplateId { get; init; } = string.Empty;
}