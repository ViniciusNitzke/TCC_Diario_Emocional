using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;

namespace DiarioEmocional.Api.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(JsonDiarioContext context) : IUserRepository
{
    public async Task AddAsync(User user)
    {
        context.Users.Add(user);
        await context.SaveAsync();
    }

    public Task<User?> FindByEmailAsync(string email) =>
        Task.FromResult(context.Users.FirstOrDefault(user => user.Email == email));

    public Task<User?> FindByIdAsync(Guid id) =>
        Task.FromResult(context.Users.FirstOrDefault(user => user.Id == id));

    public async Task UpdateAsync(User user)
    {
        var index = context.Users.FindIndex(current => current.Id == user.Id);
        if (index >= 0)
        {
            context.Users[index] = user;
            await context.SaveAsync();
        }
    }
}
