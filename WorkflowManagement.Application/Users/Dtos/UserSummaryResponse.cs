namespace WorkflowManagement.Application.Users.Dtos
{
    public class UserSummaryResponse
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
    }
}