using System.Net.Http.Json;
using System.Text.Json;
using ThomasMathers.Infrastructure.IAM.Social.Google.Models;
using ThomasMathers.Infrastructure.IAM.Social.Mappers;
using ThomasMathers.Infrastructure.IAM.Social.Models;
using ThomasMathers.Infrastructure.IAM.Social.Services;

namespace ThomasMathers.Infrastructure.IAM.Social.Google
{
    public class GoogleProfileService : ISocialMediaProfileService
    {
        private readonly HttpClient _httpClient;

        private static readonly JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
        };

        public string Name => "Google";

        public GoogleProfileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<SocialMediaProfile> GetSocialMediaProfile(string userId, string accessToken, CancellationToken cancellationToken = default)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"?access_token={accessToken}");

            var response = await _httpClient.SendAsync(request, cancellationToken);

            response.EnsureSuccessStatusCode();

            var googleProfile = await response.Content.ReadFromJsonAsync<GoogleProfile>(jsonSerializerOptions, cancellationToken);

            return SocialMediaProfileMapper.Map(googleProfile!);
        }
    }
}
