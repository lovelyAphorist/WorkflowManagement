using WorkflowManagement.Application.WorkItems.Dtos;

namespace WorkflowManagement.Application.WorkItems.Services
{
    public interface IWorkItemCommentService
    {
        Task<WorkItemCommentResponse?> CreateAsync(
            Guid workItemId,
            Guid authorId,
            CreateWorkItemCommentRequest request);

        Task<IReadOnlyList<WorkItemCommentResponse>?> GetAllAsync(
            Guid workItemId);
    }
}