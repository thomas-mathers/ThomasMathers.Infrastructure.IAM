namespace ThomasMathers.Infrastructure.IAM.Social.Providers.Facebook.Models;

public class FacebookPictureData
{
    public int Width { get; init; }
    public int Height { get; init; }
    public string Url { get; init; } = string.Empty;
    public bool IsSilhouette { get; init; }
}