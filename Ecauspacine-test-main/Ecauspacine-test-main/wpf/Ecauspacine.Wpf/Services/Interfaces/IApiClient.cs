using System.Threading.Tasks;

namespace Ecauspacine.Wpf.Services.Interfaces;

/// <summary>
/// Client HTTP bas-niveau : encapsule HttpClient.
/// </summary>
public interface IApiClient
{
    /// <summary>GET /api/health → retourne le JSON brut.</summary>
    Task<string> GetHealthAsync();
}
