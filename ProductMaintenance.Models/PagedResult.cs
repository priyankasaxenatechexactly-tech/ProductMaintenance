using System.Collections.Generic;

namespace ProductMaintenance.Models
{
    public class PagedResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => PageSize == 0 ? 0 : (int)System.Math.Ceiling((double)TotalCount / PageSize);
        public string? Query { get; set; }
    }
}
