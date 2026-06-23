using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Domain.Repositories;

public interface ISessionRepository
{
    Task<Session> CreateAsync(Guid usuarioId);
    Task<Session?> FindByTokenAsync(string token);
    Task DeleteAsync(string token);
}
