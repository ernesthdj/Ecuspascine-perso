using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Ecauspacine.Contracts.Lookups;

namespace Ecauspacine.Wpf.Services.Interfaces;

public interface ILookupClient
{
    Task<IReadOnlyList<LookupGroupDto>> GetGroupsAsync(bool includeItems = false, CancellationToken ct = default);
    Task<IReadOnlyList<LookupItemDto>> GetItemsAsync(long groupId, CancellationToken ct = default);
}
