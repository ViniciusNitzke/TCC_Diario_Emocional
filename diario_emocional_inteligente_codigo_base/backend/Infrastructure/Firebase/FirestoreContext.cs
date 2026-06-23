using Google.Cloud.Firestore;

namespace DiarioEmocional.Api.Infrastructure.Firebase;

public sealed class FirestoreContext
{
    public FirestoreContext(IConfiguration configuration)
    {
        var settings = configuration.GetSection(FirestoreSettings.SectionName).Get<FirestoreSettings>()
            ?? new FirestoreSettings();

        if (string.IsNullOrWhiteSpace(settings.ProjectId))
            throw new InvalidOperationException("Configure Firestore:ProjectId para usar persistencia Firebase.");

        if (!string.IsNullOrWhiteSpace(settings.CredentialPath))
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", settings.CredentialPath);

        Database = FirestoreDb.Create(settings.ProjectId);
    }

    public FirestoreDb Database { get; }
}
