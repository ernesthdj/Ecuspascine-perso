using System.ComponentModel.DataAnnotations;
using Ecauspacine.Contracts.Common;

namespace Ecauspacine.Contracts.Attributes;

/// <summary>
/// Définition d'un attribut EAV.
/// <para><b>Code</b> sera utilisé comme nom de propriété dans l'export .cs.</para>
/// <para>Pas d'unicité globale sur attribute_def.Code ; l'unicité est assurée par type d'entité (via les règles).</para>
///<summary>
public record AttributeDefDto(
    long Id,
    string Code,
    string Label,
    string? Unit,
    long DataKindId,
    long? RefEntityTypeId,
    long? LookupGroupId,
    string? Description,
    bool IsActive = true) : IHasId, IHasCodeLabel;

/// <summary>
/// Création: DataKindCode lisible (ex: "text","json","lookup","ref_entity") sera résolu côté API.
/// </summary>
public record AttributeDefCreateDto(
    [Required, MinLength(1), MaxLength(255)] string Code,
    [Required, MinLength(1), MaxLength(255)] string Label,
    string? Unit,
    [Required] string DataKindCode,
    long? RefEntityTypeId,
    long? LookupGroupId,
    string? Description);

/// <summary>
/// Mise à jour partielle.
/// <para>Autorise le renommage de <b>Code</b> ; l’API fera normalisation + vérif d’unicité par type d’entité.</para>
/// <para>Le changement de DataKind reste du ressort de l’API (en général refusé s'il existe des valeurs).</para>
/// </summary>
public record AttributeDefUpdateDto(
    [MaxLength(255)] string? Code = null,
    [MaxLength(255)] string? Label = null,
    string? Unit = null,
    long? LookupGroupId = null,
    string? Description = null,
    bool? IsActive = null);
