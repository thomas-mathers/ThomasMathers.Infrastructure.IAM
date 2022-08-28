using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ThomasMathers.Infrastructure.IAM.Data.EF;

public class DatabaseContext : IdentityDbContext<User, Role, Guid, IdentityUserClaim<Guid>, IdentityUserRole<Guid>, UserLogin, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>
{
    public DatabaseContext()
    {

    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        _ = builder.Entity<SocialMediaProfile>().HasKey(x => x.Id);
    }

    public DbSet<SocialMediaProfile> SocialMediaProfiles { get; set; } = null!;
}