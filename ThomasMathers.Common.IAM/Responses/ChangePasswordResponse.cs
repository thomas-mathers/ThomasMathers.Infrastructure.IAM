using OneOf;

namespace ThomasMathers.Infrastructure.IAM.Responses
{
    [GenerateOneOf]
    public partial class ChangePasswordResponse : OneOfBase<NotFoundResponse, IdentityErrorResponse, ChangePasswordSuccessResponse>
    {
    }
}
