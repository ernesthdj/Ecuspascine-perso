using Ecauspacine.Api.Features.EntityTypes;
using Ecauspacine.Api.Infrastructure;
using Ecauspacine.Api.Mapping;
using Ecauspacine.Contracts.EntityTypes;
using Microsoft.EntityFrameworkCore;

namespace Ecauspacine.Api.Features.EntityType;

/// <summary>
/// Service applicatif : logique autour d'EntityType (validation, accès DB).
/// </summary>
public class EntityTypeService : IEntityTypeService
{
    private readonly EcauspacineDbContext _db;

    public EntityTypeService(EcauspacineDbContext db) => _db = db;

    public async Task<IReadOnlyList<EntityTypeDto>> ListAsync(CancellationToken ct = default)
    {
        var items = await _db.EntityTypes
            .AsNoTracking()
            .OrderBy(e => e.Code)
            .ToListAsync(ct);

        return items.Select(x => x.ToDto()).ToList();
    }

    public async Task<EntityTypeDto> CreateAsync(EntityTypeCreateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException("Code est requis.");
        if (string.IsNullOrWhiteSpace(dto.Label))
            throw new ArgumentException("Label est requis.");

        var exists = await _db.EntityTypes.AnyAsync(e => e.Code == dto.Code, ct);
        if (exists)
            throw new InvalidOperationException($"EntityType avec Code '{dto.Code}' existe déjà.");

        var entity = dto.ToEntity(); // dto.Label et dto.Code sont non-null ici (validés)
        _db.EntityTypes.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<EntityTypeDto?> UpdateAsync(long id, EntityTypeUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _db.EntityTypes.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null) return null;

        // Renommage du Code (dto.Code est nullable)
        if (dto.Code is not null && !dto.Code.Equals(entity.Code, StringComparison.Ordinal))
        {
            if (string.IsNullOrWhiteSpace(dto.Code))
                throw new ArgumentException("Code ne peut pas être vide.");
            var conflict = await _db.EntityTypes.AnyAsync(e => e.Code == dto.Code, ct);
            if (conflict)
                throw new InvalidOperationException($"EntityType avec Code '{dto.Code}' existe déjà.");
            entity.Code = dto.Code!; // validé non-null + non-vide ci-dessus
        }

        // Mise à jour du Label (dto.Label est nullable)
        if (dto.Label is not null)
        {
            if (string.IsNullOrWhiteSpace(dto.Label))
                throw new ArgumentException("Label ne peut pas être vide.");
            entity.Label = dto.Label!; // validé non-null + non-vide
        }

        // Description peut être null pour effacer ou string pour modifier
        if (dto.Description is not null)
        {
            entity.Description = dto.Description; // ok: propriété nullable
        }

        await _db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var entity = await _db.EntityTypes.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null) return false;

        _db.EntityTypes.Remove(entity);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

