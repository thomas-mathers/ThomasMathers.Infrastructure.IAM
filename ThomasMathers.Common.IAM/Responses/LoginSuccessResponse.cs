namespace ThomasMathers.Common.IAM.Responses
{
    public record LoginSuccessResponse(Guid UserId, string UserName, string UserEmail, string AccessToken);
}
