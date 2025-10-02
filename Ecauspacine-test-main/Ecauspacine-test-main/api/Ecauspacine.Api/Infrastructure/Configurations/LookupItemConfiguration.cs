using Ecauspacine.Api.Domain.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ecauspacine.Api.Infrastructure.Configurations;

public class LookupItemConfiguration : IEntityTypeConfiguration<LookupItem>
{
    public void Configure(EntityTypeBuilder<LookupItem> b)
    {
        b.ToTable("lookup_item");

        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedOnAdd();

        b.Property(x => x.Code).IsRequired().HasMaxLength(255);
        b.Property(x => x.Label).IsRequired().HasMaxLength(255);
        b.Property(x => x.Description).HasColumnType("text");

        b.Property(x => x.Value); // int? nullable
        b.Property(x => x.OrderIndex).IsRequired().HasDefaultValue(1);

        // Unicité (group_id, code)
        b.HasIndex(x => new { x.GroupId, x.Code }).IsUnique();
    }
}
