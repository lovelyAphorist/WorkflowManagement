using WorkflowManagement.Application.WorkItems.Dtos;

namespace WorkflowManagement.Application.WorkItems.Services
{
    public interface IWorkItemService
    {
        Task<WorkItemResponse> CreateAsync(CreateWorkItemRequest request);
        Task<WorkItemResponse?> GetByIdAsync(Guid id);
        Task<IReadOnlyList<WorkItemResponse>> GetAllAsync();
        Task<WorkItemResponse?> UpdateAsync(Guid id, UpdateWorkItemRequest request);
    }
}
