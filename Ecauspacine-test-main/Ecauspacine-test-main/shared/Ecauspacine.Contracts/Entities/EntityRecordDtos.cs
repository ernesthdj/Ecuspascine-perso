using System.Collections.Generic;
using Ecauspacine.Contracts.Values;

namespace Ecauspacine.Contracts.Entities;

/// <summary>
/// Enregistrement d'entité avec toutes ses valeurs d'attributs
/// </summary>
public record EntityRecordDto(
    long Id,
    long EntityTypeId,
    string? Code,
    IReadOnlyList<AttributeValueDto> Values);

/// <summary>
/// Création d'un nouvel enregistrement d'entité
/// </summary>
public record EntityRecordCreateDto(
    long EntityTypeId,
    string? Code,
    IReadOnlyList<AttributeValueUpsertDto> Values);

/// <summary>
/// Mise à jour d'un enregistrement d'entité
/// </summary>
public record EntityRecordUpdateDto(
    string? Code,
    IReadOnlyList<AttributeValueUpsertDto> Values);
