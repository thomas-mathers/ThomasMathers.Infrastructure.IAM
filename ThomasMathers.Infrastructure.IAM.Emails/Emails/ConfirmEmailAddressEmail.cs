namespace ThomasMathers.Infrastructure.IAM.Emails.Emails;

public record ConfirmEmailAddressEmail
{
    public string Link { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
}