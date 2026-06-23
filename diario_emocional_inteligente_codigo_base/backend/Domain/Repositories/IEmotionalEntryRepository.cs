using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Domain.Repositories;

public interface IEmotionalEntryRepository
{
    Task AddAsync(EmotionalEntry entry);
    Task<IReadOnlyList<EmotionalEntry>> ListByUserAsync(Guid usuarioId, int take = 200);
    Task<int> DeleteAsync(Guid usuarioId, Guid entryId);
    Task<int> DeleteAllByUserAsync(Guid usuarioId);
}
