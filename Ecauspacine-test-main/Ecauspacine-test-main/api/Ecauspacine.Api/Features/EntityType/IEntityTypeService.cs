using Ecauspacine.Contracts.EntityTypes;

namespace Ecauspacine.Api.Features.EntityTypes;

/// <summary>
/// Contrat du service applicatif pour gérer les EntityType.
/// </summary>
public interface IEntityTypeService
{
    Task<IReadOnlyList<EntityTypeDto>> ListAsync(CancellationToken ct = default);
    Task<EntityTypeDto> CreateAsync(EntityTypeCreateDto dto, CancellationToken ct = default);
    Task<EntityTypeDto?> UpdateAsync(long id, EntityTypeUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
}
