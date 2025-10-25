using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Ecauspacine.Wpf.Services.Interfaces;

/// <summary>
/// Client HTTP bas-niveau : sérialisation JSON + gestion du token.
/// </summary>
public interface IApiClient
{
    Task<string> GetHealthAsync(CancellationToken ct = default);
    Task<T?> GetAsync<T>(string url, CancellationToken ct = default);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default);
    Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default);
    Task DeleteAsync(string url, CancellationToken ct = default);
    Task<byte[]> PostForBytesAsync<TRequest>(string url, TRequest body, CancellationToken ct = default);
    Task<(byte[] Content, IDictionary<string, IEnumerable<string>> Headers)> PostForBytesWithHeadersAsync<TRequest>(string url, TRequest body, CancellationToken ct = default);
    HttpClient RawClient { get; }
}
