using Microsoft.Extensions.Logging;
using RedOps.Application.Common.Interfaces;
using RedOps.Domain.Enums;
using RedOps.Domain.Repositories;

namespace RedOps.Infrastructure.Services;

public class SyncOrchestrator : ISyncOrchestrator
{
    private readonly IProjectRepository _projectRepository;
    private readonly ISyncOperationRepository _syncOperationRepository;
    private readonly IRedmineService _redmineService;
    private readonly IAzureDevOpsService _azureDevOpsService;
    private readonly ILogger<SyncOrchestrator> _logger;
    private readonly Dictionary<Guid, CancellationTokenSource> _activeSyncs = new();

    public SyncOrchestrator(
        IProjectRepository projectRepository,
        ISyncOperationRepository syncOperationRepository,
        IRedmineService redmineService,
        IAzureDevOpsService azureDevOpsService,
        ILogger<SyncOrchestrator> logger)
    {
        _projectRepository = projectRepository;
        _syncOperationRepository = syncOperationRepository;
        _redmineService = redmineService;
        _azureDevOpsService = azureDevOpsService;
        _logger = logger;
    }

    public async Task<Guid> StartSyncAsync(Guid projectId, SyncDirection direction, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project {projectId} not found");
        }

        var syncOperation = Domain.Entities.SyncOperation.Create(
            projectId, 
            OperationType.Sync, 
            direction);

        await _syncOperationRepository.AddAsync(syncOperation, cancellationToken);

        _logger.LogInformation("Starting sync operation {OperationId} for project {ProjectId} in direction {Direction}",
            syncOperation.Id, projectId, direction);

        // Start background sync operation
        var syncCancellationToken = new CancellationTokenSource();
        _activeSyncs[projectId] = syncCancellationToken;

        _ = Task.Run(async () =>
        {
            try
            {
                await PerformSyncAsync(syncOperation, project, direction, syncCancellationToken.Token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Sync operation {OperationId} failed", syncOperation.Id);
                syncOperation.Fail(ex.Message);
                await _syncOperationRepository.UpdateAsync(syncOperation, CancellationToken.None);
            }
            finally
            {
                _activeSyncs.Remove(projectId);
                syncCancellationToken.Dispose();
            }
        }, cancellationToken);

        return syncOperation.Id;
    }

    public async Task<bool> IsSyncInProgressAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return _activeSyncs.ContainsKey(projectId);
    }

    public async Task StopSyncAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        if (_activeSyncs.TryGetValue(projectId, out var syncToken))
        {
            syncToken.Cancel();
            _logger.LogInformation("Sync operation cancelled for project {ProjectId}", projectId);
        }
    }

    public async Task<SyncStatus> GetSyncStatusAsync(Guid operationId, CancellationToken cancellationToken = default)
    {
        var operation = await _syncOperationRepository.GetByIdAsync(operationId, cancellationToken);
        return operation?.Status ?? SyncStatus.Failed;
    }

    private async Task PerformSyncAsync(Domain.Entities.SyncOperation syncOperation, Domain.Entities.Project project, SyncDirection direction, CancellationToken cancellationToken)
    {
        try
        {
            var itemsProcessed = 0;
            var errorCount = 0;

            if (direction.HasFlag(SyncDirection.FromRedmine) && project.RedmineId != null)
            {
                _logger.LogInformation("Syncing from Redmine for project {ProjectId}", project.Id);
                var result = await SyncFromRedmineAsync(project, cancellationToken);
                itemsProcessed += result.ItemsProcessed;
                errorCount += result.ErrorCount;
            }

            if (direction.HasFlag(SyncDirection.ToRedmine) && !string.IsNullOrEmpty(project.AzureDevOpsProject))
            {
                _logger.LogInformation("Syncing to Redmine for project {ProjectId}", project.Id);
                var result = await SyncToRedmineAsync(project, cancellationToken);
                itemsProcessed += result.ItemsProcessed;
                errorCount += result.ErrorCount;
            }

            syncOperation.UpdateProgress(itemsProcessed, errorCount);
            
            if (errorCount == 0)
            {
                syncOperation.Complete($"Successfully synced {itemsProcessed} items");
                project.UpdateSyncTimestamp();
                await _projectRepository.UpdateAsync(project, cancellationToken);
            }
            else
            {
                syncOperation.Complete($"Synced {itemsProcessed} items with {errorCount} errors");
            }

            await _syncOperationRepository.UpdateAsync(syncOperation, cancellationToken);

            _logger.LogInformation("Sync operation {OperationId} completed: {ItemsProcessed} items, {ErrorCount} errors",
                syncOperation.Id, itemsProcessed, errorCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync operation {OperationId} failed", syncOperation.Id);
            syncOperation.Fail(ex.Message);
            await _syncOperationRepository.UpdateAsync(syncOperation, cancellationToken);
        }
    }

    private async Task<SyncResult> SyncFromRedmineAsync(Domain.Entities.Project project, CancellationToken cancellationToken)
    {
        // Implementation for syncing from Redmine will be completed in next phase
        await Task.Delay(1000, cancellationToken); // Simulate work
        return new SyncResult { ItemsProcessed = 5, ErrorCount = 0 };
    }

    private async Task<SyncResult> SyncToRedmineAsync(Domain.Entities.Project project, CancellationToken cancellationToken)
    {
        // Implementation for syncing to Redmine will be completed in next phase
        await Task.Delay(1000, cancellationToken); // Simulate work
        return new SyncResult { ItemsProcessed = 3, ErrorCount = 0 };
    }

    private class SyncResult
    {
        public int ItemsProcessed { get; set; }
        public int ErrorCount { get; set; }
    }
}