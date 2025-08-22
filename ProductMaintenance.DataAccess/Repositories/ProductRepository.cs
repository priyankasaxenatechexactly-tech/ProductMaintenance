using Microsoft.EntityFrameworkCore;
using ProductMaintenance.DataAccess;
using ProductMaintenance.DataAccess.Interfaces;
using ProductMaintenance.Entity;
using System;

namespace ProductMaintenance.DataAccess.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<(IEnumerable<Product> Items, int TotalCount)> SearchAsync(string? query, int page, int pageSize, string? sortField, string? sortDir)
        {
            var q = _db.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(query))
            {
                var term = query.Trim();
                q = q.Where(p => p.Name.Contains(term) || p.Category.Contains(term));
            }

            var total = await q.CountAsync();

            var descending = string.Equals(sortDir, "desc", StringComparison.OrdinalIgnoreCase);
            // Default sorting by CreatedDate desc when no explicit sortField provided
            if (string.IsNullOrWhiteSpace(sortField))
            {
                q = q.OrderByDescending(p => p.CreatedDate);
            }
            else
            {
                q = (sortField?.ToLower()) switch
                {
                    "category" => descending ? q.OrderByDescending(p => p.Category) : q.OrderBy(p => p.Category),
                    "price" => descending ? q.OrderByDescending(p => p.Price) : q.OrderBy(p => p.Price),
                    "stockquantity" => descending ? q.OrderByDescending(p => p.StockQuantity) : q.OrderBy(p => p.StockQuantity),
                    "name" => descending ? q.OrderByDescending(p => p.Name) : q.OrderBy(p => p.Name),
                    _ => q.OrderByDescending(p => p.CreatedDate)
                };
            }

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return (items, total);
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _db.Products.FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task AddAsync(Product entity)
        {
            entity.CreatedDate = DateTime.UtcNow;
            _db.Products.Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product entity)
        {
            entity.UpdatedDate = DateTime.UtcNow;
            _db.Products.Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var prod = await _db.Products.FindAsync(id);
            if (prod != null)
            {
                _db.Products.Remove(prod);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
        {
            var query = _db.Products.AsQueryable();
            query = query.Where(x => x.Name == name);
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }
            return await query.AnyAsync();
        }
    }
}
