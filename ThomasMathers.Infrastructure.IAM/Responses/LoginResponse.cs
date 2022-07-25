using OneOf;

namespace ThomasMathers.Infrastructure.IAM.Responses;

[GenerateOneOf]
public partial class LoginResponse : OneOfBase<NotFoundResponse, UserLockedOutResponse, LoginRequiresTwoFactorResponse,
    LoginIsNotAllowedResponse, LoginFailureResponse, LoginSuccessResponse>
{
}