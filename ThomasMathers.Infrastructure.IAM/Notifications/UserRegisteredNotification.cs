using MediatR;

using ThomasMathers.Infrastructure.IAM.Data.EF;

namespace ThomasMathers.Infrastructure.IAM.Notifications;

public class UserRegisteredNotification : INotification
{
    public User User { get; init; } = new();
    public string Token { get; init; } = string.Empty;
}