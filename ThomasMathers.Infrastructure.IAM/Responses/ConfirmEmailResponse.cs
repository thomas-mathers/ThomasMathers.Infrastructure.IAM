using OneOf;

namespace ThomasMathers.Infrastructure.IAM.Responses;

[GenerateOneOf]
public partial class
    ConfirmEmailResponse : OneOfBase<NotFoundResponse, IdentityErrorResponse, ConfirmEmailSuccessResponse>
{
}