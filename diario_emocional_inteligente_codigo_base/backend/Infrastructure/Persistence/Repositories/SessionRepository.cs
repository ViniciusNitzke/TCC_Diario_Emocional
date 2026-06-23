using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;

namespace DiarioEmocional.Api.Infrastructure.Persistence.Repositories;

public sealed class SessionRepository(JsonDiarioContext context) : ISessionRepository
{
    public async Task<Session> CreateAsync(Guid usuarioId)
    {
        context.Sessions.RemoveAll(session => session.UsuarioId == usuarioId || session.ExpiraEm <= DateTimeOffset.UtcNow);
        var session = JsonDiarioContext.NewSession(usuarioId);
        context.Sessions.Add(session);
        await context.SaveAsync();
        return session;
    }

    public Task<Session?> FindByTokenAsync(string token) =>
        Task.FromResult(context.Sessions.FirstOrDefault(session => session.Token == token));

    public async Task DeleteAsync(string token)
    {
        context.Sessions.RemoveAll(session => session.Token == token);
        await context.SaveAsync();
    }
}
