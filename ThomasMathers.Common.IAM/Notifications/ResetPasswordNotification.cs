using MediatR;
using ThomasMathers.Infrastructure.IAM.Data;

namespace ThomasMathers.Infrastructure.IAM.Notifications
{
    public class UserRegisteredNotification : INotification
    {
        public User User { get; init; }
    }
}
