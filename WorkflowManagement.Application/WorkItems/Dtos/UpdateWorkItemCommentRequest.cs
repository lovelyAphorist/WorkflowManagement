using System.ComponentModel.DataAnnotations;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class UpdateWorkItemCommentRequest
    {
        [Required]
        [StringLength(2000)]
        public string Body { get; set; } = string.Empty;
    }
}