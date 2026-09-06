using Microsoft.AspNetCore.Mvc;
using WorkflowManagement.Application.Common;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Application.WorkItems.Services;
using Microsoft.AspNetCore.Authorization;

namespace WorkflowManagement.Api.Controllers
{
    [ApiController]
    [Route("api/work-items")]
    [Authorize]
    public class WorkItemsController : ControllerBase
    {
        private readonly IWorkItemService _service;

        public WorkItemsController(IWorkItemService service)
        {
            _service = service;
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
    }
}