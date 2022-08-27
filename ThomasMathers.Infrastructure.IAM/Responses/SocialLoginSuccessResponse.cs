using ThomasMathers.Infrastructure.IAM.Data;

namespace ThomasMathers.Infrastructure.IAM.Responses;

public record SocialLoginSuccessResponse(User User, string AccessToken) { };
