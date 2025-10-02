using Ecauspacine.Api.Domain;
using Ecauspacine.Contracts.EntityTypes;

namespace Ecauspacine.Api.Mapping;

/// <summary>
/// Mapping Domain ↔ DTOs (DTOs = records positionnels).
/// </summary>
public static class EntityTypeMapping
{
    /// <summary>
    /// Domain → DTO (utilise le constructeur positionnel des records).
    /// </summary>
    public static EntityTypeDto ToDto(this EntityType e)
        => new EntityTypeDto(
            e.Id,
            e.Code,
            e.Label,
            e.Description
        );

    /// <summary>
    /// CreateDto → Domain (records positionnels = arg par nom).
    /// </summary>
    public static EntityType ToEntity(this EntityTypeCreateDto dto)
        => new EntityType
        {
            Code = dto.Code,
            Label = dto.Label,
            Description = dto.Description
        };
}
