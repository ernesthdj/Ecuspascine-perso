using Ecauspacine.Api.Domain;
using Ecauspacine.Api.Domain.Common;
using Ecauspacine.Api.Domain.Lookups;
using Microsoft.EntityFrameworkCore;

namespace Ecauspacine.Api.Infrastructure;

/// <summary>
/// DbContext EF Core : expose les DbSet et applique les configurations Fluent API.
/// </summary>
public class EcauspacineDbContext : DbContext
{
    public EcauspacineDbContext(DbContextOptions<EcauspacineDbContext> options) : base(options) { }

    // DbSet principal
    public DbSet<EntityType> EntityTypes => Set<EntityType>();
    public DbSet<LookupItem> LookupItems => Set<LookupItem>();
    public DbSet<LookupGroup> LookupGroups => Set<LookupGroup>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Charge toutes les IEntityTypeConfiguration<> du même assembly.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EcauspacineDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Met à jour CreatedAtUtc/UpdatedAtUtc automatiquement pour toutes les entités BaseEntity.
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => base.SaveChangesAsync(cancellationToken);
}
