using System;
using System.Collections.Generic;
using System.Text;

namespace WorkflowManagement.Application.Common
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
