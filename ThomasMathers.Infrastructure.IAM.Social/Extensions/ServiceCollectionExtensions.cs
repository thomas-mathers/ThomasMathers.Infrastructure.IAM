using Microsoft.Extensions.DependencyInjection;
using ThomasMathers.Infrastructure.IAM.Social.Facebook;
using ThomasMathers.Infrastructure.IAM.Social.Google;

namespace ThomasMathers.Infrastructure.IAM.Social.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddSocialMediaServices(this IServiceCollection services)
        {
            services.AddHttpClient<ISocialMediaProfileService, FacebookProfileService>(factory => 
            {
                factory.BaseAddress = new Uri("https://graph.facebook.com");
            });
            services.AddHttpClient<ISocialMediaProfileService, GoogleProfileService>(factory =>
            {
                factory.BaseAddress = new Uri("https://www.googleapis.com/oauth2/v1/userinfo");
            });
        }
    }
}
