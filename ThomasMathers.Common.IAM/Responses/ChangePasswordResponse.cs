using OneOf;

namespace ThomasMathers.Common.IAM.Responses
{
    [GenerateOneOf]
    public partial class ChangePasswordResponse : OneOfBase<NotFoundResponse, IdentityErrorResponse, ChangePasswordSuccessResponse>
    {
    }
}
