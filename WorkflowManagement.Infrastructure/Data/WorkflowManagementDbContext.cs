using Microsoft.EntityFrameworkCore;
using WorkflowManagement.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using WorkflowManagement.Infrastructure.Identity;

namespace WorkflowManagement.Infrastructure.Data
{
    public class WorkflowManagementDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public WorkflowManagementDbContext(DbContextOptions<WorkflowManagementDbContext> options) : base(options)
        {
        }

        public DbSet<WorkItem> WorkItems => Set<WorkItem>();
        public DbSet<WorkItemHistory> WorkItemHistory => Set<WorkItemHistory>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(WorkflowManagementDbContext).Assembly);
        }
    }
}