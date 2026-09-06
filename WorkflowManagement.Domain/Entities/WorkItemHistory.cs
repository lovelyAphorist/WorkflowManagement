using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Domain.Entities
{
    public class WorkItemHistory
    {
        public Guid Id { get; set; }
        public Guid WorkItemId { get; set; }
        public WorkItemChangeType ChangeType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangedAtUtc { get; set; }
        public WorkItem WorkItem { get; set; } = null!;
    }
}