
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Lookups;
using Ecauspacine.Wpf.Services.Interfaces;

namespace Ecauspacine.Wpf.Services.Implementations;

public class LookupClient : ILookupClient
{
    private readonly IApiClient _api;

    public LookupClient(IApiClient api) => _api = api;

    public async Task<IReadOnlyList<LookupGroupDto>> GetGroupsAsync(bool includeItems = false, CancellationToken ct = default)
        => await _api.GetAsync<IReadOnlyList<LookupGroupDto>>($"/api/lookups/groups?includeItems={includeItems.ToString().ToLowerInvariant()}", ct)
            ?? new List<LookupGroupDto>();

    public async Task<IReadOnlyList<LookupItemDto>> GetItemsAsync(long groupId, CancellationToken ct = default)
        => await _api.GetAsync<IReadOnlyList<LookupItemDto>>($"/api/lookups/groups/{groupId}/items", ct) ?? new List<LookupItemDto>();
}
