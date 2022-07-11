using OneOf;

namespace ThomasMathers.Common.IAM.Responses
{
    [GenerateOneOf]
    public partial class GeneratePasswordResetTokenResponse : OneOfBase<NotFoundResponse, string>
    {
    }
}
