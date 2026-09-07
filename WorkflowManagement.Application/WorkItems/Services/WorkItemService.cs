using WorkflowManagement.Application.Common;
using WorkflowManagement.Application.Users.Dtos;
using WorkflowManagement.Application.Users.Services;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Application.WorkItems.Enums;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Domain.Entities;
using WorkflowManagement.Domain.Enums;

namespace WorkflowManagement.Application.WorkItems.Services
{
    public class WorkItemService : IWorkItemService
    {
        private readonly IWorkItemRepository _repository;
        private readonly IUserService _userService;

        public WorkItemService(IWorkItemRepository repository, IUserService userService)
        {
            _repository = repository;
            _userService = userService;
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

            return await MapToResponseWithAssigneeAsync(workItem);
        }

        public async Task<PagedResult<WorkItemResponse>> GetAllAsync(WorkItemQueryRequest query)
        {
            var result = await _repository.GetAllAsync(query);

            var assigneeIds = result.Items
                .Where(w => w.AssigneeId.HasValue)
                .Select(w => w.AssigneeId!.Value)
                .Distinct()
                .ToList();

            IReadOnlyList<UserResponse> assignees = assigneeIds.Count == 0
                ? []
                : await _userService.GetByIdsAsync(assigneeIds);

            var assigneeLookup = assignees.ToDictionary(u => u.Id);

            var items = result.Items
                .Select(workItem =>
                {
                    UserResponse? assignee = null;

                    if (workItem.AssigneeId.HasValue)
                    {
                        assigneeLookup.TryGetValue(
                            workItem.AssigneeId.Value,
                            out assignee);
                    }

                    return MapToResponse(workItem, assignee);
                })
                .ToList();

            return new PagedResult<WorkItemResponse>
            {
                Items = items,
                Page = result.Page,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };
        }
        public async Task<WorkItemResponse?> UpdateAsync(
            Guid id,
            UpdateWorkItemRequest request)
        {
            var workItem = await _repository.GetByIdAsync(id);

            if (workItem is null)
            {
                return null;
            }

            var changedAtUtc = DateTime.UtcNow;
            var newTitle = request.Title.Trim();

            var historyEntries = new List<WorkItemHistory>();

            if (workItem.Status != request.Status.Value)
            {
                historyEntries.Add(new WorkItemHistory
                {
                    Id = Guid.NewGuid(),
                    WorkItemId = workItem.Id,
                    ChangeType = WorkItemChangeType.Status,
                    OldValue = workItem.Status.ToString(),
                    NewValue = request.Status.Value.ToString(),
                    ChangedAtUtc = changedAtUtc
                });
            }

            if (workItem.Priority != request.Priority.Value)
            {
                historyEntries.Add(new WorkItemHistory
                {
                    Id = Guid.NewGuid(),
                    WorkItemId = workItem.Id,
                    ChangeType = WorkItemChangeType.Priority,
                    OldValue = workItem.Priority.ToString(),
                    NewValue = request.Priority.Value.ToString(),
                    ChangedAtUtc = changedAtUtc
                });
            }

            if (workItem.Title != newTitle)
            {
                historyEntries.Add(new WorkItemHistory
                {
                    Id = Guid.NewGuid(),
                    WorkItemId = workItem.Id,
                    ChangeType = WorkItemChangeType.Title,
                    OldValue = workItem.Title,
                    NewValue = newTitle,
                    ChangedAtUtc = changedAtUtc
                });
            }

            if (workItem.Description != request.Description)
            {
                historyEntries.Add(new WorkItemHistory
                {
                    Id = Guid.NewGuid(),
                    WorkItemId = workItem.Id,
                    ChangeType = WorkItemChangeType.Description,
                    OldValue = workItem.Description,
                    NewValue = request.Description,
                    ChangedAtUtc = changedAtUtc
                });
            }

            if (workItem.DueDate != request.DueDate)
            {
                historyEntries.Add(new WorkItemHistory
                {
                    Id = Guid.NewGuid(),
                    WorkItemId = workItem.Id,
                    ChangeType = WorkItemChangeType.DueDate,
                    OldValue = workItem.DueDate?.ToString("yyyy-MM-dd"),
                    NewValue = request.DueDate?.ToString("yyyy-MM-dd"),
                    ChangedAtUtc = changedAtUtc
                });
            }

            if (historyEntries.Count == 0)
            {
                return MapToResponse(workItem);
            }

            workItem.Title = newTitle;
            workItem.Description = request.Description;
            workItem.Status = request.Status.Value;
            workItem.Priority = request.Priority.Value;
            workItem.DueDate = request.DueDate;
            workItem.UpdatedAtUtc = changedAtUtc;

            var updatedWorkItem =
                await _repository.UpdateAsync(workItem, historyEntries);

            return await MapToResponseWithAssigneeAsync(updatedWorkItem);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var workItem = await _repository.GetByIdAsync(id);

            if (workItem is null)
            {
                return false;
            }

            await _repository.DeleteAsync(workItem);

            return true;
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
                AssigneeId = workItem.AssigneeId,
                CreatedAtUtc = workItem.CreatedAtUtc,
                UpdatedAtUtc = workItem.UpdatedAtUtc
            };
        }
        private static WorkItemResponse MapToResponse(WorkItem workItem, UserResponse? assignee)
        {
            var response = MapToResponse(workItem);

            if (assignee is not null)
            {
                response.Assignee = new UserSummaryResponse
                {
                    Id = assignee.Id,
                    DisplayName = assignee.DisplayName
                };
            }

            return response;
        }
        private static WorkItemHistoryResponse MapHistoryToResponse(WorkItemHistory history)
        {
            return new WorkItemHistoryResponse
            {
                Id = history.Id,
                ChangeType = history.ChangeType,
                OldValue = history.OldValue,
                NewValue = history.NewValue,
                ChangedAtUtc = history.ChangedAtUtc
            };
        }
        public async Task<IReadOnlyList<WorkItemHistoryResponse>?> GetHistoryAsync(Guid id)
        {
            var workItem = await _repository.GetByIdAsync(id);

            if (workItem is null)
            {
                return null;
            }

            var history = await _repository.GetHistoryAsync(id);

            return history
                .Select(MapHistoryToResponse)
                .ToList();
        }
        public async Task<AssignWorkItemResult> AssignAsync(Guid id, AssignWorkItemRequest request)
        {
            var workItem = await _repository.GetByIdAsync(id);

            if (workItem is null)
            {
                return new AssignWorkItemResult
                {
                    Status = AssignWorkItemStatus.WorkItemNotFound
                };
            }

            if (workItem.AssigneeId == request.AssigneeId)
            {
                return new AssignWorkItemResult
                {
                    Status = AssignWorkItemStatus.Success,
                    WorkItem = await MapToResponseWithAssigneeAsync(workItem)
                };
            }

            UserResponse? newAssignee = null;

            if (request.AssigneeId.HasValue)
            {
                newAssignee = await _userService.GetByIdAsync(
                    request.AssigneeId.Value);

                if (newAssignee is null)
                {
                    return new AssignWorkItemResult
                    {
                        Status = AssignWorkItemStatus.AssigneeNotFound
                    };
                }
            }

            UserResponse? oldAssignee = null;

            if (workItem.AssigneeId.HasValue)
            {
                oldAssignee = await _userService.GetByIdAsync(
                    workItem.AssigneeId.Value);
            }

            var changedAtUtc = DateTime.UtcNow;

            var historyEntries = new List<WorkItemHistory>
           {
              new WorkItemHistory
                {
                Id = Guid.NewGuid(),
                WorkItemId = workItem.Id,
                ChangeType = WorkItemChangeType.Assignee,
                OldValue = oldAssignee?.DisplayName,
                NewValue = newAssignee?.DisplayName,
                ChangedAtUtc = changedAtUtc
            }
                };
            workItem.AssigneeId = request.AssigneeId;
            workItem.UpdatedAtUtc = changedAtUtc;

            var updatedWorkItem = await _repository.UpdateAsync(workItem, historyEntries);

            return new AssignWorkItemResult
            {
                Status = AssignWorkItemStatus.Success,
                WorkItem = await MapToResponseWithAssigneeAsync(updatedWorkItem)
            };
        }
        private async Task<WorkItemResponse> MapToResponseWithAssigneeAsync(
    WorkItem workItem)
        {
            UserResponse? assignee = null;

            if (workItem.AssigneeId.HasValue)
            {
                assignee = await _userService.GetByIdAsync(
                    workItem.AssigneeId.Value);
            }

            return MapToResponse(workItem, assignee);
        }
    }
}
