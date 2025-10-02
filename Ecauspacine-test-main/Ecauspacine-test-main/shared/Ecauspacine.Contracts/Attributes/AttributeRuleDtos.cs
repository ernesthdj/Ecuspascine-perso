using System.Text.Json;

namespace Ecauspacine.Contracts.Attributes;

/// <summary>
/// R�gle liant un attribut � un type d'entit�.
/// <para><b>AttributeCode</b> = code effectif pour ce type (utilis� � l'export .cs).
/// Il est unique par (EntityType, AttributeCode).</para>
/// </summary>
public record AttributeRuleDto(
    long Id,
    long EntityTypeId,
    long AttributeDefId,
    string AttributeCode,       // <= expos� en lecture pour l�export .cs
    bool IsRequired,
    int OrderIndex,
    bool IsPrivateSet,
    JsonElement? DefaultValue,
    JsonElement? ConstraintsJson);

/// <summary>
/// Cr�ation d�une r�gle. L�API remplira AttributeCode � partir d�AttributeDef.Code.
/// </summary>
public record AttributeRuleCreateDto(
    long EntityTypeId,
    long AttributeDefId,
    bool IsRequired = false,
    int OrderIndex = 1,
    bool IsPrivateSet = false,
    JsonElement? DefaultValue = null,
    JsonElement? ConstraintsJson = null);

/// <summary>
/// Mise � jour partielle de la r�gle (pas de renommage ici).
/// Si le Code de l�attribut change, l�API synchronisera AttributeCode automatiquement.
/// </summary>
public record AttributeRuleUpdateDto(
    bool? IsRequired = null,
    int? OrderIndex = null,
    bool? IsPrivateSet = null,
    JsonElement? DefaultValue = null,
    JsonElement? ConstraintsJson = null);
