using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Infrastructure.IAM.Emails.Mappers;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;

namespace ThomasMathers.Infrastructure.IAM.Emails.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddIamEmails(this IServiceCollection serviceCollection, EmailSettings emailSettings)
    {
        serviceCollection.AddScoped<IConfirmEmailAddressEmailMapper, ConfirmEmailAddressEmailMapper>();
        serviceCollection.AddScoped<IResetPasswordEmailMapper, ResetPasswordEmailMapper>();
        serviceCollection.AddScoped(_ => emailSettings);
        serviceCollection.AddScoped(_ => emailSettings.ConfirmEmailAddressEmailSettings);
        serviceCollection.AddScoped(_ => emailSettings.ResetPasswordEmailSettings);
    }
}