using System.Net.Http.Json;
using System.Text.Json;

using ThomasMathers.Infrastructure.IAM.Data.EF;
using ThomasMathers.Infrastructure.IAM.Social.Providers.Google.Mappers;
using ThomasMathers.Infrastructure.IAM.Social.Providers.Google.Models;

namespace ThomasMathers.Infrastructure.IAM.Social.Providers.Google;

public class GoogleProfileService : ISocialMediaProfileService
{
    private readonly HttpClient _httpClient;

    private static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
    };

    public string Name => "Google";

    public GoogleProfileService(HttpClient httpClient) => _httpClient = httpClient;

    public async Task<SocialMediaProfile> GetSocialMediaProfile(string userId, string accessToken, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, $"?access_token={accessToken}");

        var response = await _httpClient.SendAsync(request, cancellationToken);

        _ = response.EnsureSuccessStatusCode();

        var googleProfile = await response.Content.ReadFromJsonAsync<GoogleProfile>(jsonSerializerOptions, cancellationToken);

        return GoogleProfileToSocialMediaProfileMapper.Map(googleProfile!);
    }
}
