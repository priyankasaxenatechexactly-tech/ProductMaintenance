using ProductMaintenance.Models;

namespace ProductMaintenance.Business.Interfaces
{
    public interface IProductProcess
    {
        Task<PagedResult<ProductListItem>> SearchAsync(string? query, int page, int pageSize, string? sortField, string? sortDir);
        Task<ProductUpsertModel> GetForCreateAsync();
        Task<ProductUpsertModel?> GetForEditAsync(int id);
        Task<(bool Ok, string? Error)> CreateAsync(ProductUpsertModel model, string? imageUrl);
        Task<(bool Ok, string? Error)> UpdateAsync(ProductUpsertModel model, string? imageUrl);
        Task<(bool Ok, string? Error)> DeleteAsync(int id);
    }
}
