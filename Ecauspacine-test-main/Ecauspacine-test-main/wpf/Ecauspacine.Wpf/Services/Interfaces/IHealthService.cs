using System.Threading.Tasks;

namespace Ecauspacine.Wpf.Services.Interfaces;

/// <summary>
/// Service métier Health : point d\'entrée pour l\'UI.
/// </summary>
public interface IHealthService
{
    /// <summary>Vérifie la santé de l\'API et renvoie une chaîne lisible.</summary>
    Task<string> CheckAsync();
}
