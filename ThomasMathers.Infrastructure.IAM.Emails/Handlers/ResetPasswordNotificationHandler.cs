using MediatR;
using ThomasMathers.Infrastructure.Email.Services;
using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Handlers;

public class ResetPasswordNotificationHandler : INotificationHandler<ResetPasswordNotification>
{
    private readonly IResetPasswordEmailBuilder _emailBuilder;
    private readonly IEmailService _emailService;

    public ResetPasswordNotificationHandler(IEmailService emailService, IResetPasswordEmailBuilder emailBuilder)
    {
        _emailService = emailService;
        _emailBuilder = emailBuilder;
    }

    public Task Handle(ResetPasswordNotification notification, CancellationToken cancellationToken)
    {
        var email = _emailBuilder.Build(notification);
        return _emailService.SendTemplatedEmailAsync(email);
    }
}