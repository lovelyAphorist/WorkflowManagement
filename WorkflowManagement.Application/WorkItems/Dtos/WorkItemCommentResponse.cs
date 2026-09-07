using WorkflowManagement.Application.Users.Dtos;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class WorkItemCommentResponse
    {
        public Guid Id { get; set; }
        public string Body { get; set; } = string.Empty;
        public UserSummaryResponse Author { get; set; } = null!;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? EditedAtUtc { get; set; }
    }
}