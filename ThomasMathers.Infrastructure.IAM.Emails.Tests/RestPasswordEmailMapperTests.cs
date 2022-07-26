using Microsoft.AspNetCore.WebUtilities;
using ThomasMathers.Infrastructure.Email;
using ThomasMathers.Infrastructure.IAM.Data;
using ThomasMathers.Infrastructure.IAM.Emails.Mappers;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using ThomasMathers.Infrastructure.IAM.Notifications;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Emails.Tests;

public class ResetPasswordEmailMapperTests
{
    private const string FromName = "some-company";
    private const string FromEmail = "no-reply@some-company.com";
    private const string ChangePasswordBaseUri = "some-company.com";
    private const string TemplateId = "1";
    private readonly ResetPasswordEmailMapper _sut;

    public ResetPasswordEmailMapperTests()
    {
        var resetPasswordEmailSettings = new ResetPasswordEmailSettings
        {
            ChangePasswordBaseUri = ChangePasswordBaseUri,
            From = new EmailAddress
            {
                Email = FromEmail,
                Name = FromName
            },
            TemplateId = TemplateId
        };
        _sut = new ResetPasswordEmailMapper(resetPasswordEmailSettings);
    }

    [Theory]
    [InlineData("", "", "")]
    [InlineData("tmathers", "thomas@some-site.com",
        "CfDJ8MuPWcfdsiJArYG5utFAvjlqbXJto97zTGWC6wPL4IiRd58VfjKzq1qULpe1WghTqWYnHXHzpo1SPilgR6EoZPsvfrGBEBp82vZICOMjz29L4cyed6L8HXrRfB27By+2G83HCfTbujLAXEks0kr8CYEs9YTSBPDaJkKU4oAUlukEjfR33E88orlfJU5Z64BVFw64QVHJrPuQ6YWl6P3Y1O8fVG/ZhUHnR+N4FyTQk5qt")]
    public void Map_MapsCorrectly(string userName, string userEmail, string token)
    {
        // Arrange
        var notification = new ResetPasswordNotification
        {
            User = new User
            {
                UserName = userName,
                Email = userEmail
            },
            Token = token
        };
        var expectedLink = QueryHelpers.AddQueryString(ChangePasswordBaseUri, "t", token);

        // Act
        var actual = _sut.Map(notification);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.From);
        Assert.Equal(FromName, actual.From.Name);
        Assert.Equal(FromEmail, actual.From.Email);
        Assert.NotNull(actual.To);
        Assert.Equal(userName, actual.To.Name);
        Assert.Equal(userEmail, actual.To.Email);
        Assert.Equal(TemplateId, actual.TemplateId);
        Assert.Equal(userName, actual.Payload.Username);
        Assert.Equal(expectedLink, actual.Payload.Link);
    }
}