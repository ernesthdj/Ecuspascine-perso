using System.ComponentModel.DataAnnotations;
using Ecauspacine.Contracts.Common;

namespace Ecauspacine.Contracts.EntityTypes;

/// <summary>
/// Type d'entité (classe logique).
/// <para><b>Code</b> est utilisé pour l'export C# (.cs) comme nom de classe.</para>
/// </summary>
public record EntityTypeDto(
    long Id,
    string Code,
    string Label,
    string? Description) : IHasId, IHasCodeLabel;

/// <summary>
/// Création. Le Code sera validé/normalisé côté API (identifiant C#).
/// </summary>
public record EntityTypeCreateDto(
    [Required, MinLength(1), MaxLength(255)] string Code,
    [Required, MinLength(1), MaxLength(255)] string Label,
    string? Description);

/// <summary>
/// Mise à jour partielle.
/// <para>On autorise le renommage de <b>Code</b> (impacte l'export .cs). L'API vérifiera validité/unicité.</para>
/// </summary>
public record EntityTypeUpdateDto(
    [MaxLength(255)] string? Code = null,
    [MaxLength(255)] string? Label = null,
    string? Description = null);
