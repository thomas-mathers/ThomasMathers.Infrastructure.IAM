using OneOf;

namespace ThomasMathers.Infrastructure.IAM.Responses;

[GenerateOneOf]
public partial class ResetPasswordResponse : OneOfBase<NotFoundResponse, ResetPasswordSuccessResponse>
{
}