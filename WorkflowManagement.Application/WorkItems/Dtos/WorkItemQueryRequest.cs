using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class WorkItemQueryRequest
    {
        public string? Search { get; set; }

        public WorkItemStatus? Status { get; set; }

        public WorkItemPriority? Priority { get; set; }
    }
}