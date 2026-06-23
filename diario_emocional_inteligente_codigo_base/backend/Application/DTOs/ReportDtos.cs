using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Application.DTOs;

public sealed record DashboardResponse(
    int TotalRegistros,
    int RegistrosHoje,
    string? HumorHoje,
    int SequenciaDias,
    string? ProximoPromptEsm,
    SelfCareSuggestion SugestaoRapida,
    IReadOnlyList<DailySeriesResponse> Serie);

public sealed record DailySeriesResponse(string Dia, int Total, double IntensidadeMedia);

public sealed record WeeklyReportResponse(
    int Total,
    string EmocaoMaisFrequente,
    double IntensidadeMedia,
    IReadOnlyList<FrequencyItem> DistribuicaoEmocional,
    IReadOnlyList<FrequencyItem> GatilhosFrequentes,
    string InsightPrincipal);

public sealed record FrequencyItem(string Nome, int Quantidade);

public sealed record CrisisResponse(string Titulo, string Aviso, IReadOnlyList<CrisisStep> Passos);
