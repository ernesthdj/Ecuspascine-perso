using Ecauspacine.Api.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecauspacine.Api.Infrastructure.Configurations;

public class LookupGroupConfiguration : IEntityTypeConfiguration<LookupGroup>
{
    public void Configure(EntityTypeBuilder<LookupGroup> b)
    {
        b.ToTable("lookup_group");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();

        b.Property(x => x.Code).IsRequired().HasMaxLength(255);
        b.HasIndex(x => x.Code).IsUnique(); // unicité globale du groupe

        b.Property(x => x.Label).IsRequired().HasMaxLength(255);
        b.Property(x => x.Description).HasColumnType("text");


        b.HasMany(x => x.Items)
         .WithOne(i => i.Group)
         .HasForeignKey(i => i.GroupId)
         .OnDelete(DeleteBehavior.Restrict); // évite suppression groupe avec items
    }
}
