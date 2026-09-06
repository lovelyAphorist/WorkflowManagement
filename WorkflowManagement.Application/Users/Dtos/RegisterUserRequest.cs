using System.ComponentModel.DataAnnotations;

namespace WorkflowManagement.Application.Users.Dtos
{
    public class RegisterUserRequest
    {
        [Required]
        [StringLength(100)]
        public string DisplayName { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}