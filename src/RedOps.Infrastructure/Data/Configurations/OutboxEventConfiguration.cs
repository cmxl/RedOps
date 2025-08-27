using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedOps.Domain.Entities;

namespace RedOps.Infrastructure.Data.Configurations;

public class OutboxEventConfiguration : IEntityTypeConfiguration<OutboxEvent>
{
    public void Configure(EntityTypeBuilder<OutboxEvent> builder)
    {
        builder.ToTable("OutboxEvents", schema: "redops");

        builder.HasKey(oe => oe.Id);

        builder.Property(oe => oe.EventType)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(oe => oe.AggregateId)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(oe => oe.EventData)
            .IsRequired()
            .HasColumnType("nvarchar(max)");

        builder.Property(oe => oe.CreatedUtc)
            .HasColumnType("datetime2")
            .IsRequired();

        builder.Property(oe => oe.ProcessedUtc)
            .HasColumnType("datetime2");

        builder.Property(oe => oe.RetryCount)
            .IsRequired();

        builder.Property(oe => oe.LastError)
            .HasMaxLength(2000);

        builder.Property(oe => oe.IsProcessed)
            .IsRequired();

        // Indexes for outbox pattern processing
        builder.HasIndex(oe => oe.IsProcessed);
        builder.HasIndex(oe => oe.CreatedUtc);
        builder.HasIndex(oe => new { oe.IsProcessed, oe.CreatedUtc });
        builder.HasIndex(oe => new { oe.IsProcessed, oe.RetryCount });
    }
}