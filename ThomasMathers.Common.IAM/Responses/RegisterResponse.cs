using OneOf;

namespace ThomasMathers.Infrastructure.IAM.Responses
{
    [GenerateOneOf]
    public partial class RegisterResponse : OneOfBase<IdentityErrorResponse, RegisterSuccessResponse>
    {
    }
}
