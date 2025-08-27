using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedOps.Domain.Entities;

namespace RedOps.Infrastructure.Data.Configurations;

public class SyncOperationConfiguration : IEntityTypeConfiguration<SyncOperation>
{
    public void Configure(EntityTypeBuilder<SyncOperation> builder)
    {
        builder.ToTable("SyncOperations", schema: "redops");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.ProjectId)
            .IsRequired();

        builder.Property(s => s.OperationType)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.SyncDirection)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.StartTime)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(s => s.EndTime)
            .HasColumnType("datetime2");

        builder.Property(s => s.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(s => s.ItemsProcessed)
            .IsRequired();

        builder.Property(s => s.ErrorCount)
            .IsRequired();

        builder.Property(s => s.Details)
            .HasMaxLength(2000);

        builder.Property(s => s.ErrorMessage)
            .HasMaxLength(2000);

        // Indexes
        builder.HasIndex(s => s.ProjectId);
        builder.HasIndex(s => s.Status);
        builder.HasIndex(s => s.StartTime);
        builder.HasIndex(s => new { s.ProjectId, s.Status });
    }
}