using Mediator;
using Microsoft.Extensions.Logging;
using RedOps.Application.Projects.Commands.SyncProject;
using RedOps.Domain.Enums;

namespace RedOps.Worker.Services;

public class SyncJob
{
    private readonly IMediator _mediator;
    private readonly ILogger<SyncJob> _logger;

    public SyncJob(IMediator mediator, ILogger<SyncJob> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [Hangfire.Queue("sync")]
    public async Task ExecuteSyncAsync(Guid projectId, SyncDirection syncDirection)
    {
        _logger.LogInformation("Starting sync job for project {ProjectId} with direction {SyncDirection}",
            projectId, syncDirection);

        try
        {
            var command = new SyncProjectCommand(projectId, syncDirection);
            var operationId = await _mediator.Send(command);

            _logger.LogInformation("Sync job completed successfully for project {ProjectId}. Operation ID: {OperationId}",
                projectId, operationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync job failed for project {ProjectId}", projectId);
            throw; // Re-throw to let Hangfire handle retries
        }
    }

    [Hangfire.Queue("sync")]
    public async Task ExecuteIncrementalSyncAsync(Guid projectId, DateTime since)
    {
        _logger.LogInformation("Starting incremental sync job for project {ProjectId} since {Since}",
            projectId, since);

        try
        {
            // In a real implementation, you would have an incremental sync command
            var command = new SyncProjectCommand(projectId, SyncDirection.Bidirectional);
            var operationId = await _mediator.Send(command);

            _logger.LogInformation("Incremental sync job completed successfully for project {ProjectId}. Operation ID: {OperationId}",
                projectId, operationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Incremental sync job failed for project {ProjectId}", projectId);
            throw; // Re-throw to let Hangfire handle retries
        }
    }

    [Hangfire.Queue("outbox")]
    public async Task ProcessFailedOutboxEventsAsync()
    {
        _logger.LogInformation("Processing failed outbox events");

        try
        {
            // This would be called by a separate recurring job for failed events
            // Implementation would retry failed outbox events
            await Task.Delay(1000); // Placeholder
            
            _logger.LogInformation("Failed outbox events processing completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process failed outbox events");
            throw;
        }
    }
}