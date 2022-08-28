using Microsoft.AspNetCore.WebUtilities;

using ThomasMathers.Infrastructure.Email;
using ThomasMathers.Infrastructure.IAM.Emails.Emails;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Builders;

public interface IConfirmEmailAddressEmailBuilder
{
    public TemplateEmailMessage<ConfirmEmailAddressEmail> Build(UserRegisteredNotification notification);
}

public class ConfirmEmailAddressEmailBuilder : IConfirmEmailAddressEmailBuilder
{
    private readonly ConfirmEmailAddressEmailSettings _emailSettings;

    public ConfirmEmailAddressEmailBuilder(ConfirmEmailAddressEmailSettings emailSettings) => _emailSettings = emailSettings;

    public TemplateEmailMessage<ConfirmEmailAddressEmail> Build(UserRegisteredNotification notification)
    {
        var url = QueryHelpers.AddQueryString(_emailSettings.ConfirmEmailBaseUri, "t", notification.Token);

        return new TemplateEmailMessage<ConfirmEmailAddressEmail>
        {
            From = _emailSettings.From,
            To = new EmailAddress(notification.User.Email, notification.User.UserName),
            TemplateId = _emailSettings.TemplateId,
            Payload = new ConfirmEmailAddressEmail
            {
                Username = notification.User.UserName,
                Link = url
            }
        };
    }
}