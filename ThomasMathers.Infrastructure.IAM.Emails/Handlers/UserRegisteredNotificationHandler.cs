using MediatR;

using Microsoft.Extensions.Logging;

using ThomasMathers.Infrastructure.Email.Services;
using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Notifications;

namespace ThomasMathers.Infrastructure.IAM.Emails.Handlers;

public class UserRegisteredNotificationHandler : INotificationHandler<UserRegisteredNotification>
{
    private readonly IConfirmEmailAddressEmailBuilder _emailBuilder;
    private readonly IEmailService _emailService;
    private readonly ILogger<UserRegisteredNotificationHandler> _logger;

    public UserRegisteredNotificationHandler
    (
        IEmailService emailService,
        IConfirmEmailAddressEmailBuilder emailSettings,
        ILogger<UserRegisteredNotificationHandler> logger
    )
    {
        _emailService = emailService;
        _emailBuilder = emailSettings;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredNotification notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing user registered notification");
        var email = _emailBuilder.Build(notification);
        await _emailService.SendTemplatedEmailAsync(email);
        _logger.LogInformation("Processed user registered notification");
    }
}