using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
   public class CreateWorkItemRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public WorkItemPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
