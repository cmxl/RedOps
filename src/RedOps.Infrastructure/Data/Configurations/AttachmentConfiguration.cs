using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedOps.Domain.Entities;

namespace RedOps.Infrastructure.Data.Configurations;

public class AttachmentConfiguration : IEntityTypeConfiguration<Attachment>
{
    public void Configure(EntityTypeBuilder<Attachment> builder)
    {
        builder.ToTable("Attachments", schema: "redops");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.WorkItemId)
            .IsRequired();

        builder.Property(a => a.RedmineId)
            .HasConversion(
                v => v == null ? null : (int?)v.Value,
                v => v == null ? null : new Domain.ValueObjects.RedmineId(v.Value))
            .HasColumnName("RedmineId");

        builder.Property(a => a.AzureDevOpsId)
            .HasConversion(
                v => v == null ? null : (int?)v.Value,
                v => v == null ? null : new Domain.ValueObjects.AzureDevOpsId(v.Value))
            .HasColumnName("AzureDevOpsId");

        builder.Property(a => a.FileName)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.ContentType)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(a => a.FileSize)
            .IsRequired();

        builder.Property(a => a.FileUrl)
            .HasMaxLength(1000);

        builder.Property(a => a.CreatedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(a => a.LastSyncUtc)
            .HasColumnType("datetime2");

        // Indexes
        builder.HasIndex(a => a.WorkItemId);
        builder.HasIndex(a => a.FileName);
    }
}