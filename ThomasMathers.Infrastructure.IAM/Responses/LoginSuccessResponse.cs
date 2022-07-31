using ThomasMathers.Infrastructure.IAM.Data;

namespace ThomasMathers.Infrastructure.IAM.Responses;

public record LoginSuccessResponse(User User, string AccessToken);