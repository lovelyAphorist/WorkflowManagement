using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Infrastructure.Data
{
    public class WorkflowManagementDbContext : DbContext
    {
        public WorkflowManagementDbContext(
            DbContextOptions<WorkflowManagementDbContext> options)
            : base(options)
        {
        }

        public DbSet<WorkItem> WorkItems => Set<WorkItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(WorkflowManagementDbContext).Assembly);
        }
    }
}