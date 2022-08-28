using MediatR;

using Microsoft.Extensions.Logging;

using ThomasMathers.Infrastructure.Email.Services;
using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Handlers;

public class ResetPasswordNotificationHandler : INotificationHandler<ResetPasswordNotification>
{
    private readonly IResetPasswordEmailBuilder _emailBuilder;
    private readonly IEmailService _emailService;
    private readonly ILogger<ResetPasswordNotificationHandler> _logger;

    public ResetPasswordNotificationHandler(IEmailService emailService, IResetPasswordEmailBuilder emailBuilder, ILogger<ResetPasswordNotificationHandler> logger)
    {
        _emailService = emailService;
        _emailBuilder = emailBuilder;
        _logger = logger;
    }

    public async Task Handle(ResetPasswordNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing reset password notification");
        var email = _emailBuilder.Build(notification);
        await _emailService.SendTemplatedEmailAsync(email);
        _logger.LogInformation("Processed reset password notification");
    }
}