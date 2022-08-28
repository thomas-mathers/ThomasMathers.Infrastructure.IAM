using AutoFixture;

using Microsoft.AspNetCore.WebUtilities;

using ThomasMathers.Infrastructure.IAM.Emails.Builders;
using ThomasMathers.Infrastructure.IAM.Emails.Settings;
using ThomasMathers.Infrastructure.IAM.Notifications;

using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Emails.Tests.Builders;

public class ResetPasswordEmailBuilderTests
{
    private readonly Fixture _fixture;
    private readonly ResetPasswordEmailSettings _settings;
    private readonly ResetPasswordEmailBuilder _sut;

    public ResetPasswordEmailBuilderTests()
    {
        _fixture = new Fixture();
        _settings = _fixture.Create<ResetPasswordEmailSettings>();
        _sut = new ResetPasswordEmailBuilder(_settings);
    }

    [Fact]
    public void Build_ReturnsCorrectResult()
    {
        // Arrange
        var notification = _fixture.Create<ResetPasswordNotification>();
        var expectedLink = QueryHelpers.AddQueryString(_settings.ChangePasswordBaseUri, "t", notification.Token);

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