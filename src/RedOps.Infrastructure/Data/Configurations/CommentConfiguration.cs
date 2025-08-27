using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedOps.Domain.Entities;

namespace RedOps.Infrastructure.Data.Configurations;

public class CommentConfiguration : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable("Comments", schema: "redops");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.WorkItemId)
            .IsRequired();

        builder.Property(c => c.RedmineId)
            .HasConversion(
                v => v == null ? null : (int?)v.Value,
                v => v == null ? null : new Domain.ValueObjects.RedmineId(v.Value))
            .HasColumnName("RedmineId");

        builder.Property(c => c.AzureDevOpsId)
            .HasConversion(
                v => v == null ? null : (int?)v.Value,
                v => v == null ? null : new Domain.ValueObjects.AzureDevOpsId(v.Value))
            .HasColumnName("AzureDevOpsId");

        builder.Property(c => c.Content)
            .IsRequired()
            .HasMaxLength(4000);

        builder.Property(c => c.AuthorEmail)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(c => c.CreatedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(c => c.LastSyncUtc)
            .HasColumnType("datetime2");

        // Indexes
        builder.HasIndex(c => c.WorkItemId);
        builder.HasIndex(c => c.CreatedUtc);
    }
}