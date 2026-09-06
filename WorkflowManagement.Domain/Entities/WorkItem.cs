using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Domain.Entities
{
    public class WorkItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkItemStatus Status { get; set; }
        public WorkItemPriority Priority { get; set; }
        public DateOnly? DueDate { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        public ICollection<WorkItemHistory> History { get; set; } = new List<WorkItemHistory>();
        public Guid? AssigneeId { get; set; }
    }
}
