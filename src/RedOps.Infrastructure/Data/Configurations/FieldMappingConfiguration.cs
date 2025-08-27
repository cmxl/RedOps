using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedOps.Domain.Entities;

namespace RedOps.Infrastructure.Data.Configurations;

public class FieldMappingConfiguration : IEntityTypeConfiguration<FieldMapping>
{
    public void Configure(EntityTypeBuilder<FieldMapping> builder)
    {
        builder.ToTable("FieldMappings", schema: "redops");

        builder.HasKey(fm => fm.Id);

        builder.Property(fm => fm.ProjectId)
            .IsRequired();

        builder.Property(fm => fm.RedmineField)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fm => fm.AzureDevOpsField)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(fm => fm.MappingRule)
            .HasMaxLength(500);

        builder.Property(fm => fm.IsActive)
            .IsRequired();

        builder.Property(fm => fm.CreatedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(fm => fm.ModifiedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        // Indexes
        builder.HasIndex(fm => fm.ProjectId);
        builder.HasIndex(fm => new { fm.ProjectId, fm.RedmineField, fm.AzureDevOpsField })
            .IsUnique();
        builder.HasIndex(fm => fm.IsActive);
    }
}