using OneOf;

namespace ThomasMathers.Infrastructure.IAM.Responses
{
    [GenerateOneOf]
    public partial class GeneratePasswordResetTokenResponse : OneOfBase<NotFoundResponse, string>
    {
    }
}
