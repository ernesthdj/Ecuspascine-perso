using Ecauspacine.Contracts.Lookups;

namespace Ecauspacine.Api.Features.Lookups;

public interface ILookupService
{
    // Groups
    Task<IReadOnlyList<LookupGroupDto>> ListGroupsAsync(bool includeItems = false, CancellationToken ct = default);
    Task<LookupGroupDto> CreateGroupAsync(LookupGroupCreateDto dto, CancellationToken ct = default);
    Task<LookupGroupDto?> UpdateGroupAsync(long id, LookupGroupUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteGroupAsync(long id, CancellationToken ct = default);

    // Items
    Task<IReadOnlyList<LookupItemDto>> ListItemsAsync(long groupId, CancellationToken ct = default);
    Task<LookupItemDto> CreateItemAsync(LookupItemCreateDto dto, CancellationToken ct = default); // <- body porte GroupId
    Task<LookupItemDto?> UpdateItemAsync(long itemId, LookupItemUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteItemAsync(long itemId, CancellationToken ct = default);
}
