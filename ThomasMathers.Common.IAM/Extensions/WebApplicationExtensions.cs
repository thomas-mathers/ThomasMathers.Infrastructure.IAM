using Microsoft.AspNetCore.Builder;

namespace ThomasMathers.Common.IAM.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void UseIAM(this WebApplication webApplication)
        {
            webApplication.UseAuthentication();
            webApplication.UseAuthorization();
        }
    }
}
