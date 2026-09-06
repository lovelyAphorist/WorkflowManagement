using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class WorkItemHistoryResponse
    {
        public Guid Id { get; set; }
        public WorkItemChangeType ChangeType { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime ChangedAtUtc { get; set; }
    }
}