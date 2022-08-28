using ThomasMathers.Infrastructure.IAM.Data.EF;

namespace ThomasMathers.Infrastructure.IAM.Social.Responses;

public record SocialLoginSuccessResponse(User User, string AccessToken) { };
