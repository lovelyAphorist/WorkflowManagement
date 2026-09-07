using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WorkflowManagement.Domain.Entities;
using WorkflowManagement.Infrastructure.Identity;

namespace WorkflowManagement.Infrastructure.Data.Configurations
{
    public class WorkItemCommentConfiguration
        : IEntityTypeConfiguration<WorkItemComment>
    {
        public void Configure(
            EntityTypeBuilder<WorkItemComment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Body)
                .IsRequired()
                .HasMaxLength(2000);

            builder.HasOne(c => c.WorkItem)
                .WithMany(w => w.Comments)
                .HasForeignKey(c => c.WorkItemId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<ApplicationUser>()
                .WithMany()
                .HasForeignKey(c => c.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(c => new
            {
                c.WorkItemId,
                c.CreatedAtUtc
            });
        }
    }
}