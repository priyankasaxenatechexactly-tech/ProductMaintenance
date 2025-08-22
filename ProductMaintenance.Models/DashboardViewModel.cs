using System.Collections.Generic;

namespace ProductMaintenance.Models
{
    public class DashboardViewModel
    {
        public int TotalUsers { get; set; }
        public int TotalProducts { get; set; }
        public int LowStockCount { get; set; }

        public List<string> Categories { get; set; } = new();
        public List<int> CategoryCounts { get; set; } = new();
    }
}
