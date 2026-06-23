namespace DiarioEmocional.Api.Domain.Entities;

public sealed record EmotionalEntry(
    Guid Id,
    Guid UsuarioId,
    string EmocaoId,
    string EmocaoNome,
    string Familia,
    int Intensidade,
    string[] Gatilhos,
    string Situacao,
    string Observacao,
    string EstrategiaAutocuidado,
    string Origem,
    DateTimeOffset CriadoEm);
