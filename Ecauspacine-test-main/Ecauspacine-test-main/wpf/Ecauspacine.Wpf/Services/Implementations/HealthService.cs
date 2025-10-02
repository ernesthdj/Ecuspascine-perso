using System.Threading.Tasks;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Implementations;

/// <summary>
/// Service métier : pourrait plus tard mapper le JSON vers un DTO Contracts.
/// </summary>
public class HealthService : IHealthService
{
    private readonly IApiClient _api;
    public HealthService(IApiClient api) => _api = api;

    public Task<string> CheckAsync() => _api.GetHealthAsync();
}
