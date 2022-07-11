using Microsoft.AspNetCore.Identity;

namespace ThomasMathers.Common.IAM.Responses
{
    public record IdentityErrorResponse(IEnumerable<IdentityError> Errors);
}
