using MediatR;
using ThomasMathers.Infrastructure.Email.Services;
using ThomasMathers.Infrastructure.IAM.Emails.Mappers;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Handlers;

public class UserRegisteredNotificationHandler : INotificationHandler<UserRegisteredNotification>
{
    private readonly IConfirmEmailAddressEmailMapper _emailMapper;
    private readonly IEmailService _emailService;

    public UserRegisteredNotificationHandler(IEmailService emailService, IConfirmEmailAddressEmailMapper emailSettings)
    {
        _emailService = emailService;
        _emailMapper = emailSettings;
    }

    public Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    {
        var email = _emailMapper.Map(notification);
        return _emailService.SendTemplatedEmailAsync(email);
    }
}