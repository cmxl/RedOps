using RedOps.Domain.Entities;
using RedOps.Domain.ValueObjects;

namespace RedOps.Domain.Repositories;

public interface IWorkItemRepository
{
    Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<WorkItem?> GetByRedmineIdAsync(RedmineId redmineId, CancellationToken cancellationToken = default);
    Task<WorkItem?> GetByAzureDevOpsIdAsync(AzureDevOpsId azureDevOpsId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkItem>> GetModifiedSinceAsync(DateTime since, CancellationToken cancellationToken = default);
    Task<IEnumerable<WorkItem>> GetPendingSyncAsync(CancellationToken cancellationToken = default);
    Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<int> CountByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default);
}