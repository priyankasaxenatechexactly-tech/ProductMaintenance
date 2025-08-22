using ProductMaintenance.Entity;

namespace ProductMaintenance.DataAccess.Interfaces
{
    public interface IProductRepository
    {
        Task<(IEnumerable<Product> Items, int TotalCount)> SearchAsync(string? query, int page, int pageSize, string? sortField, string? sortDir);
        Task<Product?> GetByIdAsync(int id);
        Task AddAsync(Product entity);
        Task UpdateAsync(Product entity);
        Task DeleteAsync(int id);
        Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
    }
}
