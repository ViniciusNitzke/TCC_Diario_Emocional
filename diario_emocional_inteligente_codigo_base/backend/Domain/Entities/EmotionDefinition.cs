namespace DiarioEmocional.Api.Domain.Entities;

public sealed record EmotionDefinition(
    string Id,
    string Nome,
    string Familia,
    string Cor,
    string Descricao);
