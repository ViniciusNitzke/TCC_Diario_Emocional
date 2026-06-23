using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;
using Google.Cloud.Firestore;

namespace DiarioEmocional.Api.Infrastructure.Firebase.Repositories;

public sealed class FirestoreEmotionalEntryRepository(FirestoreContext context) : IEmotionalEntryRepository
{
    private CollectionReference Entries => context.Database.Collection("registrosEmocionais");

    public Task AddAsync(EmotionalEntry entry) =>
        Entries.Document(entry.Id.ToString()).SetAsync(FirestoreMapper.ToDocument(entry));

    public async Task<IReadOnlyList<EmotionalEntry>> ListByUserAsync(Guid usuarioId, int take = 200)
    {
        var snapshot = await Entries
            .WhereEqualTo("usuarioId", usuarioId.ToString())
            .OrderByDescending("criadoEm")
            .Limit(take)
            .GetSnapshotAsync();

        return snapshot.Documents
            .Where(document => document.Exists)
            .Select(FirestoreMapper.ToEntry)
            .ToArray();
    }

    public async Task<int> DeleteAsync(Guid usuarioId, Guid entryId)
    {
        var document = Entries.Document(entryId.ToString());
        var snapshot = await document.GetSnapshotAsync();
        if (!snapshot.Exists) return 0;

        var entry = FirestoreMapper.ToEntry(snapshot);
        if (entry.UsuarioId != usuarioId) return 0;

        await document.DeleteAsync();
        return 1;
    }

    public async Task<int> DeleteAllByUserAsync(Guid usuarioId)
    {
        var snapshot = await Entries.WhereEqualTo("usuarioId", usuarioId.ToString()).GetSnapshotAsync();
        if (snapshot.Count == 0) return 0;

        var batch = context.Database.StartBatch();
        foreach (var document in snapshot.Documents)
            batch.Delete(document.Reference);

        await batch.CommitAsync();
        return snapshot.Count;
    }
}
