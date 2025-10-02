using Ecauspacine.Api.Domain.Lookups;
using Ecauspacine.Contracts.Lookups;

namespace Ecauspacine.Api.Mapping;

public static class LookupMapping
{
    // ===== GROUP =====
    public static LookupGroupDto ToDto(this LookupGroup g, IReadOnlyList<LookupItemDto>? items = null)
        => new LookupGroupDto(
            g.Id,
            g.Code,
            g.Label,
            g.Description,
            items
        );

    public static LookupGroup ToEntity(this LookupGroupCreateDto dto)
        => new()
        {
            Code = dto.Code,
            Label = dto.Label,
            Description = dto.Description
        };

    // ===== ITEM =====
    public static LookupItemDto ToDto(this LookupItem i)
        => new LookupItemDto(
            i.Id,
            i.GroupId,
            i.Code,
            i.Label,
            i.Value,
            i.OrderIndex,
            i.Description
        );

    public static LookupItem ToEntity(this LookupItemCreateDto dto)
        => new()
        {
            GroupId = dto.GroupId,
            Code = dto.Code,
            Label = dto.Label,
            Value = dto.Value,
            OrderIndex = dto.OrderIndex,
            Description = dto.Description
        };
}
