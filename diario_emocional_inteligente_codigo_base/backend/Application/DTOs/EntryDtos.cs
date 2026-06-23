namespace DiarioEmocional.Api.Application.DTOs;

public sealed record CreateEntryRequest(
    string EmocaoId,
    int Intensidade,
    string[] Gatilhos,
    string Situacao,
    string Observacao,
    string? Origem);
