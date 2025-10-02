using Ecauspacine.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecauspacine.Api.Infrastructure.Configurations;

/// <summary>
/// Mapping EF Core de la table entity_type (MySQL).
/// Aligne les champs du schéma : code unique, label not null, description text.
/// </summary>
public class EntityTypeConfiguration : IEntityTypeConfiguration<EntityType>
{
    public void Configure(EntityTypeBuilder<EntityType> b)
    {
        b.ToTable("entity_type");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();

        // Code : requis, longueur 255, index unique
        b.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(255);
        b.HasIndex(x => x.Code).IsUnique();

        // Label : requis, longueur 255
        b.Property(x => x.Label)
            .IsRequired()
            .HasMaxLength(255);

        // Description : TEXT
        b.Property(x => x.Description)
            .HasColumnType("text");
    }
}
