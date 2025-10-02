using Ecauspacine.Api.Infrastructure;
using Ecauspacine.Api.Mapping;
using Ecauspacine.Contracts.Lookups;
using Microsoft.EntityFrameworkCore;

namespace Ecauspacine.Api.Features.Lookups;

public class LookupService : ILookupService
{
    private readonly EcauspacineDbContext _db;
    public LookupService(EcauspacineDbContext db) => _db = db;

    // ===== GROUPS =====
    public async Task<IReadOnlyList<LookupGroupDto>> ListGroupsAsync(bool includeItems = false, CancellationToken ct = default)
    {
        if (!includeItems)
        {
            var groups = await _db.LookupGroups
                .AsNoTracking()
                .OrderBy(g => g.Code)
                .ToListAsync(ct);

            return groups.Select(g => g.ToDto()).ToList();
        }
        else
        {
            // ✅ Evite GroupBy server-side : on inclut les items puis on mappe en mémoire
            var groupsWithItems = await _db.LookupGroups
                .AsNoTracking()
                .Include(g => g.Items)
                .OrderBy(g => g.Code)
                .ToListAsync(ct);

            return groupsWithItems.Select(g =>
            {
                var items = g.Items
                    .OrderBy(i => i.OrderIndex)
                    .ThenBy(i => i.Code)
                    .Select(i => i.ToDto())
                    .ToList()
                    .AsReadOnly();

                return g.ToDto(items);
            }).ToList();
        }
    }

    public async Task<LookupGroupDto> CreateGroupAsync(LookupGroupCreateDto dto, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Code)) throw new ArgumentException("Code requis.");
        if (string.IsNullOrWhiteSpace(dto.Label)) throw new ArgumentException("Label requis.");

        var exists = await _db.LookupGroups.AnyAsync(g => g.Code == dto.Code, ct);
        if (exists) throw new InvalidOperationException($"Groupe '{dto.Code}' existe déjà.");

        var entity = dto.ToEntity();
        _db.LookupGroups.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<LookupGroupDto?> UpdateGroupAsync(long id, LookupGroupUpdateDto dto, CancellationToken ct = default)
    {
        var g = await _db.LookupGroups.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (g is null) return null;

        if (dto.Label is not null)
        {
            if (string.IsNullOrWhiteSpace(dto.Label)) throw new ArgumentException("Label ne peut pas être vide.");
            g.Label = dto.Label!;
        }
        if (dto.Description is not null) g.Description = dto.Description;

        await _db.SaveChangesAsync(ct);
        return g.ToDto();
    }

    public async Task<bool> DeleteGroupAsync(long id, CancellationToken ct = default)
    {
        var g = await _db.LookupGroups.Include(x => x.Items).FirstOrDefaultAsync(x => x.Id == id, ct);
        if (g is null) return false;
        if (g.Items.Any())
            throw new DbUpdateException("Impossible de supprimer un groupe contenant des items.");

        _db.LookupGroups.Remove(g);
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ===== ITEMS =====
    public async Task<IReadOnlyList<LookupItemDto>> ListItemsAsync(long groupId, CancellationToken ct = default)
        => (await _db.LookupItems.AsNoTracking()
               .Where(i => i.GroupId == groupId)
               .OrderBy(i => i.OrderIndex).ThenBy(i => i.Code)
               .ToListAsync(ct))
           .Select(i => i.ToDto()).ToList();

    public async Task<LookupItemDto> CreateItemAsync(LookupItemCreateDto dto, CancellationToken ct = default)
    {
        var groupExists = await _db.LookupGroups.AnyAsync(g => g.Id == dto.GroupId, ct);
        if (!groupExists) throw new KeyNotFoundException("Groupe introuvable.");

        if (string.IsNullOrWhiteSpace(dto.Code)) throw new ArgumentException("Code requis.");
        if (string.IsNullOrWhiteSpace(dto.Label)) throw new ArgumentException("Label requis.");

        var conflict = await _db.LookupItems.AnyAsync(i => i.GroupId == dto.GroupId && i.Code == dto.Code, ct);
        if (conflict) throw new InvalidOperationException($"Item '{dto.Code}' existe déjà dans ce groupe.");

        var entity = dto.ToEntity();
        _db.LookupItems.Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.ToDto();
    }

    public async Task<LookupItemDto?> UpdateItemAsync(long itemId, LookupItemUpdateDto dto, CancellationToken ct = default)
    {
        var i = await _db.LookupItems.FirstOrDefaultAsync(x => x.Id == itemId, ct);
        if (i is null) return null;

        if (dto.Label is not null)
        {
            if (string.IsNullOrWhiteSpace(dto.Label)) throw new ArgumentException("Label ne peut pas être vide.");
            i.Label = dto.Label!;
        }
        if (dto.Value is not null) i.Value = dto.Value;
        if (dto.OrderIndex is not null) i.OrderIndex = dto.OrderIndex.Value;
        if (dto.Description is not null) i.Description = dto.Description;

        await _db.SaveChangesAsync(ct);
        return i.ToDto();
    }

    public async Task<bool> DeleteItemAsync(long itemId, CancellationToken ct = default)
    {
        var i = await _db.LookupItems.FirstOrDefaultAsync(x => x.Id == itemId, ct);
        if (i is null) return false;

        _db.LookupItems.Remove(i);
        await _db.SaveChangesAsync(ct);
        return true;
    }
}
