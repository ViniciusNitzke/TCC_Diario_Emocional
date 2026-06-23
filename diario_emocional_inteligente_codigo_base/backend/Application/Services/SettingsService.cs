using System.Text.RegularExpressions;
using DiarioEmocional.Api.Application.Common;
using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;

namespace DiarioEmocional.Api.Application.Services;

public interface ISettingsService
{
    Task<Result<EsmSettings>> UpdateEsmAsync(User user, EsmSettings request);
    Task<PrivacySettings> UpdatePrivacyAsync(User user, PrivacySettings request);
}

public sealed class SettingsService(IUserRepository users) : ISettingsService
{
    public async Task<Result<EsmSettings>> UpdateEsmAsync(User user, EsmSettings request)
    {
        var schedules = request.Horarios
            .Where(time => Regex.IsMatch(time, "^([01][0-9]|2[0-3]):[0-5][0-9]$"))
            .Distinct()
            .OrderBy(time => time)
            .Take(6)
            .ToArray();

        if (schedules.Length == 0)
            return Result<EsmSettings>.Fail("Informe pelo menos um horario ESM valido no formato HH:mm.");

        var settings = new EsmSettings(
            request.Ativo,
            schedules,
            string.IsNullOrWhiteSpace(request.Frequencia) ? "todos_os_dias" : request.Frequencia.Trim());

        await users.UpdateAsync(user with { Esm = settings });
        return Result<EsmSettings>.Ok(settings);
    }

    public async Task<PrivacySettings> UpdatePrivacyAsync(User user, PrivacySettings request)
    {
        await users.UpdateAsync(user with { Privacidade = request });
        return request;
    }
}
