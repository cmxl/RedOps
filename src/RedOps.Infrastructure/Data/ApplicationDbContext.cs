using Microsoft.EntityFrameworkCore;
using RedOps.Application.Common.Interfaces;
using RedOps.Domain.Entities;
using RedOps.Infrastructure.Data.Configurations;

namespace RedOps.Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<WorkItem> WorkItems => Set<WorkItem>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Attachment> Attachments => Set<Attachment>();
    public DbSet<SyncOperation> SyncOperations => Set<SyncOperation>();
    public DbSet<SyncConflict> SyncConflicts => Set<SyncConflict>();
    public DbSet<FieldMapping> FieldMappings => Set<FieldMapping>();
    public DbSet<OutboxEvent> OutboxEvents => Set<OutboxEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ProjectConfiguration());
        modelBuilder.ApplyConfiguration(new WorkItemConfiguration());
        modelBuilder.ApplyConfiguration(new CommentConfiguration());
        modelBuilder.ApplyConfiguration(new AttachmentConfiguration());
        modelBuilder.ApplyConfiguration(new SyncOperationConfiguration());
        modelBuilder.ApplyConfiguration(new SyncConflictConfiguration());
        modelBuilder.ApplyConfiguration(new FieldMappingConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxEventConfiguration());

        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Dispatch domain events before saving
        await DispatchDomainEventsAsync();
        
        return await base.SaveChangesAsync(cancellationToken);
    }

    private async Task DispatchDomainEventsAsync()
    {
        var domainEntities = ChangeTracker
            .Entries<Domain.Common.AggregateRoot>()
            .Where(x => x.Entity.DomainEvents.Any())
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.Entity.DomainEvents)
            .ToList();

        domainEntities.ForEach(entity => entity.Entity.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            // Create outbox events for reliable processing
            var outboxEvent = OutboxEvent.Create(
                domainEvent.GetType().Name,
                domainEvent.EventId.ToString(),
                System.Text.Json.JsonSerializer.Serialize(domainEvent, domainEvent.GetType())
            );

            OutboxEvents.Add(outboxEvent);
        }
    }
}