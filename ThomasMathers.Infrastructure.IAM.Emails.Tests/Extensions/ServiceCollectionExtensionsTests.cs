using Microsoft.Extensions.DependencyInjection;

using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Emails.Extensions;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;

using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Emails.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddIamEmails_RegistersRequiredServices()
    {
        // Arrange
        var serviceCollection = new ServiceCollection();
        var emailSettings = new IamEmailSettings();

        // Act
        serviceCollection.AddIamEmails(emailSettings);
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Assert
        Assert.NotNull(serviceProvider.GetRequiredService<IConfirmEmailAddressEmailBuilder>());
        Assert.NotNull(serviceProvider.GetRequiredService<IResetPasswordEmailBuilder>());
        Assert.NotNull(serviceProvider.GetRequiredService<IamEmailSettings>());
        Assert.NotNull(serviceProvider.GetRequiredService<ConfirmEmailAddressEmailSettings>());
        Assert.NotNull(serviceProvider.GetRequiredService<ResetPasswordEmailSettings>());
    }
}