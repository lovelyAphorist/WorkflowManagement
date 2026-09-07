using WorkflowManagement.Domain.Enums;
using WorkflowManagement.Application.Users.Dtos;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class WorkItemResponse
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkItemStatus Status { get; set; }
        public WorkItemPriority Priority { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public Guid? AssigneeId { get; set; }
        public UserSummaryResponse? Assignee { get; set; }
    }
}
