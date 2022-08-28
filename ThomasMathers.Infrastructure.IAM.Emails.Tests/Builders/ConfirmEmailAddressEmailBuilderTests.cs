using AutoFixture;

using Microsoft.AspNetCore.WebUtilities;

using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using ThomasMathers.Infrastructure.IAM.Notifications;

using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Emails.Tests.Builders;

public class ConfirmEmailAddressEmailBuilderTests
{
    private readonly Fixture _fixture;
    private readonly ConfirmEmailAddressEmailSettings _settings;
    private readonly ConfirmEmailAddressEmailBuilder _sut;

    public ConfirmEmailAddressEmailBuilderTests()
    {
        _fixture = new Fixture();
        _settings = _fixture.Create<ConfirmEmailAddressEmailSettings>();
        _sut = new ConfirmEmailAddressEmailBuilder(_settings);
    }

    [Fact]
    public void Build_ReturnsCorrectly()
    {
        // Arrange
        var notification = _fixture.Create<UserRegisteredNotification>();
        var expectedLink = QueryHelpers.AddQueryString(_settings.ConfirmEmailBaseUri, "t", notification.Token);

        // Act
        var actual = _sut.Build(notification);

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.From);
        Assert.Equal(_settings.From.Name, actual.From.Name);
        Assert.Equal(_settings.From.Email, actual.From.Email);
        Assert.NotNull(actual.To);
        Assert.Equal(notification.User.UserName, actual.To.Name);
        Assert.Equal(notification.User.Email, actual.To.Email);
        Assert.Equal(_settings.TemplateId, actual.TemplateId);
        Assert.Equal(notification.User.UserName, actual.Payload.Username);
        Assert.Equal(expectedLink, actual.Payload.Link);
    }
}