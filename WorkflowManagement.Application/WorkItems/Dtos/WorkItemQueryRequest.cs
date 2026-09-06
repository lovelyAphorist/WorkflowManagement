using System.ComponentModel.DataAnnotations;
using WorkflowManagement.Application.WorkItems.Enums;
using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class WorkItemQueryRequest
    {
        public string? Search { get; set; }
        public WorkItemStatus? Status { get; set; }
        public WorkItemPriority? Priority { get; set; }
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(1, 100)]
        public int PageSize { get; set; } = 20;
        public WorkItemSortField SortBy { get; set; } = WorkItemSortField.CreatedAt;
        public SortDirection SortDirection { get; set; } = SortDirection.Descending;
    }
} 