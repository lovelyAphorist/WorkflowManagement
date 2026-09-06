using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManagement.Application.WorkItems.Dtos
{
    public class AssignWorkItemRequest
    {
        public Guid? AssigneeId { get; set; }
    }
}