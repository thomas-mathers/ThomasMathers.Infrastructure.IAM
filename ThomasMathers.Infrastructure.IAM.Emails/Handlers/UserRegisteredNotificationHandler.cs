using MediatR;
using Microsoft.AspNetCore.WebUtilities;
using ThomasMathers.Infrastructure.Email;
using ThomasMathers.Infrastructure.Email.Services;
using ThomasMathers.Infrastructure.IAM.Emails.Emails;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Handlers;

public class UserRegisteredNotificationHandler : INotificationHandler<UserRegisteredNotification>
{
    private readonly IEmailService _emailService;
    private readonly ConfirmEmailAddressEmailSettings _emailSettings;

    public UserRegisteredNotificationHandler(IEmailService emailService, ConfirmEmailAddressEmailSettings emailSettings)
    {
        _emailService = emailService;
        _emailSettings = emailSettings;
    }

    public Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    {
        var url = QueryHelpers.AddQueryString(_emailSettings.ConfirmEmailBaseUri, "t", notification.Token);

        var email = new TemplateEmailMessage<ConfirmEmailAddressEmail>
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

        return _emailService.SendTemplatedEmailAsync(email);
    }
}