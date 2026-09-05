using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Application.WorkItems.Repositories
{
    public interface IWorkItemRepository
    {
        Task<WorkItem> AddAsync(WorkItem workItem);
        Task<WorkItem?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<WorkItem>> GetAllAsync();
        Task<WorkItem> UpdateAsync(WorkItem workItem);
    }
}
