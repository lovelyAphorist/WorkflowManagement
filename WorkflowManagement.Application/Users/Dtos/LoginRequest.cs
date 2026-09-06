using System.ComponentModel.DataAnnotations;

namespace WorkflowManagement.Application.Users.Dtos
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}