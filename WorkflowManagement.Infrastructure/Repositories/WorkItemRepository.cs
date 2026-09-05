using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Domain.Entities;
using WorkflowManagement.Infrastructure.Data;

namespace WorkflowManagement.Infrastructure.Repositories
{
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly WorkflowManagementDbContext _context;

        public WorkItemRepository(WorkflowManagementDbContext context)
        {
            _context = context;
        }
        public async Task<WorkItem> AddAsync(WorkItem workItem)
        {
            _context.WorkItems.Add(workItem);

            await _context.SaveChangesAsync();

            return workItem;
        }
        public async Task<WorkItem?> GetByIdAsync(Guid id)
        {
            return await _context.WorkItems.FindAsync(id);
        }
    }
}

