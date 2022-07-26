using Microsoft.AspNetCore.Builder;

namespace ThomasMathers.Infrastructure.IAM.Extensions;

public static class WebApplicationExtensions
{
    public static void UseIam(this WebApplication webApplication)
    {
        webApplication.UseAuthentication();
        webApplication.UseAuthorization();
    }
}