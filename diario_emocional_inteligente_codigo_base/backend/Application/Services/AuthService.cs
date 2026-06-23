using System.Text.RegularExpressions;
using DiarioEmocional.Api.Application.Common;
using DiarioEmocional.Api.Application.DTOs;
using DiarioEmocional.Api.Application.Security;
using DiarioEmocional.Api.Domain.Entities;
using DiarioEmocional.Api.Domain.Repositories;

namespace DiarioEmocional.Api.Application.Services;

public interface IAuthService
{
    Task<Result<AuthResponse>> RegisterAsync(RegisterUserRequest request);
    Task<Result<AuthResponse>> LoginAsync(LoginRequest request);
    Task<User?> GetCurrentUserAsync(string? token);
    Task LogoutAsync(string? token);
}

public sealed class AuthService(
    IUserRepository users,
    ISessionRepository sessions,
    IPasswordHasher passwordHasher) : IAuthService
{
    public async Task<Result<AuthResponse>> RegisterAsync(RegisterUserRequest request)
    {
        var nome = request.Nome.Trim();
        var email = request.Email.Trim().ToLowerInvariant();

        if (nome.Length < 2)
            return Result<AuthResponse>.Fail("Informe um nome com pelo menos 2 caracteres.");

        if (!Regex.IsMatch(email, "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$"))
            return Result<AuthResponse>.Fail("Informe um e-mail valido.");

        if (request.Senha.Length < 6)
            return Result<AuthResponse>.Fail("A senha deve ter pelo menos 6 caracteres.");

        if (await users.FindByEmailAsync(email) is not null)
            return Result<AuthResponse>.Fail("Ja existe uma conta cadastrada com este e-mail.");

        var (hash, salt) = passwordHasher.Hash(request.Senha);
        var user = new User(
            Guid.NewGuid(),
            nome,
            email,
            hash,
            salt,
            DateTimeOffset.UtcNow,
            new PrivacySettings(true, true, true),
            new EsmSettings(true, new[] { "09:00", "14:00", "20:00" }, "todos_os_dias"));

        await users.AddAsync(user);
        var session = await sessions.CreateAsync(user.Id);
        return Result<AuthResponse>.Ok(ToAuthResponse(user, session.Token));
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await users.FindByEmailAsync(request.Email.Trim().ToLowerInvariant());

        if (user is null || !passwordHasher.Verify(request.Senha, user.SenhaHash, user.SenhaSalt))
            return Result<AuthResponse>.Fail("E-mail ou senha invalidos.");

        var session = await sessions.CreateAsync(user.Id);
        return Result<AuthResponse>.Ok(ToAuthResponse(user, session.Token));
    }

    public async Task<User?> GetCurrentUserAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var session = await sessions.FindByTokenAsync(token);
        if (session is null || session.ExpiraEm <= DateTimeOffset.UtcNow) return null;

        return await users.FindByIdAsync(session.UsuarioId);
    }

    public async Task LogoutAsync(string? token)
    {
        if (!string.IsNullOrWhiteSpace(token))
            await sessions.DeleteAsync(token);
    }

    private static AuthResponse ToAuthResponse(User user, string token) =>
        new(token, ToUserResponse(user));

    public static UserResponse ToUserResponse(User user) =>
        new(user.Id, user.Nome, user.Email, user.CriadoEm, user.Privacidade, user.Esm);
}
