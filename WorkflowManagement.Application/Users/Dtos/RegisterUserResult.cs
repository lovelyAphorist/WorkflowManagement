namespace WorkflowManagement.Application.Users.Dtos
{
    public class RegisterUserResult
    {
        public bool Succeeded { get; set; }
        public UserResponse? User { get; set; }
        public IReadOnlyList<string> Errors { get; set; } = [];
    }
}