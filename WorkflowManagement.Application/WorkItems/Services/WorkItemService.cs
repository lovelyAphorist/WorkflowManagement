using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Domain.Entities;
using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Application.WorkItems.Services
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IWorkItemRepository _repository;

        public WorkItemService(IWorkItemRepository repository)
        {
            _repository = repository;
        }
        public async Task<WorkItemResponse> CreateAsync(CreateWorkItemRequest request)
        {
            var now = DateTime.UtcNow;

            var workItem = new WorkItem
            {
                Id = Guid.NewGuid(),
                Title = request.Title.Trim(),
                Description = request.Description,
                Status = WorkItemStatus.Backlog,
                Priority = request.Priority,
                DueDate = request.DueDate,
                CreatedAtUtc = now,
                UpdatedAtUtc = now
            };

            var createdWorkItem = await _repository.AddAsync(workItem);

            return MapToResponse(createdWorkItem);
        }
        public async Task<WorkItemResponse?> GetByIdAsync(Guid id)
        {
            var workItem = await _repository.GetByIdAsync(id);
            if (workItem is null)
            {
                return null;
            }
            return MapToResponse(workItem);
        }
        private static WorkItemResponse MapToResponse(WorkItem workItem)
        {
            return new WorkItemResponse
            {
                Id = workItem.Id,
                Title = workItem.Title,
                Description = workItem.Description,
                Status = workItem.Status,
                Priority = workItem.Priority,
                DueDate = workItem.DueDate,
                CreatedAtUtc = workItem.CreatedAtUtc,
                UpdatedAtUtc = workItem.UpdatedAtUtc
            };
        }
    }
}
