using System.Xml.Linq;
using WorkflowManagement.Application.Users.Dtos;
using WorkflowManagement.Application.Users.Services;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Application.WorkItems.Enums;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Application.WorkItems.Services
{
    public class WorkItemCommentService : IWorkItemCommentService
    {
        private readonly IWorkItemRepository _workItemRepository;
        private readonly IWorkItemCommentRepository _commentRepository;
        private readonly IUserService _userService;

        public WorkItemCommentService(
            IWorkItemRepository workItemRepository,
            IWorkItemCommentRepository commentRepository,
            IUserService userService)
        {
            _workItemRepository = workItemRepository;
            _commentRepository = commentRepository;
            _userService = userService;
        }
        public async Task<WorkItemCommentResponse?> CreateAsync(Guid workItemId, Guid authorId, CreateWorkItemCommentRequest request)
        {
            var workItem =
                await _workItemRepository.GetByIdAsync(workItemId);

            if (workItem is null)
            {
                return null;
            }

            var author = await _userService.GetByIdAsync(authorId);

            if (author is null)
            {
                return null;
            }

            var comment = new WorkItemComment
            {
                Id = Guid.NewGuid(),
                WorkItemId = workItemId,
                AuthorId = authorId,
                Body = request.Body.Trim(),
                CreatedAtUtc = DateTime.UtcNow
            };

            var createdComment =
                await _commentRepository.AddAsync(comment);

            return MapToResponse(createdComment, author);
        }
        private static WorkItemCommentResponse MapToResponse(WorkItemComment comment, UserResponse author)
        {
            return new WorkItemCommentResponse
            {
                Id = comment.Id,
                Body = comment.Body,
                CreatedAtUtc = comment.CreatedAtUtc,
                EditedAtUtc = comment.EditedAtUtc,
                Author = new UserSummaryResponse
                {
                    Id = author.Id,
                    DisplayName = author.DisplayName
                }
            };
        }
        private async Task<WorkItemCommentResponse>MapToResponseWithAuthorAsync(WorkItemComment comment)
        {
            var author =
                await _userService.GetByIdAsync(comment.AuthorId);

            if (author is null)
            {
                throw new InvalidOperationException(
                    "Comment author could not be found.");
            }

            return MapToResponse(comment, author);
        }
        public async Task<IReadOnlyList<WorkItemCommentResponse>?> GetAllAsync(Guid workItemId)
        {
            var workItem =
                await _workItemRepository.GetByIdAsync(workItemId);

            if (workItem is null)
            {
                return null;
            }

            var comments =
                await _commentRepository.GetByWorkItemIdAsync(workItemId);
            
            var authorIds = comments
                .Select(c => c.AuthorId)
                 .Distinct()
                 .ToList();

            var authors =
                await _userService.GetByIdsAsync(authorIds);

            var authorLookup =
                authors.ToDictionary(u => u.Id);

            var results = new List<WorkItemCommentResponse>();

            foreach (var comment in comments)
            {
                if (authorLookup.TryGetValue(comment.AuthorId, out var author))
                {
                    results.Add(MapToResponse(comment, author));
                }
            }

            return results;
        }
        public async Task<WorkItemCommentOperationResult> UpdateAsync(Guid workItemId, Guid commentId, Guid userId, UpdateWorkItemCommentRequest request)
        {
            var comment =
                await _commentRepository.GetByIdAsync(commentId);

            if (comment is null || comment.WorkItemId != workItemId)
            {
                return new WorkItemCommentOperationResult
                {
                    Status = WorkItemCommentOperationStatus.NotFound
                };
            }

            if (comment.AuthorId != userId)
            {
                return new WorkItemCommentOperationResult
                {
                    Status = WorkItemCommentOperationStatus.Forbidden
                };
            }

            var newBody = request.Body.Trim();

            if (comment.Body == newBody)
            {
                var existingResponse =
                    await MapToResponseWithAuthorAsync(comment);

                return new WorkItemCommentOperationResult
                {
                    Status = WorkItemCommentOperationStatus.Success,
                    Comment = existingResponse
                };
            }

            comment.Body = newBody;
            comment.EditedAtUtc = DateTime.UtcNow;

            var updatedComment =
                await _commentRepository.UpdateAsync(comment);

            return new WorkItemCommentOperationResult
            {
                Status = WorkItemCommentOperationStatus.Success,
                Comment = await MapToResponseWithAuthorAsync(
                    updatedComment)
            };
        }
        public async Task<WorkItemCommentOperationResult> DeleteAsync(Guid workItemId, Guid commentId, Guid userId, bool isAdmin)
        {
            var comment =
                await _commentRepository.GetByIdAsync(commentId);

            if (comment is null || comment.WorkItemId != workItemId)
            {
                return new WorkItemCommentOperationResult
                {
                    Status = WorkItemCommentOperationStatus.NotFound
                };
            }

            if (comment.AuthorId != userId && !isAdmin)
            {
                return new WorkItemCommentOperationResult
                {
                    Status = WorkItemCommentOperationStatus.Forbidden
                };
            }

            await _commentRepository.DeleteAsync(comment);

            return new WorkItemCommentOperationResult
            {
                Status = WorkItemCommentOperationStatus.Success
            };
        }
    }
}