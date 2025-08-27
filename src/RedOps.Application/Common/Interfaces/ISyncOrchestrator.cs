using RedOps.Domain.Enums;

namespace RedOps.Application.Common.Interfaces;

public interface ISyncOrchestrator
{
    Task<Guid> StartSyncAsync(Guid projectId, SyncDirection direction, CancellationToken cancellationToken = default);
    Task<bool> IsSyncInProgressAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task StopSyncAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<SyncStatus> GetSyncStatusAsync(Guid operationId, CancellationToken cancellationToken = default);
}