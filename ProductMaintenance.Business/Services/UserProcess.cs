using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using ProductMaintenance.Business.Interfaces;
using ProductMaintenance.DataAccess.Interfaces;
using ProductMaintenance.Models;
using System.Text;
using EntityUser = ProductMaintenance.Entity.User;

namespace ProductMaintenance.Business.Services
{
    public class UserProcess : IUserProcess
    {
        private readonly IUserRepository _repo;
        private readonly ILogger<UserProcess> _logger;

        public UserProcess(IUserRepository repo, ILogger<UserProcess> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // Login
        public async Task<UserItem?> UserLogin(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return null;

            email = email.Trim();
            password = password.Trim();

            var user = await _repo.GetByEmailAsync(email);
            if (user is null) return null;

            var incomingHash = ComputeSha256(password);
            if (!string.Equals(incomingHash, user.PasswordHash, StringComparison.Ordinal))
                return null;

            return new UserItem
            {
                Id = user.Id,
                Name = user.Name,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.UserType?.Name
            };
        }

        // Management operations
        public async Task<PagedResult<UserListItem>> SearchAsync(string? query, int page, int pageSize, int? excludeUserId = null)
        {
            var (items, total) = await _repo.SearchAsync(query, page, pageSize, excludeUserId);
            var list = items.Select(u => new UserListItem
            {
                Id = u.Id,
                Name = string.IsNullOrWhiteSpace(u.LastName) ? u.Name : $"{u.Name} {u.LastName}",
                Role = u.UserType?.Name ?? string.Empty,
                Email = u.Email,
                MobileNo = u.MobileNo
            });
            return new PagedResult<UserListItem>
            {
                Items = list.ToList(),
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Query = query
            };
        }

        public async Task<UserUpsertModel> GetForCreateAsync()
        {
            var roles = await _repo.GetUserTypesAsync();
            return new UserUpsertModel
            {
                Roles = roles.Select(r => new RoleItem { Id = r.Id, Name = r.Name }).ToList()
            };
        }

        public async Task<UserUpsertModel?> GetForEditAsync(int id)
        {
            var entity = await _repo.GetUserByIdAsync(id);
            if (entity == null) return null;
            var roles = await _repo.GetUserTypesAsync();
            return new UserUpsertModel
            {
                Id = entity.Id,
                Name = entity.Name,
                LastName = entity.LastName,
                Email = entity.Email,
                MobileNo = entity.MobileNo,
                UserTypeId = entity.UserTypeId,
                // Show masked placeholder so UI behaves like other fields without exposing real password
                Password = "********",
                Roles = roles.Select(r => new RoleItem { Id = r.Id, Name = r.Name }).ToList()
            };
        }

        public async Task<(bool Ok, string? Error)> CreateAsync(UserUpsertModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name) || string.IsNullOrWhiteSpace(model.Email) || string.IsNullOrWhiteSpace(model.Password))
                    return (false, "Name, Email and Password are required.");

                var existing = await _repo.GetByEmailAsync(model.Email);
                if (existing != null)
                    return (false, "Email is already in use.");

                var now = DateTime.UtcNow;
                var entity = new EntityUser
                {
                    Name = model.Name.Trim(),
                    LastName = string.IsNullOrWhiteSpace(model.LastName) ? null : model.LastName.Trim(),
                    Email = model.Email.Trim(),
                    MobileNo = string.IsNullOrWhiteSpace(model.MobileNo) ? null : model.MobileNo.Trim(),
                    UserTypeId = model.UserTypeId,
                    PasswordHash = ComputeSha256(model.Password!.Trim()),
                    CreatedDate = now,
                    UpdatedDate = now
                };
                await _repo.AddUserAsync(entity);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user {Email}", model?.Email);
                return (false, "Unexpected error while creating the user.");
            }
        }

        public async Task<(bool Ok, string? Error)> UpdateAsync(UserUpsertModel model)
        {
            try
            {
                var entity = await _repo.GetUserByIdAsync(model.Id);
                if (entity == null) return (false, "User not found.");

                if (!string.Equals(entity.Email, model.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var other = await _repo.GetByEmailAsync(model.Email);
                    if (other != null && other.Id != entity.Id)
                        return (false, "Email is already in use.");
                }

                entity.Name = model.Name.Trim();
                entity.LastName = string.IsNullOrWhiteSpace(model.LastName) ? null : model.LastName.Trim();
                entity.Email = model.Email.Trim();
                entity.MobileNo = string.IsNullOrWhiteSpace(model.MobileNo) ? null : model.MobileNo.Trim();
                entity.UserTypeId = model.UserTypeId;
                // Clear navigation to avoid EF using stale UserType from the AsNoTracking load
                entity.UserType = null;
                // If the password field contains the masked placeholder or is blank, keep existing password
                var masked = "********";
                if (!string.IsNullOrWhiteSpace(model.Password) && !string.Equals(model.Password, masked, StringComparison.Ordinal))
                {
                    entity.PasswordHash = ComputeSha256(model.Password.Trim());
                }
                entity.UpdatedDate = DateTime.UtcNow;

                await _repo.UpdateUserAsync(entity);
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user {Id}", model?.Id);
                return (false, "Unexpected error while updating the user.");
            }
        }

        public async Task<(bool Ok, string? Error)> DeleteAsync(int id)
        {
            try
            {
                var ok = await _repo.DeleteUserAsync(id);
                if (!ok) return (false, "User not found.");
                return (true, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {Id}", id);
                return (false, "Unexpected error while deleting the user.");
            }
        }

        private static string ComputeSha256(string input)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            var sb = new StringBuilder(hash.Length * 2);
            foreach (var b in hash)
            {
                sb.Append(b.ToString("X2")); // uppercase hex, no separators
            }
            return sb.ToString();
        }
    }
}
