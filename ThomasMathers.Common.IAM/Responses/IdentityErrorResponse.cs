using Microsoft.AspNetCore.Identity;

namespace ThomasMathers.Infrastructure.IAM.Responses
{
    public record IdentityErrorResponse(IEnumerable<IdentityError> Errors);
}
