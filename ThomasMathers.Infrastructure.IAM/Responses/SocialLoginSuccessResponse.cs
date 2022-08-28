using ThomasMathers.Infrastructure.IAM.Data.EF;

namespace ThomasMathers.Infrastructure.IAM.Responses;

public record SocialLoginSuccessResponse(User User, string AccessToken) { };
