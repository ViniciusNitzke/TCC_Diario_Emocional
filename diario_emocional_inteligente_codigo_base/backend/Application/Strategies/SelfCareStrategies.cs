using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Application.Strategies;

public sealed class AnxietyFearStrategy : ISelfCareStrategy
{
    private static readonly string[] EmotionIds = { "ansiedade", "medo" };

    public bool CanHandle(string emotionId) => EmotionIds.Contains(emotionId);

    public IReadOnlyList<SelfCareSuggestion> Build(int intensity) => WithCrisisIfNeeded(intensity, new[]
    {
        new SelfCareSuggestion("Respiracao 4-6", "Inspire por 4 segundos e expire por 6 por tres minutos.", "respiracao"),
        new SelfCareSuggestion("Cheque de realidade", "Anote uma evidencia a favor e uma contra o pensamento ansioso.", "reflexao"),
        new SelfCareSuggestion("Aterramento 5-4-3-2-1", "Use os sentidos para voltar ao presente.", "crise")
    });

    private static IReadOnlyList<SelfCareSuggestion> WithCrisisIfNeeded(int intensity, IReadOnlyList<SelfCareSuggestion> suggestions) =>
        intensity >= 4
            ? suggestions.Prepend(new SelfCareSuggestion("Plano de estabilizacao", "Reduza estimulos, respire devagar e acione apoio se sentir risco.", "crise")).ToArray()
            : suggestions;
}

public sealed class AngerStrategy : ISelfCareStrategy
{
    public bool CanHandle(string emotionId) => emotionId == "raiva";

    public IReadOnlyList<SelfCareSuggestion> Build(int intensity)
    {
        var suggestions = new[]
        {
            new SelfCareSuggestion("Pausa de 90 segundos", "Afaste-se do estimulo e observe a sensacao passar pelo corpo.", "pausa"),
            new SelfCareSuggestion("Mensagem nao enviada", "Escreva o que gostaria de dizer e revise depois.", "reflexao"),
            new SelfCareSuggestion("Movimento breve", "Caminhe por cinco minutos para descarregar ativacao.", "corpo")
        };

        return intensity >= 4
            ? suggestions.Prepend(new SelfCareSuggestion("Reduzir estimulo", "Evite responder no impulso e retome a conversa quando estiver mais regulado.", "crise")).ToArray()
            : suggestions;
    }
}

public sealed class SadnessStrategy : ISelfCareStrategy
{
    public bool CanHandle(string emotionId) => emotionId == "tristeza";

    public IReadOnlyList<SelfCareSuggestion> Build(int intensity)
    {
        var suggestions = new[]
        {
            new SelfCareSuggestion("Contato seguro", "Envie uma mensagem simples para alguem de confianca.", "apoio"),
            new SelfCareSuggestion("Microtarefa gentil", "Escolha uma tarefa de ate dois minutos e conclua sem perfeccionismo.", "rotina"),
            new SelfCareSuggestion("Registro compassivo", "Escreva o que voce diria a um amigo na mesma situacao.", "reflexao")
        };

        return intensity >= 4
            ? suggestions.Prepend(new SelfCareSuggestion("Apoio agora", "Se houver risco ou sofrimento intenso, procure uma pessoa de confianca ou atendimento especializado.", "crise")).ToArray()
            : suggestions;
    }
}

public sealed class DefaultSelfCareStrategy : ISelfCareStrategy
{
    public bool CanHandle(string emotionId) => true;

    public IReadOnlyList<SelfCareSuggestion> Build(int intensity) => new[]
    {
        new SelfCareSuggestion("Check-in corporal", "Observe tensao, respiracao e postura por um minuto.", "consciencia"),
        new SelfCareSuggestion("Gratidao concreta", "Registre uma coisa pequena que ajudou hoje.", "reflexao"),
        new SelfCareSuggestion("Pausa consciente", "Beba agua e faca uma pausa sem tela por cinco minutos.", "pausa")
    };
}
