using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;

namespace ThomasMathers.Infrastructure.IAM.Emails.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIamEmails(this IServiceCollection serviceCollection, IConfiguration configuration) => serviceCollection.AddIamEmails(configuration.GetSection("IamEmailSettings"));

    public static void AddIamEmails(this IServiceCollection serviceCollection, IConfigurationSection section) => serviceCollection.AddIamEmails(IamEmailSettingsBuilder.Build(section));

    public static void AddIamEmails(this IServiceCollection serviceCollection, IamEmailSettings iamEmailSettings)
    {
        _ = serviceCollection.AddLogging();
        _ = serviceCollection.AddScoped<IConfirmEmailAddressEmailBuilder, ConfirmEmailAddressEmailBuilder>();
        _ = serviceCollection.AddScoped<IResetPasswordEmailBuilder, ResetPasswordEmailBuilder>();
        _ = serviceCollection.AddScoped(_ => iamEmailSettings);
        _ = serviceCollection.AddScoped(_ => iamEmailSettings.ConfirmEmailAddressEmailSettings);
        _ = serviceCollection.AddScoped(_ => iamEmailSettings.ResetPasswordEmailSettings);
    }
}