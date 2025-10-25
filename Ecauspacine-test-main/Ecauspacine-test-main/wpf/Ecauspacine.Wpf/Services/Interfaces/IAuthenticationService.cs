using System.Threading;
using System.Threading.Tasks;

namespace Ecauspacine.Wpf.Services.Interfaces;

public interface IAuthenticationService
{
    Task<bool> LoginAsync(string username, string password, CancellationToken ct = default);
    void Logout();
}
