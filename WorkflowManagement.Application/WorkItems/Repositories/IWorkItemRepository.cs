using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Application.WorkItems.Repositories
{
    public interface IWorkItemRepository
    {
        Task<WorkItem> AddAsync(WorkItem workItem);
        Task<WorkItem?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<WorkItem>> GetAllAsync(WorkItemQueryRequest query);
        Task<WorkItem> UpdateAsync(WorkItem workItem);
        Task DeleteAsync(WorkItem workItem);
    }
}
