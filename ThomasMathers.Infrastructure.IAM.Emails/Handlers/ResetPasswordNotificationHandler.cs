using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using ThomasMathers.Infrastructure.Email;
using ThomasMathers.Infrastructure.Email.Services;
using ThomasMathers.Infrastructure.IAM.Emails.Emails;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Handlers;

public class ResetPasswordNotificationHandler : INotificationHandler<ResetPasswordNotification>
{
    private readonly IEmailService _emailService;
    private readonly ResetPasswordEmailSettings _emailSettings;

    public ResetPasswordNotificationHandler(IEmailService emailService, ResetPasswordEmailSettings emailSettings)
    {
        _emailService = emailService;
        _emailSettings = emailSettings;
    }

    public Task Handle(ResetPasswordNotification notification, CancellationToken cancellationToken)
    {
        var url = QueryHelpers.AddQueryString(_emailSettings.ChangePasswordBaseUri, "t", notification.Token);

        var email = new TemplateEmailMessage<ResetPasswordEmail>
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

        return _emailService.SendTemplatedEmailAsync(email);
    }
}