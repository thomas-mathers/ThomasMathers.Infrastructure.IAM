using System.ComponentModel.DataAnnotations;

namespace ThomasMathers.Common.IAM.Requests
{
    public record RegisterRequest
    {
        [Required]
        public string UserName { get; init; } = string.Empty;
        [Required]
        public string Email { get; init; } = string.Empty;
        [Required]
        public string Password { get; init; } = string.Empty;
    }
}
