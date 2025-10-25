namespace Ecauspacine.Wpf.Services.Interfaces;

/// <summary>
/// Stocke l'état d'authentification courant (token JWT).
/// </summary>
public interface IApiSession
{
    string? AccessToken { get; set; }
    bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken);
}
