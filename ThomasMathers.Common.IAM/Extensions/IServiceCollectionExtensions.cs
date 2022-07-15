using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ThomasMathers.Common.IAM.Data;
using ThomasMathers.Common.IAM.Services;
using ThomasMathers.Common.IAM.Settings;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ThomasMathers.Common.IAM.Mappers;

namespace ThomasMathers.Common.IAM.Extensions
{
    public static class IServiceCollectionExtensions
    {
        public static void AddIAM(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIAM(configuration.GetRequiredSection("IAMSettings"));
        }

        public static void AddIAM(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            services.AddIAM(IAMSettings.FromConfigurationSection(configurationSection));
        }

        public static void AddIAM(this IServiceCollection services, IAMSettings iamSettings)
        {
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
                    optionsBuilder.UseSqlServer(iamSettings.ConnectionString);
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(iamSettings.JwtTokenSettings.Key)),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateIssuerSigningKey = true,
                    };
                });

            services.AddAuthorization();

            services
                .AddIdentity<User, Role>(options =>
                {
                    options.User = UserSettingsMapper.Map(iamSettings.UserSettings);
                    options.Password = PasswordSettingsMapper.Map(iamSettings.PasswordSettings);
                })
                .AddEntityFrameworkStores<DatabaseContext>()
                .AddDefaultTokenProviders();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAccessTokenGenerator, AccessTokenGenerator>();
            services.AddScoped(serviceProvider => iamSettings);
            services.AddScoped(serviceProvider => iamSettings.UserSettings);
            services.AddScoped(serviceProvider => iamSettings.PasswordSettings);
            services.AddScoped(serviceProvider => iamSettings.JwtTokenSettings);
        }
    }
}
