using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Application.Strategies;

public interface ISelfCareStrategy
{
    bool CanHandle(string emotionId);
    IReadOnlyList<SelfCareSuggestion> Build(int intensity);
}
