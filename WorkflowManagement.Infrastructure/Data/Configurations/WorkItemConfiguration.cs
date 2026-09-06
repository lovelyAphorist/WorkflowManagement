using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowManagement.Domain.Entities;

namespace WorkflowManagement.Infrastructure.Data.Configurations
{
    public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
    {
        public void Configure(EntityTypeBuilder<WorkItem> builder)
        {
            builder.HasKey(w => w.Id);

            builder.Property(w => w.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(w => w.Description)
                .HasMaxLength(2000);
        }
    }
}