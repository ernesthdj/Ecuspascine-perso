
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Entities;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Implementations;

public class DataService : IDataService
{
    private readonly IApiClient _api;

    public DataService(IApiClient api) => _api = api;

    public async Task<IReadOnlyList<EntityRecordDto>> GetRecordsAsync(long entityTypeId, string? search, string? sort, bool desc, CancellationToken ct = default)
    {
        var query = $"/api/entity-types/{entityTypeId}/records?desc={desc.ToString().ToLowerInvariant()}";
        if (!string.IsNullOrWhiteSpace(search))
            query += $"&search={System.Net.WebUtility.UrlEncode(search)}";
        if (!string.IsNullOrWhiteSpace(sort))
            query += $"&sort={System.Net.WebUtility.UrlEncode(sort)}";

        return await _api.GetAsync<IReadOnlyList<EntityRecordDto>>(query, ct) ?? new List<EntityRecordDto>();
    }

    public async Task<EntityRecordDto> CreateRecordAsync(EntityRecordCreateDto dto, CancellationToken ct = default)
        => await _api.PostAsync<EntityRecordCreateDto, EntityRecordDto>($"/api/entity-types/{dto.EntityTypeId}/records", dto, ct)
            ?? throw new System.InvalidOperationException("Création d'enregistrement échouée");

    public Task<EntityRecordDto?> UpdateRecordAsync(long entityTypeId, long recordId, EntityRecordUpdateDto dto, CancellationToken ct = default)
        => _api.PutAsync<EntityRecordUpdateDto, EntityRecordDto>($"/api/entity-types/{entityTypeId}/records/{recordId}", dto, ct);

    public Task DeleteRecordAsync(long entityTypeId, long recordId, CancellationToken ct = default)
        => _api.DeleteAsync($"/api/entity-types/{entityTypeId}/records/{recordId}", ct);
}
