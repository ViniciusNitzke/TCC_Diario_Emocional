using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;
using System.Security.Cryptography;

namespace DiarioEmocional.Api.Infrastructure.Firebase.Repositories;

public sealed class FirestoreSessionRepository(FirestoreContext context) : ISessionRepository
{
    private Google.Cloud.Firestore.CollectionReference Sessions => context.Database.Collection("sessoes");

    public async Task<Session> CreateAsync(Guid usuarioId)
    {
        var oldSessions = await Sessions.WhereEqualTo("usuarioId", usuarioId.ToString()).GetSnapshotAsync();
        var batch = context.Database.StartBatch();
        foreach (var document in oldSessions.Documents)
            batch.Delete(document.Reference);

        var session = NewSession(usuarioId);
        batch.Set(Sessions.Document(session.Token), FirestoreMapper.ToDocument(session));
        await batch.CommitAsync();

        return session;
    }

    public async Task<Session?> FindByTokenAsync(string token)
    {
        var snapshot = await Sessions.Document(token).GetSnapshotAsync();
        return snapshot.Exists ? FirestoreMapper.ToSession(snapshot) : null;
    }

    public Task DeleteAsync(string token) =>
        Sessions.Document(token).DeleteAsync();

    private static Session NewSession(Guid usuarioId) =>
        new(
            Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant(),
            usuarioId,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(7));
}
