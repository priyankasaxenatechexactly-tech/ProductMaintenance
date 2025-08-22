using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ProductMaintenance.DataAccess.Interfaces;
using ProductMaintenance.Entity;
using System.Linq;
using System.Collections.Generic;

namespace ProductMaintenance.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(ApplicationDbContext db, ILogger<UserRepository> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email)) return null;
                var target = email.Trim().ToLower();

                return await _db.Users
                    .AsNoTracking()
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == target);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by email {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _db.Users
                    .AsNoTracking()
                    .Include(u => u.UserType)
                    .FirstOrDefaultAsync(u => u.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user by id {Id}", id);
                throw;
            }
        }

        public async Task<User> AddUserAsync(User user)
        {
            try
            {
                if (user is null) throw new ArgumentNullException(nameof(user));
                user.CreatedDate = DateTime.UtcNow;
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while adding user with email {Email}", user?.Email);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while adding user with email {Email}", user?.Email);
                throw;
            }
        }

        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                if (user is null) throw new ArgumentNullException(nameof(user));
                user.UpdatedDate = DateTime.UtcNow;

                // Attach and mark individual properties as modified to ensure changes persist
                _db.Attach(user);
                var entry = _db.Entry(user);
                entry.Property(u => u.Name).IsModified = true;
                entry.Property(u => u.LastName).IsModified = true;
                entry.Property(u => u.Email).IsModified = true;
                entry.Property(u => u.MobileNo).IsModified = true;
                entry.Property(u => u.UserTypeId).IsModified = true;
                entry.Property(u => u.PasswordHash).IsModified = true; // may or may not have changed
                entry.Property(u => u.UpdatedDate).IsModified = true;

                await _db.SaveChangesAsync();
                return user;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while updating user {Id}", user?.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating user {Id}", user?.Id);
                throw;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var entity = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (entity == null) return false;
                _db.Users.Remove(entity);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Database update error while deleting user {Id}", id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting user {Id}", id);
                throw;
            }
        }

        public async Task<(IEnumerable<User> Items, int TotalCount)> SearchAsync(string? query, int page, int pageSize, int? excludeUserId = null)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1) pageSize = 10;
                var q = _db.Users.AsNoTracking().Include(u => u.UserType).AsQueryable();
                if (excludeUserId.HasValue)
                {
                    var exId = excludeUserId.Value;
                    q = q.Where(u => u.Id != exId);
                }
                if (!string.IsNullOrWhiteSpace(query))
                {
                    var term = query.Trim().ToLower();
                    q = q.Where(u => u.Name.ToLower().Contains(term)
                                   || (u.LastName != null && u.LastName.ToLower().Contains(term))
                                   || u.Email.ToLower().Contains(term));
                }
                var total = await q.CountAsync();
                var items = await q.OrderByDescending(u => u.CreatedDate)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
                return (items, total);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching users");
                throw;
            }
        }

        public async Task<List<UserType>> GetUserTypesAsync()
        {
            try
            {
                return await _db.UserTypes.AsNoTracking().OrderBy(t => t.Name).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching user types");
                throw;
            }
        }
    }
}
