namespace Ecauspacine.Contracts.Auth;

/// <summary>
/// Requête de connexion
/// </summary>
public record LoginRequestDto(string Username, string Password);

/// <summary>
/// Réponse de connexion avec token d'accès
/// </summary>
public record LoginResponseDto(string AccessToken, string? RefreshToken = null);
