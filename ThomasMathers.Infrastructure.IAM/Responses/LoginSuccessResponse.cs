using ThomasMathers.Infrastructure.IAM.Data.EF;

namespace ThomasMathers.Infrastructure.IAM.Responses;

public record LoginSuccessResponse(User User, string AccessToken);