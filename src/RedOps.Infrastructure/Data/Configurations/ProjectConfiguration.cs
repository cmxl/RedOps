using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedOps.Domain.Entities;

namespace RedOps.Infrastructure.Data.Configurations;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.ToTable("Projects", schema: "redops");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Description)
            .HasMaxLength(1000);

        builder.Property(p => p.RedmineId)
            .HasConversion(
                v => v == null ? null : (int?)v.Value,
                v => v == null ? null : new Domain.ValueObjects.RedmineId(v.Value))
            .HasColumnName("RedmineId");

        builder.Property(p => p.AzureDevOpsProject)
            .HasMaxLength(100);

        builder.Property(p => p.SyncDirection)
            .HasConversion<int>();

        builder.Property(p => p.LastSyncUtc)
            .HasColumnType("datetime2");

        builder.Property(p => p.CreatedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(p => p.ModifiedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.HasIndex(p => p.RedmineId)
            .IsUnique()
            .HasFilter("[RedmineId] IS NOT NULL");

        builder.HasIndex(p => p.AzureDevOpsProject)
            .IsUnique()
            .HasFilter("[AzureDevOpsProject] IS NOT NULL");

        builder.HasIndex(p => p.Name);

        // Relationships
        builder.HasMany<WorkItem>()
            .WithOne()
            .HasForeignKey(w => w.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<FieldMapping>()
            .WithOne()
            .HasForeignKey(fm => fm.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events for EF configuration
        builder.Ignore(p => p.DomainEvents);
    }
}