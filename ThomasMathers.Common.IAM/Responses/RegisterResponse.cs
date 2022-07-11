using OneOf;

namespace ThomasMathers.Common.IAM.Responses
{
    [GenerateOneOf]
    public partial class RegisterResponse : OneOfBase<IdentityErrorResponse, RegisterSuccessResponse>
    {
    }
}
