using System.ComponentModel.DataAnnotations;
using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class UpdateWorkItemRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;
        [StringLength(2000)]
        public string? Description { get; set; }
        [Required]
        public WorkItemStatus? Status { get; set; }
        [Required]
        public WorkItemPriority? Priority { get; set; }
        public DateOnly? DueDate { get; set; }
    }
}