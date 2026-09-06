using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Infrastructure.Data.Configurations
{
    public class WorkItemHistoryConfiguration
        : IEntityTypeConfiguration<WorkItemHistory>
    {
        public void Configure(
            EntityTypeBuilder<WorkItemHistory> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.OldValue)
                .HasMaxLength(2000);

            builder.Property(h => h.NewValue)
                .HasMaxLength(2000);

            builder.HasOne(h => h.WorkItem)
                .WithMany(w => w.History)
                .HasForeignKey(h => h.WorkItemId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}