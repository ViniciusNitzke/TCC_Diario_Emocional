using DiarioEmocional.Api.Application.DTOs;
using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Application.Services;

public interface ICrisisSupportService
{
    CrisisResponse GetSupportPlan();
}

public sealed class CrisisSupportService : ICrisisSupportService
{
    public CrisisResponse GetSupportPlan() => new(
        "Modo de acolhimento",
        "Use estas orientacoes para estabilizacao imediata. Em risco de autoagressao ou emergencia, procure ajuda presencial ou ligue 188 (CVV) no Brasil.",
        new[]
        {
            new CrisisStep("Respire em caixa", "Inspire por 4 segundos, segure por 4, expire por 4 e aguarde por 4. Repita 4 vezes."),
            new CrisisStep("Aterre no ambiente", "Nomeie 5 coisas que voce ve, 4 que sente, 3 que ouve, 2 que cheira e 1 sabor."),
            new CrisisStep("Reduza estimulos", "Sente-se, afrouxe roupas apertadas, beba agua e diminua luz ou ruido se possivel."),
            new CrisisStep("Acione apoio", "Envie uma mensagem para uma pessoa de confianca ou procure atendimento se houver risco.")
        });
}
