using MediatR;
using ThomasMathers.Infrastructure.IAM.Data;

namespace ThomasMathers.Infrastructure.IAM.Notifications;

public class ResetPasswordNotification : INotification
{
    public User User { get; init; } = new();
    public string Token { get; init; } = string.Empty;
}