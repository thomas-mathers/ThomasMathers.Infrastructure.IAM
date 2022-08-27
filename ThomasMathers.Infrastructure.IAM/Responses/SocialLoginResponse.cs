using OneOf;

namespace ThomasMathers.Infrastructure.IAM.Responses;

[GenerateOneOf]
public partial class SocialLoginResponse : OneOfBase<SocialLoginProviderUnsupported, SocialLoginSuccessResponse>
{
}