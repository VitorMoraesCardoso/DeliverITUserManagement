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
    
    //Caso a validacao de e-mail unico n fosse feita pelo banco, poderia adicionar
    //um metodo para verificar se o email ja existe
    //Task<bool> EmailExistsAsync(string email);
}