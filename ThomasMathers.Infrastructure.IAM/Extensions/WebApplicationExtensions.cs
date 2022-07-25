using Microsoft.AspNetCore.Builder;

namespace ThomasMathers.Infrastructure.IAM.Extensions;

public static class WebApplicationExtensions
{
    public static void UseIAM(this WebApplication webApplication)
    {
        webApplication.UseAuthentication();
        webApplication.UseAuthorization();
    }
}