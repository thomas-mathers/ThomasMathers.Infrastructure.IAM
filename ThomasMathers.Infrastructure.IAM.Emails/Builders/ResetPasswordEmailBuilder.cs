using Microsoft.AspNetCore.WebUtilities;

using ThomasMathers.Infrastructure.Email;
using ThomasMathers.Infrastructure.IAM.Emails.Emails;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Builders;

public interface IResetPasswordEmailBuilder
{
    public TemplateEmailMessage<ResetPasswordEmail> Build(ResetPasswordNotification notification);
}

public class ResetPasswordEmailBuilder : IResetPasswordEmailBuilder
{
    private readonly ResetPasswordEmailSettings _emailSettings;

    public ResetPasswordEmailBuilder(ResetPasswordEmailSettings emailSettings) => _emailSettings = emailSettings;

    public TemplateEmailMessage<ResetPasswordEmail> Build(ResetPasswordNotification notification)
    {
        var url = QueryHelpers.AddQueryString(_emailSettings.ChangePasswordBaseUri, "t", notification.Token);

        return new TemplateEmailMessage<ResetPasswordEmail>
        {
            From = _emailSettings.From,
            To = new EmailAddress(notification.User.Email, notification.User.UserName),
            TemplateId = _emailSettings.TemplateId,
            Payload = new ResetPasswordEmail
            {
                Username = notification.User.UserName,
                Link = url
            }
        };
    }
}