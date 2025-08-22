using System.Threading.Tasks;
using ProductMaintenance.Models;

namespace ProductMaintenance.Business.Interfaces
{
    public interface IUserProcess
    {
        // Auth
        Task<UserItem?> UserLogin(string email, string password);

        // User management
        Task<PagedResult<UserListItem>> SearchAsync(string? query, int page, int pageSize, int? excludeUserId = null);
        Task<UserUpsertModel> GetForCreateAsync();
        Task<UserUpsertModel?> GetForEditAsync(int id);
        Task<(bool Ok, string? Error)> CreateAsync(UserUpsertModel model);
        Task<(bool Ok, string? Error)> UpdateAsync(UserUpsertModel model);
        Task<(bool Ok, string? Error)> DeleteAsync(int id);
    }
}
