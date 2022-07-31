using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ThomasMathers.Infrastructure.IAM.Data;

public class DatabaseContext : IdentityDbContext<User, Role, Guid>
{
    public DatabaseContext()
    {

    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }
}