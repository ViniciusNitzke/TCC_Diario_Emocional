using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;

namespace DiarioEmocional.Api.Infrastructure.Persistence.Repositories;

public sealed class EmotionalEntryRepository(JsonDiarioContext context) : IEmotionalEntryRepository
{
    public async Task AddAsync(EmotionalEntry entry)
    {
        context.Entries.Add(entry);
        await context.SaveAsync();
    }

    public Task<IReadOnlyList<EmotionalEntry>> ListByUserAsync(Guid usuarioId, int take = 200)
    {
        var entries = context.Entries
            .Where(entry => entry.UsuarioId == usuarioId)
            .OrderByDescending(entry => entry.CriadoEm)
            .Take(take)
            .ToArray();

        return Task.FromResult<IReadOnlyList<EmotionalEntry>>(entries);
    }

    public async Task<int> DeleteAsync(Guid usuarioId, Guid entryId)
    {
        var removed = context.Entries.RemoveAll(entry => entry.Id == entryId && entry.UsuarioId == usuarioId);
        if (removed > 0) await context.SaveAsync();
        return removed;
    }

    public async Task<int> DeleteAllByUserAsync(Guid usuarioId)
    {
        var removed = context.Entries.RemoveAll(entry => entry.UsuarioId == usuarioId);
        await context.SaveAsync();
        return removed;
    }
}
