using DiarioEmocional.Api.Domain.Entities;
using Google.Cloud.Firestore;

namespace DiarioEmocional.Api.Infrastructure.Firebase;

public static class FirestoreMapper
{
    public static Dictionary<string, object> ToDocument(User user) => new()
    {
        ["id"] = user.Id.ToString(),
        ["nome"] = user.Nome,
        ["email"] = user.Email,
        ["senhaHash"] = user.SenhaHash,
        ["senhaSalt"] = user.SenhaSalt,
        ["criadoEm"] = user.CriadoEm.UtcDateTime,
        ["privacidade"] = new Dictionary<string, object>
        {
            ["bloqueioLocal"] = user.Privacidade.BloqueioLocal,
            ["permitirRelatorios"] = user.Privacidade.PermitirRelatorios,
            ["consentimentoDadosSensiveis"] = user.Privacidade.ConsentimentoDadosSensiveis
        },
        ["esm"] = new Dictionary<string, object>
        {
            ["ativo"] = user.Esm.Ativo,
            ["horarios"] = user.Esm.Horarios,
            ["frequencia"] = user.Esm.Frequencia
        }
    };

    public static User ToUser(DocumentSnapshot snapshot)
    {
        var data = snapshot.ToDictionary();
        var privacy = ReadMap(data, "privacidade");
        var esm = ReadMap(data, "esm");

        return new User(
            Guid.Parse(ReadString(data, "id", snapshot.Id)),
            ReadString(data, "nome"),
            ReadString(data, "email"),
            ReadString(data, "senhaHash"),
            ReadString(data, "senhaSalt"),
            ReadDate(data, "criadoEm"),
            new PrivacySettings(
                ReadBool(privacy, "bloqueioLocal", true),
                ReadBool(privacy, "permitirRelatorios", true),
                ReadBool(privacy, "consentimentoDadosSensiveis", true)),
            new EsmSettings(
                ReadBool(esm, "ativo", true),
                ReadStringArray(esm, "horarios", new[] { "09:00", "14:00", "20:00" }),
                ReadString(esm, "frequencia", "todos_os_dias")));
    }

    public static Dictionary<string, object> ToDocument(Session session) => new()
    {
        ["token"] = session.Token,
        ["usuarioId"] = session.UsuarioId.ToString(),
        ["criadoEm"] = session.CriadoEm.UtcDateTime,
        ["expiraEm"] = session.ExpiraEm.UtcDateTime
    };

    public static Session ToSession(DocumentSnapshot snapshot)
    {
        var data = snapshot.ToDictionary();
        return new Session(
            ReadString(data, "token", snapshot.Id),
            Guid.Parse(ReadString(data, "usuarioId")),
            ReadDate(data, "criadoEm"),
            ReadDate(data, "expiraEm"));
    }

    public static Dictionary<string, object> ToDocument(EmotionalEntry entry) => new()
    {
        ["id"] = entry.Id.ToString(),
        ["usuarioId"] = entry.UsuarioId.ToString(),
        ["emocaoId"] = entry.EmocaoId,
        ["emocaoNome"] = entry.EmocaoNome,
        ["familia"] = entry.Familia,
        ["intensidade"] = entry.Intensidade,
        ["gatilhos"] = entry.Gatilhos,
        ["situacao"] = entry.Situacao,
        ["observacao"] = entry.Observacao,
        ["estrategiaAutocuidado"] = entry.EstrategiaAutocuidado,
        ["origem"] = entry.Origem,
        ["criadoEm"] = entry.CriadoEm.UtcDateTime
    };

    public static EmotionalEntry ToEntry(DocumentSnapshot snapshot)
    {
        var data = snapshot.ToDictionary();
        return new EmotionalEntry(
            Guid.Parse(ReadString(data, "id", snapshot.Id)),
            Guid.Parse(ReadString(data, "usuarioId")),
            ReadString(data, "emocaoId"),
            ReadString(data, "emocaoNome"),
            ReadString(data, "familia"),
            ReadInt(data, "intensidade"),
            ReadStringArray(data, "gatilhos", Array.Empty<string>()),
            ReadString(data, "situacao"),
            ReadString(data, "observacao"),
            ReadString(data, "estrategiaAutocuidado"),
            ReadString(data, "origem", "espontaneo"),
            ReadDate(data, "criadoEm"));
    }

    private static Dictionary<string, object> ReadMap(Dictionary<string, object> data, string key) =>
        data.TryGetValue(key, out var value) && value is Dictionary<string, object> map
            ? map
            : new Dictionary<string, object>();

    private static string ReadString(Dictionary<string, object> data, string key, string fallback = "") =>
        data.TryGetValue(key, out var value) ? Convert.ToString(value) ?? fallback : fallback;

    private static bool ReadBool(Dictionary<string, object> data, string key, bool fallback = false) =>
        data.TryGetValue(key, out var value) ? Convert.ToBoolean(value) : fallback;

    private static int ReadInt(Dictionary<string, object> data, string key, int fallback = 0) =>
        data.TryGetValue(key, out var value) ? Convert.ToInt32(value) : fallback;

    private static DateTimeOffset ReadDate(Dictionary<string, object> data, string key)
    {
        if (!data.TryGetValue(key, out var value)) return DateTimeOffset.UtcNow;
        return value switch
        {
            Timestamp timestamp => new DateTimeOffset(timestamp.ToDateTime(), TimeSpan.Zero),
            DateTime dateTime => new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc)),
            DateTimeOffset dateTimeOffset => dateTimeOffset,
            string text when DateTimeOffset.TryParse(text, out var parsed) => parsed,
            _ => DateTimeOffset.UtcNow
        };
    }

    private static string[] ReadStringArray(Dictionary<string, object> data, string key, string[] fallback)
    {
        if (!data.TryGetValue(key, out var value)) return fallback;

        return value switch
        {
            string[] values => values,
            IEnumerable<string> values => values.ToArray(),
            IEnumerable<object> values => values.Select(item => Convert.ToString(item) ?? "").Where(item => item.Length > 0).ToArray(),
            _ => fallback
        };
    }
}
