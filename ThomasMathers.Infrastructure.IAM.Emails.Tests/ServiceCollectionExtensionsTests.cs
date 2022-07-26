using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Infrastructure.IAM.Emails.Extensions;
using ThomasMathers.Infrastructure.IAM.Emails.Mappers;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Emails.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddIamEmails_RegistersRequiredServices()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var emailSettings = new EmailSettings();

        // Act
        serviceCollection.AddIamEmails(emailSettings);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetRequiredService<IConfirmEmailAddressEmailMapper>());
        Assert.NotNull(serviceProvider.GetRequiredService<IResetPasswordEmailMapper>());
        Assert.NotNull(serviceProvider.GetRequiredService<EmailSettings>());
        Assert.NotNull(serviceProvider.GetRequiredService<ConfirmEmailAddressEmailSettings>());
        Assert.NotNull(serviceProvider.GetRequiredService<ResetPasswordEmailSettings>());
    }
}