using Ecauspacine.Contracts.Common;

namespace Ecauspacine.Contracts.Lookups;

/// <summary>
/// Élément d'un référentiel (lookup). Unicité (GroupId, Code) côté DB.
/// </summary>
public record LookupItemDto(
    long Id,
    long GroupId,
    string Code,
    string Label,
    int? Value,
    int OrderIndex,
    string? Description) : IHasId, IHasCodeLabel;

/// <summary>Payload de création d'un lookup item.</summary>
public record LookupItemCreateDto(
    long GroupId,
    string Code,
    string Label,
    int? Value,
    int OrderIndex = 1,
    string? Description = null);

/// <summary>Payload de mise à jour partielle (PATCH-like).</summary>
public record LookupItemUpdateDto(
    string? Label = null,
    int? Value = null,
    int? OrderIndex = null,
    string? Description = null);
