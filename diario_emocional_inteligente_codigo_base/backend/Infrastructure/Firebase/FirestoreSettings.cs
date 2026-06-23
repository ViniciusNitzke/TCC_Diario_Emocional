namespace DiarioEmocional.Api.Infrastructure.Firebase;

public sealed class FirestoreSettings
{
    public const string SectionName = "Firestore";

    public string ProjectId { get; init; } = "";
    public string? CredentialPath { get; init; }
}
