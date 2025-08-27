using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedOps.Domain.Entities;

namespace RedOps.Infrastructure.Data.Configurations;

public class WorkItemConfiguration : IEntityTypeConfiguration<WorkItem>
{
    public void Configure(EntityTypeBuilder<WorkItem> builder)
    {
        builder.ToTable("WorkItems", schema: "redops");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.ProjectId)
            .IsRequired();

        builder.Property(w => w.RedmineId)
            .HasConversion(
                v => v == null ? null : (int?)v.Value,
                v => v == null ? null : new Domain.ValueObjects.RedmineId(v.Value))
            .HasColumnName("RedmineId");

        builder.Property(w => w.AzureDevOpsId)
            .HasConversion(
                v => v == null ? null : (int?)v.Value,
                v => v == null ? null : new Domain.ValueObjects.AzureDevOpsId(v.Value))
            .HasColumnName("AzureDevOpsId");

        builder.Property(w => w.Title)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(w => w.Description)
            .HasMaxLength(4000);

        builder.Property(w => w.Status)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(w => w.Priority)
            .HasMaxLength(50);

        builder.Property(w => w.AssigneeEmail)
            .HasMaxLength(100);

        builder.Property(w => w.CreatedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(w => w.ModifiedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(w => w.LastSyncUtc)
            .HasColumnType("datetime2");

        builder.Property(w => w.RedmineData)
            .HasColumnType("nvarchar(max)");

        builder.Property(w => w.AzureDevOpsData)
            .HasColumnType("nvarchar(max)");

        // Indexes
        builder.HasIndex(w => w.ProjectId);
        builder.HasIndex(w => w.RedmineId)
            .IsUnique()
            .HasFilter("[RedmineId] IS NOT NULL");
        builder.HasIndex(w => w.AzureDevOpsId)
            .IsUnique()
            .HasFilter("[AzureDevOpsId] IS NOT NULL");
        builder.HasIndex(w => w.ModifiedUtc);
        builder.HasIndex(w => w.LastSyncUtc);

        // Relationships
        builder.HasMany<Comment>()
            .WithOne()
            .HasForeignKey(c => c.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<Attachment>()
            .WithOne()
            .HasForeignKey(a => a.WorkItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events for EF configuration
        builder.Ignore(w => w.DomainEvents);
    }
}