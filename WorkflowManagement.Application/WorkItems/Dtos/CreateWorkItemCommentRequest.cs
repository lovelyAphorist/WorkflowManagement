using System.ComponentModel.DataAnnotations;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class CreateWorkItemCommentRequest
    {
        [Required]
        [StringLength(2000)]
        public string Body { get; set; } = string.Empty;
    }
}