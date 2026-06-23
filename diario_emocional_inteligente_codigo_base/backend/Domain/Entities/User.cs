namespace DiarioEmocional.Api.Domain.Entities;

public sealed record User(
    Guid Id,
    string Nome,
    string Email,
    string SenhaHash,
    string SenhaSalt,
    DateTimeOffset CriadoEm,
    PrivacySettings Privacidade,
    EsmSettings Esm);

public sealed record PrivacySettings(
    bool BloqueioLocal,
    bool PermitirRelatorios,
    bool ConsentimentoDadosSensiveis);

public sealed record EsmSettings(
    bool Ativo,
    string[] Horarios,
    string Frequencia);

public sealed record Session(
    string Token,
    Guid UsuarioId,
    DateTimeOffset CriadoEm,
    DateTimeOffset ExpiraEm);
