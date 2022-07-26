using MediatR;
using ThomasMathers.Infrastructure.Email.Services;
using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Handlers;

public class UserRegisteredNotificationHandler : INotificationHandler<UserRegisteredNotification>
{
    private readonly IConfirmEmailAddressEmailBuilder _emailBuilder;
    private readonly IEmailService _emailService;

    public UserRegisteredNotificationHandler(IEmailService emailService, IConfirmEmailAddressEmailBuilder emailSettings)
    {
        _emailService = emailService;
        _emailBuilder = emailSettings;
    }

    public Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    {
        var email = _emailBuilder.Build(notification);
        return _emailService.SendTemplatedEmailAsync(email);
    }
}