using Microsoft.EntityFrameworkCore;
using UserManagement.Domain.Entities;
using UserManagement.Domain.Interfaces;
using UserManagement.Infra.Context;

namespace UserManagement.Infra.Repositories;

public class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        await dbContext.Users.AddAsync(user);
    }

    public Task UpdateAsync(User user)
    {
        try
        {
            dbContext.Users.Update(user);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    public Task DeleteAsync(User user)
    {
        try
        {
            dbContext.Users.Remove(user);
            return Task.CompletedTask;
        }
        catch (Exception exception)
        {
            return Task.FromException(exception);
        }
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await dbContext.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetPagedAsync(int page, int pageSize)
    {
        return await dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> GetTotalCountAsync()
    {
        return await dbContext.Users.CountAsync();
    }
    
    public async Task<bool> EmailExistsAsync(string email, int? excludeUserId = null)
    {
        return await dbContext.Users
            .AnyAsync(u => u.Email == email && (!excludeUserId.HasValue || u.Id != excludeUserId.Value));
    }

    public async Task<bool> SaveChangesAsync()
    {
        return await dbContext.SaveChangesAsync() > 0;
    }
}