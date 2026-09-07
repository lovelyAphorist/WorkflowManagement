using WorkflowManagement.Application.WorkItems.Enums;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class WorkItemCommentOperationResult
    {
        public WorkItemCommentOperationStatus Status { get; set; }
        public WorkItemCommentResponse? Comment { get; set; }
    }
}