
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Auth;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Implementations;

public class AuthenticationService : IAuthenticationService
{
    private readonly IApiClient _api;
    private readonly IApiSession _session;

    public AuthenticationService(IApiClient api, IApiSession session)
    {
        _api = api;
        _session = session;
    }

    public async Task<bool> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var request = new LoginRequestDto(username, password);
        var response = await _api.PostAsync<LoginRequestDto, LoginResponseDto>("/api/auth/login", request, ct);
        if (response is null)
            return false;

        _session.AccessToken = response.AccessToken;
        return true;
    }

    public void Logout() => _session.AccessToken = null;
}
