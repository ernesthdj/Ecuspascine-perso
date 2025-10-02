using System.Text.Json;

namespace Ecauspacine.Contracts.Values;

/// <summary>
/// Représentation "flattened" d'une valeur EAV.
/// Un seul des 3 champs est utilisé selon DataKind:
/// - json      → JsonValue
/// - lookup    → LookupItemId
/// - ref_entity→ RefEntityId
/// </summary>
public record AttributeValueDto(
    long AttributeValId,
    long EntityId,
    long AttributeDefId,
    string DataKind,                // "json" | "lookup" | "ref_entity"
    JsonElement? JsonValue,
    long? LookupItemId,
    long? RefEntityId);

/// <summary>
/// Upsert (création ou mise à jour) d'une valeur EAV.
/// L'API validera que le champ fourni correspond bien au DataKind.
/// </summary>
public record AttributeValueUpsertDto(
    long EntityId,
    long AttributeDefId,
    string DataKind,                // "json" | "lookup" | "ref_entity"
    JsonElement? JsonValue = null,
    long? LookupItemId = null,
    long? RefEntityId = null);
