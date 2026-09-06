using WorkflowManagement.Application.Common;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Application.WorkItems.Repositories
{
    public interface IWorkItemRepository
    {
        Task<WorkItem> AddAsync(WorkItem workItem);
        Task<WorkItem?> GetByIdAsync(Guid id);
        Task<PagedResult<WorkItem>> GetAllAsync(WorkItemQueryRequest query);
        Task<WorkItem> UpdateAsync(WorkItem workItem, IReadOnlyCollection<WorkItemHistory> historyEntries);
        Task DeleteAsync(WorkItem workItem);
    }
}
