using System.Security.Cryptography;
using System.Text.Json;
using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Infrastructure.Persistence;

public sealed class JsonDiarioContext
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web) { WriteIndented = true };
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonDiarioContext(IWebHostEnvironment environment)
    {
        _filePath = Path.Combine(environment.ContentRootPath, "data", "diario-data.json");
        Load();
    }

    public List<User> Users { get; private set; } = new();
    public List<Session> Sessions { get; private set; } = new();
    public List<EmotionalEntry> Entries { get; private set; } = new();

    public async Task SaveAsync()
    {
        await _lock.WaitAsync();
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_filePath)!);
            var snapshot = new DiarioSnapshot(Users, Sessions, Entries);
            var json = JsonSerializer.Serialize(snapshot, _jsonOptions);
            await File.WriteAllTextAsync(_filePath, json);
        }
        finally
        {
            _lock.Release();
        }
    }

    public static Session NewSession(Guid userId) =>
        new(
            Convert.ToHexString(RandomNumberGenerator.GetBytes(32)).ToLowerInvariant(),
            userId,
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddDays(7));

    private void Load()
    {
        if (!File.Exists(_filePath)) return;

        var json = File.ReadAllText(_filePath);
        var snapshot = JsonSerializer.Deserialize<DiarioSnapshot>(json, _jsonOptions);
        if (snapshot is null) return;

        Users = snapshot.Users ?? new();
        Sessions = snapshot.Sessions ?? new();
        Entries = snapshot.Entries ?? new();
    }

    private sealed record DiarioSnapshot(
        List<User>? Users,
        List<Session>? Sessions,
        List<EmotionalEntry>? Entries);
}
