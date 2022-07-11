using OneOf;

namespace ThomasMathers.Common.IAM.Responses
{
    [GenerateOneOf]
    public partial class LoginResponse : OneOfBase<NotFoundResponse, UserLockedOutResponse, LoginRequiresTwoFactorResponse, LoginIsNotAllowedResponse, LoginFailureResponse, LoginSuccessResponse>
    {
    }
}
