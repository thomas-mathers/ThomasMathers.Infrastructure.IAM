using MediatR;
using ThomasMathers.Common.IAM.Data;

namespace ThomasMathers.Common.IAM.Notifications
{
    public class ResetPasswordNotification : INotification
    {
        public User User { get; init; }
        public string Token { get; init; }
    }
}
