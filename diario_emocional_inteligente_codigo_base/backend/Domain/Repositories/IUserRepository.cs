using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Domain.Repositories;

public interface IUserRepository
{
    Task AddAsync(User user);
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByIdAsync(Guid id);
    Task UpdateAsync(User user);
}
