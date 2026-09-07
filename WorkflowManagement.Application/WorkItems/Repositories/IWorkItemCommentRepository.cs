using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Application.WorkItems.Repositories
{
    public interface IWorkItemCommentRepository
    {
        Task<WorkItemComment> AddAsync(WorkItemComment comment);
        Task<IReadOnlyList<WorkItemComment>> GetByWorkItemIdAsync(Guid workItemId);
        Task<WorkItemComment?> GetByIdAsync(Guid id);
        Task<WorkItemComment> UpdateAsync(WorkItemComment comment);
        Task DeleteAsync(WorkItemComment comment);
    }
}