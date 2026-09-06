using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Application.WorkItems.Dtos;
using WorkflowManagement.Application.WorkItems.Repositories;
using WorkflowManagement.Domain.Entities;
using WorkflowManagement.Infrastructure.Data;
using WorkflowManagement.Application.Common;
using WorkflowManagement.Application.WorkItems.Enums;

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

            workItems = (query.SortBy, query.SortDirection) switch
            {
                (WorkItemSortField.CreatedAt, SortDirection.Ascending) =>
                    workItems.OrderBy(w => w.CreatedAtUtc),

                (WorkItemSortField.CreatedAt, SortDirection.Descending) =>
                    workItems.OrderByDescending(w => w.CreatedAtUtc),

                (WorkItemSortField.UpdatedAt, SortDirection.Ascending) =>
                    workItems.OrderBy(w => w.UpdatedAtUtc),

                (WorkItemSortField.UpdatedAt, SortDirection.Descending) =>
                    workItems.OrderByDescending(w => w.UpdatedAtUtc),

                (WorkItemSortField.Priority, SortDirection.Ascending) =>
                    workItems.OrderBy(w => w.Priority),

                (WorkItemSortField.Priority, SortDirection.Descending) =>
                    workItems.OrderByDescending(w => w.Priority),

                (WorkItemSortField.Title, SortDirection.Ascending) =>
                    workItems.OrderBy(w => w.Title),

                (WorkItemSortField.Title, SortDirection.Descending) =>
                    workItems.OrderByDescending(w => w.Title),

                (WorkItemSortField.DueDate, SortDirection.Ascending) =>
                    workItems
                        .OrderBy(w => w.DueDate == null)
                        .ThenBy(w => w.DueDate),

                (WorkItemSortField.DueDate, SortDirection.Descending) =>
                    workItems
                        .OrderBy(w => w.DueDate == null)
                        .ThenByDescending(w => w.DueDate),

                _ => workItems.OrderByDescending(w => w.CreatedAtUtc)
            };

            var items = await workItems
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

