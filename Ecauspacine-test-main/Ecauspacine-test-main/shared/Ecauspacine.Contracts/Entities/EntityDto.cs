namespace Ecauspacine.Contracts.Entities;

/// <summary>
/// Instance concr�te (ex: "Corvette MK1").
/// </summary>
public record EntityDto(long Id, long EntityTypeId);

/// <summary>
/// Cr�ation d'entit� (on choisit d'abord le type).
/// </summary>
public record EntityCreateDto(long EntityTypeId);

/// <summary>
/// Options �ventuelles pour la suppression (rarement n�cessaire).
/// Exemple: autoriser la cascade si des relations existent.
/// </summary>
public record EntityDeleteOptionsDto(bool ForceCascade = false);
