using Microsoft.EntityFrameworkCore;
using RedOps.Domain.Entities;

namespace RedOps.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Project> Projects { get; }
    DbSet<WorkItem> WorkItems { get; }
    DbSet<Comment> Comments { get; }
    DbSet<Attachment> Attachments { get; }
    DbSet<SyncOperation> SyncOperations { get; }
    DbSet<SyncConflict> SyncConflicts { get; }
    DbSet<FieldMapping> FieldMappings { get; }
    DbSet<OutboxEvent> OutboxEvents { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}