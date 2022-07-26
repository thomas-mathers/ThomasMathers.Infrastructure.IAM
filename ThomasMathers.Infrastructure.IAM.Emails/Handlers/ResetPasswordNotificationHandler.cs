using MediatR;
using ThomasMathers.Infrastructure.Email.Services;
using ThomasMathers.Infrastructure.IAM.Emails.Mappers;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Handlers;

public class ResetPasswordNotificationHandler : INotificationHandler<ResetPasswordNotification>
{
    private readonly IResetPasswordEmailMapper _emailMapper;
    private readonly IEmailService _emailService;

    public ResetPasswordNotificationHandler(IEmailService emailService, IResetPasswordEmailMapper emailMapper)
    {
        _emailService = emailService;
        _emailMapper = emailMapper;
    }

    public Task Handle(ResetPasswordNotification notification, CancellationToken cancellationToken)
    {
        var email = _emailMapper.Map(notification);
        return _emailService.SendTemplatedEmailAsync(email);
    }
}