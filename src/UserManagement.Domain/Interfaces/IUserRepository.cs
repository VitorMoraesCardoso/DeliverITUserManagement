using UserManagement.Domain.Entities;

namespace UserManagement.Domain.Interfaces;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(User user);
    Task<User?> GetByIdAsync(int id);
    Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize);
    Task<int> GetTotalCountAsync();
    Task<bool> EmailExistsAsync(string email, int? excludeUserId = null);
    Task<bool> SaveChangesAsync();
}