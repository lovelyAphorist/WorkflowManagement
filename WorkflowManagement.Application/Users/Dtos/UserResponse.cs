namespace WorkflowManagement.Application.Users.Dtos
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}