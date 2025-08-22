using ProductMaintenance.Business.Interfaces;
using ProductMaintenance.DataAccess.Interfaces;
using ProductMaintenance.Entity;
using ProductMaintenance.Models;
using System.Linq;
using EntityProduct = ProductMaintenance.Entity.Product;

namespace ProductMaintenance.Business.Services
{
    public class ProductProcess : IProductProcess
    {
        private readonly IProductRepository _repo;
        public ProductProcess(IProductRepository repo)
        {
            _repo = repo;
        }

        public async Task<PagedResult<ProductListItem>> SearchAsync(string? query, int page, int pageSize, string? sortField, string? sortDir)
        {
            var result = await _repo.SearchAsync(query, page, pageSize, sortField, sortDir);
            var items = result.Items;
            var total = result.TotalCount;
            return new PagedResult<ProductListItem>
            {
                Items = items.Select(p => new ProductListItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    Category = p.Category,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl
                }).ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Query = query
            };
        }

        public Task<ProductUpsertModel> GetForCreateAsync()
        {
            var model = new ProductUpsertModel();
            return Task.FromResult(model);
        }

        public async Task<ProductUpsertModel?> GetForEditAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return new ProductUpsertModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Category = entity.Category,
                Price = entity.Price,
                StockQuantity = entity.StockQuantity,
                Description = entity.Description,
                ImageUrl = entity.ImageUrl
            };
        }

        public async Task<(bool Ok, string? Error)> CreateAsync(ProductUpsertModel model, string? imageUrl)
        {
            if (await _repo.ExistsByNameAsync(model.Name))
                return (false, "A product with this name already exists.");

            var entity = new EntityProduct
            {
                Name = model.Name.Trim(),
                Category = model.Category.Trim(),
                Price = model.Price,
                StockQuantity = model.StockQuantity,
                Description = model.Description?.Trim(),
                ImageUrl = imageUrl ?? model.ImageUrl
            };

            await _repo.AddAsync(entity);
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> UpdateAsync(ProductUpsertModel model, string? imageUrl)
        {
            var existing = await _repo.GetByIdAsync(model.Id);
            if (existing == null) return (false, "Product not found.");

            if (await _repo.ExistsByNameAsync(model.Name, model.Id))
                return (false, "A product with this name already exists.");

            existing.Name = model.Name.Trim();
            existing.Category = model.Category.Trim();
            existing.Price = model.Price;
            existing.StockQuantity = model.StockQuantity;
            existing.Description = model.Description?.Trim();
            existing.ImageUrl = imageUrl ?? model.ImageUrl ?? existing.ImageUrl;

            await _repo.UpdateAsync(existing);
            return (true, null);
        }

        public async Task<(bool Ok, string? Error)> DeleteAsync(int id)
        {
            await _repo.DeleteAsync(id);
            return (true, null);
        }
    }
}
