using RedOps.Domain.Entities;
using RedOps.Domain.Enums;

namespace RedOps.Domain.Repositories;

public interface ISyncOperationRepository
{
    Task<SyncOperation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<SyncOperation>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<SyncOperation>> GetRecentOperationsAsync(int count = 50, CancellationToken cancellationToken = default);
    Task<IEnumerable<SyncOperation>> GetByStatusAsync(SyncStatus status, CancellationToken cancellationToken = default);
    Task AddAsync(SyncOperation syncOperation, CancellationToken cancellationToken = default);
    Task UpdateAsync(SyncOperation syncOperation, CancellationToken cancellationToken = default);
}