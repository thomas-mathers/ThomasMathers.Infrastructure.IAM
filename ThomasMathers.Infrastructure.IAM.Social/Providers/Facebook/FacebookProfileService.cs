using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

using ThomasMathers.Infrastructure.Common.Serialization.Json;
using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Social.Providers.Facebook.Mappers;
using ThomasMathers.Infrastructure.IAM.Social.Providers.Facebook.Models;

namespace ThomasMathers.Infrastructure.IAM.Social.Providers.Facebook;

public class FacebookProfileService : ISocialMediaProfileService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = new SnakeCaseNamingPolicy()
    };

    public string Name => "Facebook";

    public FacebookProfileService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<SocialMediaProfile> GetSocialMediaProfile(string userId, string accessToken, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"/{userId}?fields=id,email,name,picture");

        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        _ = response.EnsureSuccessStatusCode();

        var facebookProfile = await response.Content.ReadFromJsonAsync<FacebookProfile>(jsonSerializerOptions, cancellationToken);

        return FacebookProfileToSocialMediaProfileMapper.Map(facebookProfile!);
    }
}
