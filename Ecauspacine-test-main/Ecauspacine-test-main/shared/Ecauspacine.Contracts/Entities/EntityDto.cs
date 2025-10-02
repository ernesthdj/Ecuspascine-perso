namespace Ecauspacine.Contracts.Entities;

/// <summary>
/// Instance concrète (ex: "Corvette MK1").
/// </summary>
public record EntityDto(long Id, long EntityTypeId);

/// <summary>
/// Création d'entité (on choisit d'abord le type).
/// </summary>
public record EntityCreateDto(long EntityTypeId);

/// <summary>
/// Options éventuelles pour la suppression (rarement nécessaire).
/// Exemple: autoriser la cascade si des relations existent.
/// </summary>
public record EntityDeleteOptionsDto(bool ForceCascade = false);
