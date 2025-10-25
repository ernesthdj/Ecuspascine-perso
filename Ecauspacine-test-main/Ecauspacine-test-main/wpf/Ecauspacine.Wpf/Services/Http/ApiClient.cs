using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Collections.Generic;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Http;

/// <summary>
/// Client HTTP générique (JSON + gestion du token).
/// </summary>
public class ApiClient : IApiClient
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    private readonly HttpClient _http;
    private readonly IApiSession _session;

    public ApiClient(HttpClient http, IApiSession session)
    {
        _http = http;
        _session = session;
    }

    public HttpClient RawClient => _http;

    public async Task<string> GetHealthAsync(CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/api/health");
        await ApplyAuthAsync(request);
        var response = await _http.SendAsync(request, ct);
        await EnsureSuccess(response);
        return await response.Content.ReadAsStringAsync(ct);
    }

    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        await ApplyAuthAsync(request);
        var response = await _http.SendAsync(request, ct);
        await EnsureSuccess(response);
        return await DeserializeAsync<T>(response, ct);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = CreateJsonContent(body)
        };
        await ApplyAuthAsync(request);
        var response = await _http.SendAsync(request, ct);
        await EnsureSuccess(response);
        return await DeserializeAsync<TResponse>(response, ct);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Put, url)
        {
            Content = CreateJsonContent(body)
        };
        await ApplyAuthAsync(request);
        var response = await _http.SendAsync(request, ct);
        await EnsureSuccess(response);
        return await DeserializeAsync<TResponse>(response, ct);
    }

    public async Task DeleteAsync(string url, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, url);
        await ApplyAuthAsync(request);
        var response = await _http.SendAsync(request, ct);
        await EnsureSuccess(response);
    }

    public async Task<byte[]> PostForBytesAsync<TRequest>(string url, TRequest body, CancellationToken ct = default)
    {
        var (content, _) = await PostForBytesWithHeadersAsync(url, body, ct);
        return content;
    }

    public async Task<(byte[] Content, IDictionary<string, IEnumerable<string>> Headers)> PostForBytesWithHeadersAsync<TRequest>(string url, TRequest body, CancellationToken ct = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = CreateJsonContent(body)
        };
        await ApplyAuthAsync(request);
        var response = await _http.SendAsync(request, ct);
        await EnsureSuccess(response);
        var bytes = await response.Content.ReadAsByteArrayAsync(ct);
        var headers = new Dictionary<string, IEnumerable<string>>();
        foreach (var header in response.Headers)
        {
            headers[header.Key] = header.Value;
        }
        return (bytes, headers);
    }

    private HttpContent CreateJsonContent<T>(T body)
    {
        var json = JsonSerializer.Serialize(body, JsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private static async Task<T?> DeserializeAsync<T>(HttpResponseMessage response, CancellationToken ct)
    {
        if (response.Content.Headers.ContentLength == 0)
            return default;
        await using var stream = await response.Content.ReadAsStreamAsync(ct);
        if (stream.Length == 0)
            return default;
        return await JsonSerializer.DeserializeAsync<T>(stream, JsonOptions, ct);
    }

    private async Task ApplyAuthAsync(HttpRequestMessage request)
    {
        if (!string.IsNullOrEmpty(_session.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _session.AccessToken);
        }
        await Task.CompletedTask;
    }

    private static async Task EnsureSuccess(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
            return;

        string? error = null;
        try
        {
            var payload = await response.Content.ReadAsStringAsync();
            if (!string.IsNullOrWhiteSpace(payload))
            {
                using var doc = JsonDocument.Parse(payload);
                if (doc.RootElement.TryGetProperty("error", out var errorProp))
                    error = errorProp.GetString();
            }
        }
        catch
        {
            // ignore parsing errors
        }

        response.Dispose();
        throw new HttpRequestException(error ?? $"Erreur HTTP {(int)response.StatusCode} {response.ReasonPhrase}");
    }
}
