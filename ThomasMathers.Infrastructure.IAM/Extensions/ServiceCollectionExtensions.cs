using System.Reflection;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ThomasMathers.Infrastructure.IAM.Builders;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Mappers;
using ThomasMathers.Infrastructure.IAM.Services;
using ThomasMathers.Infrastructure.IAM.Settings;

namespace ThomasMathers.Infrastructure.IAM.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIam(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddIam(configuration.GetRequiredSection("IamSettings"));
    }

    public static void AddIam(this IServiceCollection services, IConfigurationSection configurationSection)
    {
        services.AddIam(IamSettingsBuilder.Build(configurationSection));
    }

    public static void AddIam(this IServiceCollection services, IamSettings iamSettings)
    {
        services.AddMediatR(GetMediatorAssemblies());
        services.AddLogging();

        if (string.IsNullOrEmpty(iamSettings.ConnectionString))
        {
            services.AddDbContext<DatabaseContext>(optionsBuilder =>
            {
                optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
            });
        }
        else
        {
            services.AddDbContext<DatabaseContext>(optionsBuilder =>
            {
                optionsBuilder.UseSqlServer(iamSettings.ConnectionString, options => options.MigrationsAssembly(iamSettings.MigrationsAssembly));
            });
        }

        services
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

        services.AddAuthorization();

        services
            .AddIdentity<User, Role>(options =>
            {
                options.User = UserOptionsMapper.Map(iamSettings.UserSettings);
                options.Password = PasswordOptionsMapper.Map(iamSettings.PasswordSettings);
            })
            .AddEntityFrameworkStores<DatabaseContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
        services.AddScoped(_ => iamSettings);
        services.AddScoped(_ => iamSettings.UserSettings);
        services.AddScoped(_ => iamSettings.PasswordSettings);
        services.AddScoped(_ => iamSettings.JwtTokenSettings);
    }

    private static Assembly[] GetMediatorAssemblies()
    {
        return AppDomain.CurrentDomain.GetAssemblies();
    }
}