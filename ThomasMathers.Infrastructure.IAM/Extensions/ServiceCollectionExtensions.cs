using System.Reflection;
using System.Text;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using ThomasMathers.Infrastructure.IAM.Builders;
using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Mappers;
using ThomasMathers.Infrastructure.IAM.Services;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIam(this IServiceCollection services, IConfiguration configuration) => services.AddIam(configuration.GetRequiredSection("IamSettings"));

    public static void AddIam(this IServiceCollection services, IConfigurationSection configurationSection) => services.AddIam(IamSettingsBuilder.Build(configurationSection));

    public static void AddIam(this IServiceCollection services, IamSettings iamSettings)
    {
        _ = services.AddMediatR(GetMediatorAssemblies());
        _ = services.AddLogging();

        _ = string.IsNullOrEmpty(iamSettings.ConnectionString)
            ? services.AddDbContext<DatabaseContext>(optionsBuilder =>
            {
                _ = optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
                _ = optionsBuilder.ConfigureWarnings(x => x.Ignore(InMemoryEventId.TransactionIgnoredWarning));
            })
            : services.AddDbContext<DatabaseContext>(optionsBuilder =>
            {
                _ = optionsBuilder.UseSqlServer(iamSettings.ConnectionString, options => options.MigrationsAssembly(iamSettings.MigrationsAssembly));
            });

        _ = services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = iamSettings.JwtTokenSettings.Issuer,
                    ValidAudience = iamSettings.JwtTokenSettings.Audience,
                    IssuerSigningKey =
                        new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iamSettings.JwtTokenSettings.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateIssuerSigningKey = true
                };
            });

        _ = services.AddAuthorization();

        _ = services
            .AddIdentity<User, Role>(options =>
            {
                options.User = UserOptionsMapper.Map(iamSettings.UserSettings);
                options.Password = PasswordOptionsMapper.Map(iamSettings.PasswordSettings);
            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

        _ = services.AddScoped<IAuthService, AuthService>();
        _ = services.AddScoped<IUserService, UserService>();
        _ = services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        _ = services.AddScoped(_ => iamSettings);
        _ = services.AddScoped(_ => iamSettings.UserSettings);
        _ = services.AddScoped(_ => iamSettings.PasswordSettings);
        _ = services.AddScoped(_ => iamSettings.JwtTokenSettings);
    }

    private static Assembly[] GetMediatorAssemblies() => AppDomain.CurrentDomain.GetAssemblies();
}