using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Domain.Entities;
using WorkflowManagement.Infrastructure.Data;

namespace WorkflowManagement.Infrastructure.Repositories
{
    public class WorkItemCommentRepository
        : IWorkItemCommentRepository
    {
        private readonly WorkflowManagementDbContext _context;

        public WorkItemCommentRepository(
            WorkflowManagementDbContext context)
        {
            _context = context;
        }

        public async Task<WorkItemComment> AddAsync(
            WorkItemComment comment)
        {
            _context.WorkItemComments.Add(comment);

            await _context.SaveChangesAsync();

            return comment;
        }

        public async Task<IReadOnlyList<WorkItemComment>>
            GetByWorkItemIdAsync(Guid workItemId)
        {
            return await _context.WorkItemComments
                .AsNoTracking()
                .Where(c => c.WorkItemId == workItemId)
                .OrderBy(c => c.CreatedAtUtc)
                .ToListAsync();
        }
    }
}