namespace WorkflowManagement.Domain.Entities
{
    public class WorkItemComment
    {
        public Guid Id { get; set; }
        public Guid WorkItemId { get; set; }
        public Guid AuthorId { get; set; }
        public string Body { get; set; } = string.Empty;
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? EditedAtUtc { get; set; }
        public WorkItem WorkItem { get; set; } = null!;
    }
}