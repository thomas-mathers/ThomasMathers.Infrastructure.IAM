using OneOf;

namespace ThomasMathers.Infrastructure.IAM.Social.Responses;

[GenerateOneOf]
public partial class SocialLoginResponse : OneOfBase<SocialLoginProviderUnsupported, SocialLoginSuccessResponse>
{
}