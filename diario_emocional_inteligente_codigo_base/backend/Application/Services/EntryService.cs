using DiarioEmocional.Api.Application.Common;
using DiarioEmocional.Api.Application.DTOs;
using DiarioEmocional.Api.Application.Strategies;
using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;

namespace DiarioEmocional.Api.Application.Services;

public interface IEntryService
{
    Task<Result<EmotionalEntry>> CreateAsync(Guid userId, CreateEntryRequest request);
    Task<IReadOnlyList<EmotionalEntry>> ListAsync(Guid userId, int take);
    Task<Result> DeleteAsync(Guid userId, Guid entryId);
    Task<int> DeleteAllAsync(Guid userId);
}

public sealed class EntryService(
    IEmotionalEntryRepository entries,
    ICatalogService catalog,
    ISelfCareSuggestionFactory suggestions) : IEntryService
{
    public async Task<Result<EmotionalEntry>> CreateAsync(Guid userId, CreateEntryRequest request)
    {
        var emotion = catalog.FindEmotion(request.EmocaoId);
        if (emotion is null)
            return Result<EmotionalEntry>.Fail("Selecione uma emocao valida do catalogo.");

        if (request.Intensidade is < 1 or > 5)
            return Result<EmotionalEntry>.Fail("A intensidade deve estar entre 1 e 5.");

        var triggers = request.Gatilhos
            .Select(trigger => trigger.Trim())
            .Where(trigger => trigger.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Take(8)
            .ToArray();

        var suggestedAction = suggestions.Create(emotion.Id, request.Intensidade).First();
        var entry = new EmotionalEntry(
            Guid.NewGuid(),
            userId,
            emotion.Id,
            emotion.Nome,
            emotion.Familia,
            request.Intensidade,
            triggers,
            request.Situacao.Trim(),
            request.Observacao.Trim(),
            suggestedAction.Titulo,
            string.IsNullOrWhiteSpace(request.Origem) ? "espontaneo" : request.Origem.Trim(),
            DateTimeOffset.UtcNow);

        await entries.AddAsync(entry);
        return Result<EmotionalEntry>.Ok(entry);
    }

    public Task<IReadOnlyList<EmotionalEntry>> ListAsync(Guid userId, int take) =>
        entries.ListByUserAsync(userId, Math.Clamp(take, 1, 200));

    public async Task<Result> DeleteAsync(Guid userId, Guid entryId)
    {
        var removed = await entries.DeleteAsync(userId, entryId);
        return removed == 0 ? Result.Fail("Registro nao encontrado.") : Result.Ok();
    }

    public Task<int> DeleteAllAsync(Guid userId) => entries.DeleteAllByUserAsync(userId);
}
