using System.Threading.Tasks;
using ProductMaintenance.Entity;
using System.Collections.Generic;

namespace ProductMaintenance.DataAccess.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(int id);
        Task<User> AddUserAsync(User user);
        Task<User> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int id);
        Task<(IEnumerable<User> Items, int TotalCount)> SearchAsync(string? query, int page, int pageSize, int? excludeUserId = null);
        Task<List<UserType>> GetUserTypesAsync();
    }
}
