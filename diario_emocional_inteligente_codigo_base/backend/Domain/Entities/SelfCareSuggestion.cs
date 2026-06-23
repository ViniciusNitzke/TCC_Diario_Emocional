namespace DiarioEmocional.Api.Domain.Entities;

public sealed record SelfCareSuggestion(string Titulo, string Descricao, string Tipo);

public sealed record CrisisStep(string Titulo, string Descricao);
