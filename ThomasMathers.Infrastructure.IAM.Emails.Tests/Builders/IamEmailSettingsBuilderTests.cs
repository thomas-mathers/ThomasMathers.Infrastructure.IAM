using System.Text.Json;

using ThomasMathers.Infrastructure.Email;
using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using ThomasMathers.Infrastructure.IAM.Emails.Tests.Helpers;

using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Emails.Tests.Builders;

public class IamEmailSettingsBuilderTests
{
    [Theory]
    [InlineData("{}")]
    [InlineData("{\"IamEmailSettings\": {}}")]
    public void Build_NoOverridesSpecified_ReturnsCorrectDefaults(string configJson)
    {
        var configuration = ConfigurationBuilder.Build(configJson);

        // Act
        var actual = IamEmailSettingsBuilder.Build(configuration.GetSection("IamEmailSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.ConfirmEmailAddressEmailSettings);
        Assert.NotNull(actual.ResetPasswordEmailSettings);
    }

    [Theory]
    [InlineData("", "", "", "")]
    [InlineData("tmathers@some-site.com", "tmathers", "www.some-site.com/confirm-email", "1")]
    public void Build_ConfirmEmailAddressEmailSettingsOverrides_ReturnsOverrides(string fromEmail, string fromName, string confirmEmailBaseUri, string templateId)
    {
        var json = JsonSerializer.Serialize(new
        {
            IamEmailSettings = new IamEmailSettings()
            {
                ConfirmEmailAddressEmailSettings = new()
                {
                    From = new EmailAddress
                    {
                        Email = fromEmail,
                        Name = fromName,
                    },
                    ConfirmEmailBaseUri = confirmEmailBaseUri,
                    TemplateId = templateId
                }
            }
        });
        var configuration = ConfigurationBuilder.Build(json);

        // Act
        var actual = IamEmailSettingsBuilder.Build(configuration.GetSection("IamEmailSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.ConfirmEmailAddressEmailSettings);
        Assert.NotNull(actual.ConfirmEmailAddressEmailSettings.From);
        Assert.Equal(fromEmail, actual.ConfirmEmailAddressEmailSettings.From.Email);
        Assert.Equal(fromName, actual.ConfirmEmailAddressEmailSettings.From.Name);
        Assert.Equal(confirmEmailBaseUri, actual.ConfirmEmailAddressEmailSettings.ConfirmEmailBaseUri);
        Assert.Equal(templateId, actual.ConfirmEmailAddressEmailSettings.TemplateId);
    }

    [Theory]
    [InlineData("", "", "", "")]
    [InlineData("tmathers@some-site.com", "tmathers", "www.some-site.com/change-password", "1")]
    public void Build_ResetPasswordEmailSettingsOverrides_ReturnsOverrides(string fromEmail, string fromName, string changePasswordBaseUri, string templateId)
    {
        var json = JsonSerializer.Serialize(new
        {
            IamEmailSettings = new IamEmailSettings()
            {
                ResetPasswordEmailSettings = new()
                {
                    From = new EmailAddress
                    {
                        Email = fromEmail,
                        Name = fromName,
                    },
                    ChangePasswordBaseUri = changePasswordBaseUri,
                    TemplateId = templateId
                }
            }
        });
        var configuration = ConfigurationBuilder.Build(json);

        // Act
        var actual = IamEmailSettingsBuilder.Build(configuration.GetSection("IamEmailSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.ConfirmEmailAddressEmailSettings);
        Assert.NotNull(actual.ConfirmEmailAddressEmailSettings.From);
        Assert.Equal(fromEmail, actual.ResetPasswordEmailSettings.From.Email);
        Assert.Equal(fromName, actual.ResetPasswordEmailSettings.From.Name);
        Assert.Equal(changePasswordBaseUri, actual.ResetPasswordEmailSettings.ChangePasswordBaseUri);
        Assert.Equal(templateId, actual.ResetPasswordEmailSettings.TemplateId);
    }
}
