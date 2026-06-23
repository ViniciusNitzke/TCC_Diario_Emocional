using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Application.Services;

public interface ICatalogService
{
    IReadOnlyList<EmotionDefinition> GetEmotions();
    EmotionDefinition? FindEmotion(string id);
}

public sealed class CatalogService : ICatalogService
{
    private static readonly EmotionDefinition[] Emotions =
    {
        new("alegria", "Alegria", "positiva", "#f2b705", "Energia, prazer, gratidao e conexao."),
        new("confianca", "Confianca", "positiva", "#4caf50", "Seguranca, abertura e sensacao de apoio."),
        new("calma", "Calma", "regulacao", "#22a6b3", "Tranquilidade e estabilidade emocional."),
        new("surpresa", "Surpresa", "ativacao", "#7c4dff", "Reacao a algo inesperado ou novo."),
        new("tristeza", "Tristeza", "vulneravel", "#4f7cac", "Perda, desanimo, saudade ou necessidade de acolhimento."),
        new("medo", "Medo", "alerta", "#8e44ad", "Percepcao de risco, inseguranca ou ameaca."),
        new("raiva", "Raiva", "defesa", "#e74c3c", "Frustracao, injustica percebida ou limite ultrapassado."),
        new("ansiedade", "Ansiedade", "alerta", "#f97316", "Preocupacao antecipatoria e ativacao fisiologica."),
        new("nojo", "Nojo", "defesa", "#6b8e23", "Rejeicao, aversao ou necessidade de afastamento."),
        new("expectativa", "Expectativa", "ativacao", "#0f9d58", "Antecipacao, preparacao e curiosidade diante do futuro.")
    };

    public IReadOnlyList<EmotionDefinition> GetEmotions() => Emotions;

    public EmotionDefinition? FindEmotion(string id) =>
        Emotions.FirstOrDefault(emotion => emotion.Id == id);
}
