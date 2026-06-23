using DiarioEmocional.Api.Application.DTOs;
using DiarioEmocional.Api.Application.Strategies;
using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;

namespace DiarioEmocional.Api.Application.Services;

public interface IReportService
{
    Task<DashboardResponse> BuildDashboardAsync(User user);
    Task<WeeklyReportResponse> BuildWeeklyReportAsync(Guid userId);
}

public sealed class ReportService(
    IEmotionalEntryRepository entries,
    ISelfCareSuggestionFactory suggestions) : IReportService
{
    public async Task<DashboardResponse> BuildDashboardAsync(User user)
    {
        var records = (await entries.ListByUserAsync(user.Id, 200))
            .OrderByDescending(record => record.CriadoEm)
            .ToList();

        var today = DateTimeOffset.UtcNow.Date;
        var recordsToday = records.Where(record => record.CriadoEm.Date == today).ToList();
        var lastSevenDays = records.Where(record => record.CriadoEm >= DateTimeOffset.UtcNow.AddDays(-7)).ToList();
        var latest = records.FirstOrDefault();

        var series = Enumerable.Range(0, 7)
            .Select(offset => today.AddDays(-6 + offset))
            .Select(day =>
            {
                var dayRecords = lastSevenDays.Where(record => record.CriadoEm.Date == day).ToList();
                return new DailySeriesResponse(
                    day.ToString("yyyy-MM-dd"),
                    dayRecords.Count,
                    dayRecords.Count == 0 ? 0 : Math.Round(dayRecords.Average(record => record.Intensidade), 2));
            })
            .ToArray();

        return new DashboardResponse(
            records.Count,
            recordsToday.Count,
            recordsToday.GroupBy(record => record.EmocaoNome).OrderByDescending(group => group.Count()).FirstOrDefault()?.Key,
            CalculateStreak(records),
            CalculateNextEsmPrompt(user.Esm),
            suggestions.Create(latest?.EmocaoId ?? "calma", latest?.Intensidade ?? 2).First(),
            series);
    }

    public async Task<WeeklyReportResponse> BuildWeeklyReportAsync(Guid userId)
    {
        var start = DateTimeOffset.UtcNow.AddDays(-7);
        var records = (await entries.ListByUserAsync(userId, 200))
            .Where(record => record.CriadoEm >= start)
            .OrderBy(record => record.CriadoEm)
            .ToList();

        var distribution = records
            .GroupBy(record => record.EmocaoNome)
            .Select(group => new FrequencyItem(group.Key, group.Count()))
            .OrderByDescending(item => item.Quantidade)
            .ToArray();

        var triggers = records
            .SelectMany(record => record.Gatilhos)
            .GroupBy(trigger => trigger, StringComparer.OrdinalIgnoreCase)
            .Select(group => new FrequencyItem(group.Key, group.Count()))
            .OrderByDescending(item => item.Quantidade)
            .Take(6)
            .ToArray();

        var averageIntensity = records.Count == 0 ? 0 : Math.Round(records.Average(record => record.Intensidade), 2);

        return new WeeklyReportResponse(
            records.Count,
            distribution.FirstOrDefault()?.Nome ?? "sem dados",
            averageIntensity,
            distribution,
            triggers,
            BuildInsight(records.Count, averageIntensity, triggers));
    }

    private static string BuildInsight(int total, double averageIntensity, IReadOnlyList<FrequencyItem> triggers) =>
        total switch
        {
            0 => "Ainda nao ha dados suficientes. O primeiro passo e registrar como voce esta agora.",
            < 3 => "Ha poucos registros, mas a consistencia ja comeca a reduzir o vies de memoria.",
            _ when averageIntensity >= 4 => "A intensidade media esta alta. Priorize pausas, respiracao e apoio social nesta semana.",
            _ when triggers.Count > 0 => $"O gatilho '{triggers[0].Nome}' apareceu com mais frequencia. Observe contextos e horarios associados.",
            _ => "A semana mostra variacao emocional acompanhavel. Continue registrando para revelar padroes mais confiaveis."
        };

    private static int CalculateStreak(IEnumerable<EmotionalEntry> records)
    {
        var days = records.Select(record => record.CriadoEm.Date).Distinct().ToHashSet();
        var cursor = DateTimeOffset.UtcNow.Date;
        var streak = 0;

        while (days.Contains(cursor))
        {
            streak++;
            cursor = cursor.AddDays(-1);
        }

        return streak;
    }

    private static string? CalculateNextEsmPrompt(EsmSettings settings)
    {
        if (!settings.Ativo || settings.Horarios.Length == 0) return null;

        var now = DateTimeOffset.Now;
        foreach (var time in settings.Horarios.OrderBy(time => time))
        {
            var parts = time.Split(':');
            var candidate = new DateTimeOffset(now.Year, now.Month, now.Day, int.Parse(parts[0]), int.Parse(parts[1]), 0, now.Offset);
            if (candidate > now) return candidate.ToString("HH:mm");
        }

        return settings.Horarios.OrderBy(time => time).First();
    }
}
