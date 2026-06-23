using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Application.DTOs;

public sealed record RegisterUserRequest(string Nome, string Email, string Senha);
public sealed record LoginRequest(string Email, string Senha);
public sealed record AuthResponse(string Token, UserResponse Usuario);

public sealed record UserResponse(
    Guid Id,
    string Nome,
    string Email,
    DateTimeOffset CriadoEm,
    PrivacySettings Privacidade,
    EsmSettings Esm);
