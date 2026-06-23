using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Application.Strategies;

public interface ISelfCareSuggestionFactory
{
    IReadOnlyList<SelfCareSuggestion> Create(string emotionId, int intensity);
}

public sealed class SelfCareSuggestionFactory(IEnumerable<ISelfCareStrategy> strategies) : ISelfCareSuggestionFactory
{
    public IReadOnlyList<SelfCareSuggestion> Create(string emotionId, int intensity)
    {
        var strategy = strategies.First(strategy => strategy.CanHandle(emotionId));
        return strategy.Build(Math.Clamp(intensity, 1, 5));
    }
}
