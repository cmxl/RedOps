using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedOps.Domain.Entities;

namespace RedOps.Infrastructure.Data.Configurations;

public class SyncConflictConfiguration : IEntityTypeConfiguration<SyncConflict>
{
    public void Configure(EntityTypeBuilder<SyncConflict> builder)
    {
        builder.ToTable("SyncConflicts", schema: "redops");

        builder.HasKey(sc => sc.Id);

        builder.Property(sc => sc.WorkItemId)
            .IsRequired();

        builder.Property(sc => sc.ConflictType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(sc => sc.RedmineData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(sc => sc.AzureDevOpsData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(sc => sc.Resolution)
            .HasMaxLength(2000);

        builder.Property(sc => sc.ResolvedBy)
            .HasMaxLength(100);

        builder.Property(sc => sc.CreatedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(sc => sc.ResolvedUtc)
            .HasColumnType("datetime2");

        builder.Property(sc => sc.IsResolved)
            .IsRequired();

        // Indexes
        builder.HasIndex(sc => sc.WorkItemId);
        builder.HasIndex(sc => sc.IsResolved);
        builder.HasIndex(sc => sc.CreatedUtc);
        builder.HasIndex(sc => new { sc.IsResolved, sc.CreatedUtc });
    }
}