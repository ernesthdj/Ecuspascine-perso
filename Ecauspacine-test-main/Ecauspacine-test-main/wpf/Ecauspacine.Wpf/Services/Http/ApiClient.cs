using System.Net.Http;
using System.Threading.Tasks;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Http;

/// <summary>
/// Implémentation d\'IApiClient basée sur HttpClient.
/// </summary>
public class ApiClient : IApiClient
{
    private readonly HttpClient _http;
    public ApiClient(HttpClient http) => _http = http;

    public async Task<string> GetHealthAsync()
    {
        var resp = await _http.GetAsync("/api/health"); // endpoint de santé
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadAsStringAsync();
    }
}
