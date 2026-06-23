using DiarioEmocional.Api.Application.Documents;
using DiarioEmocional.Api.Application.DTOs;
using DiarioEmocional.Api.Application.Services;
using DiarioEmocional.Api.Application.Strategies;
using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Api;

public static class EndpointMapping
{
    public static WebApplication MapApiEndpoints(this WebApplication app)
    {
        app.MapGet("/api/health", () => Results.Ok(new
        {
            status = "ok",
            nome = "Diario Emocional Inteligente",
            data = DateTimeOffset.UtcNow
        }));

        app.MapGet("/api/catalogo/emocoes", (ICatalogService catalog) =>
            Results.Ok(catalog.GetEmotions()));

        app.MapPost("/api/auth/register", async (RegisterUserRequest request, IAuthService auth) =>
        {
            var result = await auth.RegisterAsync(request);
            return result.Success ? Results.Created("/api/auth/me", result.Value) : Results.BadRequest(new { mensagem = result.Error });
        });

        app.MapPost("/api/auth/login", async (LoginRequest request, IAuthService auth) =>
        {
            var result = await auth.LoginAsync(request);
            return result.Success ? Results.Ok(result.Value) : Results.BadRequest(new { mensagem = result.Error });
        });

        app.MapPost("/api/auth/logout", async (HttpContext http, IAuthService auth) =>
        {
            await auth.LogoutAsync(http.GetBearerToken());
            return Results.NoContent();
        });

        app.MapGet("/api/auth/me", async (HttpContext http, IAuthService auth) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            return user is null ? Results.Unauthorized() : Results.Ok(AuthService.ToUserResponse(user));
        });

        app.MapGet("/api/dashboard", async (HttpContext http, IAuthService auth, IReportService reports) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            return user is null ? Results.Unauthorized() : Results.Ok(await reports.BuildDashboardAsync(user));
        });

        app.MapPost("/api/registros", async (HttpContext http, CreateEntryRequest request, IAuthService auth, IEntryService entries) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            if (user is null) return Results.Unauthorized();

            var result = await entries.CreateAsync(user.Id, request);
            return result.Success ? Results.Created($"/api/registros/{result.Value!.Id}", result.Value) : Results.BadRequest(new { mensagem = result.Error });
        });

        app.MapGet("/api/registros", async (HttpContext http, IAuthService auth, IEntryService entries, int? take) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            return user is null ? Results.Unauthorized() : Results.Ok(await entries.ListAsync(user.Id, take ?? 50));
        });

        app.MapDelete("/api/registros/{id:guid}", async (HttpContext http, Guid id, IAuthService auth, IEntryService entries) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            if (user is null) return Results.Unauthorized();

            var result = await entries.DeleteAsync(user.Id, id);
            return result.Success ? Results.NoContent() : Results.NotFound(new { mensagem = result.Error });
        });

        app.MapGet("/api/relatorios/semanal", async (HttpContext http, IAuthService auth, IReportService reports) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            return user is null ? Results.Unauthorized() : Results.Ok(await reports.BuildWeeklyReportAsync(user.Id));
        });

        app.MapGet("/api/relatorios/pdf", async (HttpContext http, IAuthService auth, IReportService reports, IPdfReportExporter exporter) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            if (user is null) return Results.Unauthorized();

            var report = await reports.BuildWeeklyReportAsync(user.Id);
            var bytes = exporter.Export(user.Nome, report);
            return Results.File(bytes, "application/pdf", $"relatorio-emocional-{DateTime.UtcNow:yyyyMMdd}.pdf");
        });

        app.MapGet("/api/autocuidado/sugestoes", (string? emocaoId, int? intensidade, ISelfCareSuggestionFactory factory) =>
            Results.Ok(factory.Create(string.IsNullOrWhiteSpace(emocaoId) ? "calma" : emocaoId, intensidade ?? 3)));

        app.MapGet("/api/crise", (ICrisisSupportService crisis) =>
            Results.Ok(crisis.GetSupportPlan()));

        app.MapGet("/api/esm/configuracao", async (HttpContext http, IAuthService auth) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            return user is null ? Results.Unauthorized() : Results.Ok(user.Esm);
        });

        app.MapPut("/api/esm/configuracao", async (HttpContext http, EsmSettings request, IAuthService auth, ISettingsService settings) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            if (user is null) return Results.Unauthorized();

            var result = await settings.UpdateEsmAsync(user, request);
            return result.Success ? Results.Ok(result.Value) : Results.BadRequest(new { mensagem = result.Error });
        });

        app.MapPut("/api/privacidade", async (HttpContext http, PrivacySettings request, IAuthService auth, ISettingsService settings) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            return user is null ? Results.Unauthorized() : Results.Ok(await settings.UpdatePrivacyAsync(user, request));
        });

        app.MapDelete("/api/privacidade/dados", async (HttpContext http, IAuthService auth, IEntryService entries) =>
        {
            var user = await http.GetAuthenticatedUserAsync(auth);
            if (user is null) return Results.Unauthorized();

            var removed = await entries.DeleteAllAsync(user.Id);
            return Results.Ok(new { registrosRemovidos = removed });
        });

        return app;
    }
}
