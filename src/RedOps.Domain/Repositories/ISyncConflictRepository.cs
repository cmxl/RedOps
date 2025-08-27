using RedOps.Domain.Entities;

namespace RedOps.Domain.Repositories;

public interface ISyncConflictRepository
{
    Task<SyncConflict?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SyncConflict>> GetByWorkItemIdAsync(Guid workItemId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SyncConflict>> GetUnresolvedConflictsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<SyncConflict>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task AddAsync(SyncConflict conflict, CancellationToken cancellationToken = default);
    Task UpdateAsync(SyncConflict conflict, CancellationToken cancellationToken = default);
    Task<int> CountUnresolvedByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
}