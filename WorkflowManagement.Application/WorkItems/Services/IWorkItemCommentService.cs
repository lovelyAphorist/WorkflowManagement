using WorkflowManagement.Application.WorkItems.Dtos;

namespace WorkflowManagement.Application.WorkItems.Services
{
    public interface IWorkItemCommentService
    {
        Task<WorkItemCommentResponse?> CreateAsync(Guid workItemId, Guid authorId, CreateWorkItemCommentRequest request);
        Task<IReadOnlyList<WorkItemCommentResponse>?> GetAllAsync(Guid workItemId);
        Task<WorkItemCommentOperationResult> UpdateAsync(Guid workItemId, Guid commentId, Guid userId, UpdateWorkItemCommentRequest request);
        Task<WorkItemCommentOperationResult> DeleteAsync(Guid workItemId, Guid commentId, Guid userId, bool isAdmin);
    }
}