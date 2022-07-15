using MediatR;
using ThomasMathers.Common.IAM.Data;

namespace ThomasMathers.Common.IAM.Notifications
{
    public class UserRegisteredNotification : INotification
    {
        public User User { get; init; }
    }
}
