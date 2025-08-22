using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProductMaintenance.DataAccess;
using ProductMaintenance.Models;

namespace ProductMaintenance.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _db;

        public DashboardController(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var totalUsers = await _db.Users.CountAsync();
            var totalProducts = await _db.Products.CountAsync();
            var lowStock = await _db.Products.CountAsync(p => p.StockQuantity < 10);

            // Default categories to always display (even when count is 0)
            var defaultCategories = new List<string> { "Electronics", "Clothing", "Groceries", "Books", "Furniture", "Sports" };

            var grouped = await _db.Products
                .GroupBy(p => p.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToListAsync();

            // Merge defaults with any additional categories present in data
            var allCategories = defaultCategories
                .Union(grouped.Select(x => x.Category ?? "Uncategorized"), StringComparer.OrdinalIgnoreCase)
                .ToList();

            var map = grouped
                .GroupBy(x => x.Category ?? "Uncategorized", StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Count), StringComparer.OrdinalIgnoreCase);

            var categories = allCategories;
            var counts = categories.Select(c => map.TryGetValue(c, out var n) ? n : 0).ToList();

            var model = new DashboardViewModel
            {
                TotalUsers = totalUsers,
                TotalProducts = totalProducts,
                LowStockCount = lowStock,
                Categories = categories,
                CategoryCounts = counts
            };

            return View(model);
        }
    }
}

