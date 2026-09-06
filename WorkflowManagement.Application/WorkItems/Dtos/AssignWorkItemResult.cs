using WorkflowManagement.Application.WorkItems.Enums;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class AssignWorkItemResult
    {
        public AssignWorkItemStatus Status { get; set; }
        public WorkItemResponse? WorkItem { get; set; }
    }
}