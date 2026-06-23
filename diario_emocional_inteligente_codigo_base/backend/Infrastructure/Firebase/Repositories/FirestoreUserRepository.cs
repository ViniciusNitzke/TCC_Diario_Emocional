using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;
using Google.Cloud.Firestore;

namespace DiarioEmocional.Api.Infrastructure.Firebase.Repositories;

public sealed class FirestoreUserRepository(FirestoreContext context) : IUserRepository
{
    private CollectionReference Users => context.Database.Collection("usuarios");

    public Task AddAsync(User user) =>
        Users.Document(user.Id.ToString()).SetAsync(FirestoreMapper.ToDocument(user));

    public async Task<User?> FindByEmailAsync(string email)
    {
        var query = await Users.WhereEqualTo("email", email).Limit(1).GetSnapshotAsync();
        var snapshot = query.Documents.FirstOrDefault();
        return snapshot is null || !snapshot.Exists ? null : FirestoreMapper.ToUser(snapshot);
    }

    public async Task<User?> FindByIdAsync(Guid id)
    {
        var snapshot = await Users.Document(id.ToString()).GetSnapshotAsync();
        return snapshot.Exists ? FirestoreMapper.ToUser(snapshot) : null;
    }

    public Task UpdateAsync(User user) =>
        Users.Document(user.Id.ToString()).SetAsync(FirestoreMapper.ToDocument(user), SetOptions.Overwrite);
}
