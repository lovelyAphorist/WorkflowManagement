namespace WorkflowManagement.Application.Users.Dtos
{
    public class LoginResult
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAtUtc { get; set; }
        public UserResponse? User { get; set; }
        public IReadOnlyList<string> Errors { get; set; } = [];
    }
}