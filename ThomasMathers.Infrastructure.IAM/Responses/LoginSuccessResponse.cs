namespace ThomasMathers.Infrastructure.IAM.Responses;

public record LoginSuccessResponse(Guid UserId, string UserName, string UserEmail, string AccessToken);