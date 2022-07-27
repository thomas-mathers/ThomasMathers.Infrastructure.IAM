using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;

namespace ThomasMathers.Infrastructure.IAM.Emails.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIamEmails(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddIamEmails(configuration.GetSection("IamEmailSettings"));
    }

    public static void AddIamEmails(this IServiceCollection serviceCollection, IConfigurationSection section)
    {
        serviceCollection.AddIamEmails(IamEmailSettingsBuilder.Build(section));
    }

    public static void AddIamEmails(this IServiceCollection serviceCollection, IamEmailSettings iamEmailSettings)
    {
        serviceCollection.AddLogging();
        serviceCollection.AddScoped<IConfirmEmailAddressEmailBuilder, ConfirmEmailAddressEmailBuilder>();
        serviceCollection.AddScoped<IResetPasswordEmailBuilder, ResetPasswordEmailBuilder>();
        serviceCollection.AddScoped(_ => iamEmailSettings);
        serviceCollection.AddScoped(_ => iamEmailSettings.ConfirmEmailAddressEmailSettings);
        serviceCollection.AddScoped(_ => iamEmailSettings.ResetPasswordEmailSettings);
    }
}