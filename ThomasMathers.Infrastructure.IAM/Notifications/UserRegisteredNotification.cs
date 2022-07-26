using MediatR;
using ThomasMathers.Infrastructure.IAM.Data;

namespace ThomasMathers.Infrastructure.IAM.Notifications;

public record UserRegisteredNotification : INotification
{
    public string Token { get; init; } = string.Empty;
    public User User { get; init; } = new();
}