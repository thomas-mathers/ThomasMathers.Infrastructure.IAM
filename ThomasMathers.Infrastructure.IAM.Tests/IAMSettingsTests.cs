using System.Text.Json;
using ThomasMathers.Infrastructure.IAM.Settings;
using ThomasMathers.Infrastructure.IAM.Tests.Helpers;
using Xunit;

namespace ThomasMathers.Infrastructure.IAM.Tests;

public class IAMSettingsTests
{
    [Theory]
    [InlineData("{}")]
    [InlineData("{\"IAMSettings\": {}}")]
    public void FromConfigurationSection_NoOverridesSpecified_ReturnsCorrectDefaults(string configJson)
    {
        var configuration = IConfigurationBuilder.Build(configJson);

        // Act
        var actual = IAMSettings.FromConfigurationSection(configuration.GetSection("IAMSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.ConnectionString);
        Assert.Empty(actual.ConnectionString);
        Assert.NotNull(actual.PasswordSettings);
        Assert.True(actual.PasswordSettings.RequireDigit);
        Assert.Equal(6, actual.PasswordSettings.RequiredLength);
        Assert.Equal(1, actual.PasswordSettings.RequiredUniqueChars);
        Assert.True(actual.PasswordSettings.RequireLowercase);
        Assert.True(actual.PasswordSettings.RequireNonAlphanumeric);
        Assert.True(actual.PasswordSettings.RequireUppercase);
        Assert.NotNull(actual.JwtTokenSettings);
        Assert.NotNull(actual.JwtTokenSettings.Key);
        Assert.NotEmpty(actual.JwtTokenSettings.Key);
        Assert.NotNull(actual.JwtTokenSettings.Issuer);
        Assert.NotNull(actual.JwtTokenSettings.Audience);
        Assert.Equal(1, actual.JwtTokenSettings.LifespanInDays);
    }


    [Theory]
    [InlineData("")]
    [InlineData(
        "Server=tcp:myserver.database.windows.net,1433;Database=myDataBase;User ID=mylogin@myserver;Password=myPassword;Trusted_Connection=False;Encrypt=True;")]
    public void FromConfigurationSection_ConnectionStringOverride_ReturnsOverride(string connectionString)
    {
        var json = JsonSerializer.Serialize(new
        {
            IAMSettings = new IAMSettings
            {
                ConnectionString = connectionString
            }
        });
        var configuration = IConfigurationBuilder.Build(json);

        // Act
        var actual = IAMSettings.FromConfigurationSection(configuration.GetSection("IAMSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.Equal(connectionString, actual.ConnectionString);
    }

    [Theory]
    [InlineData("", "", "", 0)]
    [InlineData("=l3`[bZB%;4cT$!nLM3v0<pR~N28*R", "issuer", "audience", 360)]
    public void FromConfigurationSection_JwtOverride_ReturnsOverride(string key, string issuer, string audience,
        int lifespanInDays)
    {
        var json = JsonSerializer.Serialize(new
        {
            IAMSettings = new IAMSettings
            {
                JwtTokenSettings = new JwtTokenSettings
                {
                    Key = key,
                    Issuer = issuer,
                    Audience = audience,
                    LifespanInDays = lifespanInDays
                }
            }
        });
        var configuration = IConfigurationBuilder.Build(json);

        // Act
        var actual = IAMSettings.FromConfigurationSection(configuration.GetSection("IAMSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.JwtTokenSettings);
        Assert.Equal(key, actual.JwtTokenSettings.Key);
        Assert.Equal(issuer, actual.JwtTokenSettings.Issuer);
        Assert.Equal(audience, actual.JwtTokenSettings.Audience);
        Assert.Equal(lifespanInDays, actual.JwtTokenSettings.LifespanInDays);
    }

    [Theory]
    [InlineData(false, 2, 1, false, false, false)]
    [InlineData(false, 7, 3, false, false, true)]
    [InlineData(false, 8, 4, false, true, false)]
    [InlineData(false, 9, 6, false, true, true)]
    [InlineData(false, 14, 2, true, false, false)]
    [InlineData(false, 15, 3, true, false, true)]
    [InlineData(false, 7, 5, true, true, false)]
    [InlineData(false, 8, 9, true, true, true)]
    [InlineData(true, 9, 5, false, false, false)]
    [InlineData(true, 7, 4, false, false, true)]
    [InlineData(true, 3, 2, false, true, false)]
    [InlineData(true, 8, 5, false, true, true)]
    [InlineData(true, 6, 3, true, false, false)]
    [InlineData(true, 7, 2, true, false, true)]
    [InlineData(true, 5, 3, true, true, false)]
    [InlineData(true, 6, 1, true, true, true)]
    public void FromConfigurationSection_PasswordSettingsOverride_ReturnsOverride(bool requireDigit, int requiredLength,
        int requiredUniqueChars, bool requireLowercase, bool requireNonAlphanumeric, bool requireUppercase)
    {
        var json = JsonSerializer.Serialize(new
        {
            IAMSettings = new IAMSettings
            {
                PasswordSettings = new PasswordSettings
                {
                    RequireDigit = requireDigit,
                    RequiredLength = requiredLength,
                    RequiredUniqueChars = requiredUniqueChars,
                    RequireLowercase = requireLowercase,
                    RequireNonAlphanumeric = requireNonAlphanumeric,
                    RequireUppercase = requireUppercase
                }
            }
        });
        var configuration = IConfigurationBuilder.Build(json);

        // Act
        var actual = IAMSettings.FromConfigurationSection(configuration.GetSection("IAMSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.PasswordSettings);
        Assert.Equal(requireDigit, actual.PasswordSettings.RequireDigit);
        Assert.Equal(requiredLength, actual.PasswordSettings.RequiredLength);
        Assert.Equal(requiredUniqueChars, actual.PasswordSettings.RequiredUniqueChars);
        Assert.Equal(requireLowercase, actual.PasswordSettings.RequireLowercase);
        Assert.Equal(requireNonAlphanumeric, actual.PasswordSettings.RequireNonAlphanumeric);
        Assert.Equal(requireUppercase, actual.PasswordSettings.RequireUppercase);
    }

    [Theory]
    [InlineData(true, "abcdefghijklmnopqrstuvwxyz")]
    [InlineData(false, "abcdefghijklmnopqrstuvwxyz")]
    public void FromConfigurationSection_UserSettingsOverride_ReturnsOverride(bool requireUniqueEmail,
        string allowedUserNameCharacters)
    {
        var json = JsonSerializer.Serialize(new
        {
            IAMSettings = new IAMSettings
            {
                UserSettings = new UserSettings
                {
                    RequireUniqueEmail = requireUniqueEmail,
                    AllowedUserNameCharacters = allowedUserNameCharacters
                }
            }
        });
        var configuration = IConfigurationBuilder.Build(json);

        // Act
        var actual = IAMSettings.FromConfigurationSection(configuration.GetSection("IAMSettings"));

        // Assert
        Assert.NotNull(actual);
        Assert.NotNull(actual.UserSettings);
        Assert.Equal(requireUniqueEmail, actual.UserSettings.RequireUniqueEmail);
        Assert.Equal(allowedUserNameCharacters, actual.UserSettings.AllowedUserNameCharacters);
    }
}