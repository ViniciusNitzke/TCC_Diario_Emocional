using DiarioEmocional.Api.Application.Services;
using DiarioEmocional.Api.Domain.Entities;

namespace DiarioEmocional.Api.Api;

public static class AuthExtensions
{
    public static string? GetBearerToken(this HttpContext http)
    {
        var header = http.Request.Headers.Authorization.ToString();
        return header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? header["Bearer ".Length..].Trim()
            : null;
    }

    public static Task<User?> GetAuthenticatedUserAsync(this HttpContext http, IAuthService auth) =>
        auth.GetCurrentUserAsync(http.GetBearerToken());
}
