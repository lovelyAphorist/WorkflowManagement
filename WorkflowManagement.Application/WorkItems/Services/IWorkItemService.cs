using WorkflowManagement.Application.Common;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Application.WorkItems.Services
{
    public interface IWorkItemService
    {
        Task<WorkItemResponse> CreateAsync(CreateWorkItemRequest request);
        Task<WorkItemResponse?> GetByIdAsync(Guid id);
        Task<PagedResult<WorkItemResponse>> GetAllAsync(WorkItemQueryRequest query);
        Task<WorkItemResponse?> UpdateAsync(Guid id, UpdateWorkItemRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
