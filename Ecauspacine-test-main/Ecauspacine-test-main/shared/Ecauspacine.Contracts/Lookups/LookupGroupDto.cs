using Ecauspacine.Contracts.Common;

namespace Ecauspacine.Contracts.Lookups;

/// <summary>
/// Groupe (référentiel). Peut optionnellement embarquer ses Items.
/// </summary>
public record LookupGroupDto(
    long Id,
    string Code,
    string Label,
    string? Description,
    IReadOnlyList<LookupItemDto>? Items = null) : IHasId, IHasCodeLabel;

public record LookupGroupCreateDto(string Code, string Label, string? Description);
public record LookupGroupUpdateDto(string? Label = null, string? Description = null);
