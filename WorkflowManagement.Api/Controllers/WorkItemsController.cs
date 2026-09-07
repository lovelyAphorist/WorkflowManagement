using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WorkflowManagement.Application.Common;
using WorkflowManagement.Application.Users.Constants;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Application.WorkItems.Enums;
using WorkflowManagement.Application.WorkItems.Services;
using System.Security.Claims;

namespace WorkflowManagement.Api.Controllers
{
    [ApiController]
    [Route("api/work-items")]
    [Authorize]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkItemService _service;
        private readonly IWorkItemCommentService _commentService;

        public WorkItemsController(IWorkItemService service, IWorkItemCommentService commentService)
        {
            _service = service;
            _commentService = commentService;
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<WorkItemResponse>> GetById(Guid id)
        {
            var workItem = await _service.GetByIdAsync(id);

            if (workItem is null)
            {
                return NotFound();
            }

            return Ok(workItem);
        }

        [HttpPost]
        public async Task<ActionResult<WorkItemResponse>> Create(
            CreateWorkItemRequest request)
        {
            var createdWorkItem = await _service.CreateAsync(request);

            return CreatedAtAction(
                nameof(GetById),
                new { id = createdWorkItem.Id },
                createdWorkItem);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<WorkItemResponse>>> GetAll([FromQuery] WorkItemQueryRequest query)
        {
            var result = await _service.GetAllAsync(query);

            return Ok(result);
        }
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<WorkItemResponse>> Update(Guid id,UpdateWorkItemRequest request)
        {
            var updatedWorkItem = await _service.UpdateAsync(id, request);

            if (updatedWorkItem is null)
            {
                return NotFound();
            }

            return Ok(updatedWorkItem);
        }

        [HttpDelete("{id:guid}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleted = await _service.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound();
            }

            return NoContent();
        }
        [HttpGet("{id:guid}/history")]
        public async Task<ActionResult<IReadOnlyList<WorkItemHistoryResponse>>> GetHistory(Guid id)
        {
            var history = await _service.GetHistoryAsync(id);

            if (history is null)
            {
                return NotFound();
            }

            return Ok(history);
        }
        [HttpPut("{id:guid}/assignee")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<WorkItemResponse>> Assign(Guid id, AssignWorkItemRequest request)
        {
            var result = await _service.AssignAsync(id, request);

            return result.Status switch
            {
                AssignWorkItemStatus.Success =>
                    Ok(result.WorkItem),

                AssignWorkItemStatus.WorkItemNotFound =>
                    NotFound(new
                    {
                        message = "Work item not found."
                    }),

                AssignWorkItemStatus.AssigneeNotFound =>
                    BadRequest(new
                    {
                        message = "Assignee not found."
                    }),

                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }

        [HttpGet("{id:guid}/comments")]
        public async Task<ActionResult<IReadOnlyList<WorkItemCommentResponse>>> GetComments(Guid id)
        {
            var comments = await _commentService.GetAllAsync(id);

            if (comments is null)
            {
                return NotFound();
            }

            return Ok(comments);
        }

        [HttpPost("{id:guid}/comments")]
        public async Task<ActionResult<WorkItemCommentResponse>> CreateComment(
            Guid id,
            CreateWorkItemCommentRequest request)
        {
            var userIdValue =
                User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            var comment = await _commentService.CreateAsync(
                id,
                userId,
                request);

            if (comment is null)
            {
                return NotFound();
            }

            return StatusCode(
                StatusCodes.Status201Created,
                comment);
        }
    }
}