using System.ComponentModel.DataAnnotations;

namespace ThomasMathers.Common.IAM.Requests
{
    public record GeneratePasswordResetTokenRequest
    {
        [Required]
        public string UserName { get; init; } = string.Empty;
    }
}
