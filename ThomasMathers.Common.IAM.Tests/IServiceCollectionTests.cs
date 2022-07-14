using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Common.IAM.Extensions;
using ThomasMathers.Common.IAM.Services;
using ThomasMathers.Common.IAM.Settings;
using Xunit;

namespace ThomasMathers.Common.IAM.Tests
{
    public class IServiceCollectionTests
    {
        private readonly IServiceCollection _services;

        public IServiceCollectionTests()
        {
            _services = new ServiceCollection();
        }

        [Fact]
        public void AddIAM_RegistersRequiredServices()
        {
            // Arrange
            var settings = new IAMSettings();

            // Act
            _services.AddIAM(settings);
            var serviceProvider = _services.BuildServiceProvider();

            // Assert
            Assert.NotNull(serviceProvider.GetRequiredService<IUserService>());
            Assert.NotNull(serviceProvider.GetRequiredService<IAuthService>());
            Assert.NotNull(serviceProvider.GetRequiredService<IAccessTokenGenerator>());
            Assert.NotNull(serviceProvider.GetRequiredService<IAMSettings>());
            Assert.NotNull(serviceProvider.GetRequiredService<JwtTokenSettings>());
            Assert.NotNull(serviceProvider.GetRequiredService<PasswordSettings>());
        }
    }
}
