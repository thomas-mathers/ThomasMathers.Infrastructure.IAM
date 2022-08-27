using Microsoft.AspNetCore.Identity;

namespace ThomasMathers.Infrastructure.IAM.Data
{
    public class UserLogin : IdentityUserLogin<Guid>
    {
        public DateTime Created => DateTime.UtcNow;
    }
}
