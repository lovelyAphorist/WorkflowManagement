using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Domain.Entities;
using WorkflowManagement.Infrastructure.Data;
using WorkflowManagement.Application.Common;

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
        public async Task<PagedResult<WorkItem>> GetAllAsync(
            WorkItemQueryRequest query)
        {
            var workItems = _context.WorkItems
                .AsNoTracking()
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.Trim();

                workItems = workItems.Where(w =>
                    w.Title.Contains(search) ||
                    (w.Description != null &&
                     w.Description.Contains(search)));
            }

            if (query.Status.HasValue)
            {
                workItems = workItems.Where(
                    w => w.Status == query.Status.Value);
            }

            if (query.Priority.HasValue)
            {
                workItems = workItems.Where(
                    w => w.Priority == query.Priority.Value);
            }
            var totalCount = await workItems.CountAsync();
            var items = await workItems
             .OrderByDescending(w => w.CreatedAtUtc)
             .Skip((query.Page - 1) * query.PageSize)
             .Take(query.PageSize)
              .ToListAsync();

            return new PagedResult<WorkItem>
            {
                Items = items,
                Page = query.Page,
                PageSize = query.PageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(
        totalCount / (double)query.PageSize)
            };

            /*            return await workItems
                            .OrderByDescending(w => w.CreatedAtUtc)
                            .ToListAsync();*/
        }
        public async Task<WorkItem> UpdateAsync(WorkItem workItem)
        {
            _context.WorkItems.Update(workItem);

            await _context.SaveChangesAsync();

            return workItem;
        }
        public async Task DeleteAsync(WorkItem workItem)
        {
            _context.WorkItems.Remove(workItem);

            await _context.SaveChangesAsync();
        }
    }
}

