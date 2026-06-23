using DiarioEmocional.Api.Application.Documents;
using DiarioEmocional.Api.Application.Security;
using DiarioEmocional.Api.Application.Services;
using DiarioEmocional.Api.Application.Strategies;
using DiarioEmocional.Api.Domain.Repositories;
using DiarioEmocional.Api.Infrastructure.Documents;
using DiarioEmocional.Api.Infrastructure.Firebase;
using DiarioEmocional.Api.Infrastructure.Firebase.Repositories;
using DiarioEmocional.Api.Infrastructure.Persistence;
using DiarioEmocional.Api.Infrastructure.Persistence.Repositories;
using DiarioEmocional.Api.Infrastructure.Security;

namespace DiarioEmocional.Api.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddDiarioEmocional(this IServiceCollection services, IConfiguration configuration)
    {
        var provider = configuration.GetValue<string>("Persistence:Provider") ?? "Json";
        if (provider.Equals("Firestore", StringComparison.OrdinalIgnoreCase))
        {
            services.AddSingleton<FirestoreContext>();
            services.AddScoped<IUserRepository, FirestoreUserRepository>();
            services.AddScoped<ISessionRepository, FirestoreSessionRepository>();
            services.AddScoped<IEmotionalEntryRepository, FirestoreEmotionalEntryRepository>();
        }
        else
        {
            services.AddSingleton<JsonDiarioContext>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISessionRepository, SessionRepository>();
            services.AddScoped<IEmotionalEntryRepository, EmotionalEntryRepository>();
        }

        services.AddScoped<IPasswordHasher, Sha256PasswordHasher>();
        services.AddScoped<IPdfReportExporter, SimplePdfReportExporter>();

        services.AddScoped<ICatalogService, CatalogService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEntryService, EntryService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ISettingsService, SettingsService>();
        services.AddScoped<ICrisisSupportService, CrisisSupportService>();

        services.AddScoped<ISelfCareStrategy, AnxietyFearStrategy>();
        services.AddScoped<ISelfCareStrategy, AngerStrategy>();
        services.AddScoped<ISelfCareStrategy, SadnessStrategy>();
        services.AddScoped<ISelfCareStrategy, DefaultSelfCareStrategy>();
        services.AddScoped<ISelfCareSuggestionFactory, SelfCareSuggestionFactory>();

        return services;
    }
}
