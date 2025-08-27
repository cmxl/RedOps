using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedOps.Application.Projects.Commands.SyncProject;
using RedOps.Domain.Repositories;
using RedOps.Worker.Configuration;
using Hangfire;

namespace RedOps.Worker.Services;

public class SyncSchedulerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<SyncSchedulerService> _logger;
    private readonly SyncSchedulerOptions _options;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public SyncSchedulerService(
        IServiceProvider serviceProvider,
        ILogger<SyncSchedulerService> logger,
        IOptions<SyncSchedulerOptions> options,
        IBackgroundJobClient backgroundJobClient)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
        _backgroundJobClient = backgroundJobClient;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sync Scheduler Service started");

        // Schedule recurring jobs
        SetupRecurringJobs();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ScheduleSyncOperationsAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(_options.ScheduleIntervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while scheduling sync operations");
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken); // Wait before retrying
            }
        }

        _logger.LogInformation("Sync Scheduler Service stopped");
    }

    private void SetupRecurringJobs()
    {
        // Schedule automatic sync for all active projects
        RecurringJob.AddOrUpdate<SyncSchedulerService>(
            "automatic-project-sync",
            service => service.ExecuteProjectSyncAsync(),
            _options.AutoSyncCronExpression);

        // Schedule cleanup of old sync operations
        RecurringJob.AddOrUpdate<SyncSchedulerService>(
            "cleanup-old-operations",
            service => service.CleanupOldOperationsAsync(),
            Cron.Daily(2)); // Run at 2 AM daily

        _logger.LogInformation("Recurring sync jobs scheduled");
    }

    private async Task ScheduleSyncOperationsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();

        var activeProjects = await projectRepository.GetActiveProjectsAsync(cancellationToken);

        foreach (var project in activeProjects)
        {
            if (ShouldScheduleSync(project))
            {
                var jobId = _backgroundJobClient.Enqueue<SyncJob>(
                    job => job.ExecuteSyncAsync(project.Id, project.SyncDirection));

                _logger.LogInformation("Scheduled sync job {JobId} for project {ProjectName} ({ProjectId})",
                    jobId, project.Name, project.Id);
            }
        }
    }

    private bool ShouldScheduleSync(Domain.Entities.Project project)
    {
        if (project.LastSyncUtc == null)
        {
            return true; // Never synced
        }

        var timeSinceLastSync = DateTime.UtcNow - project.LastSyncUtc.Value;
        return timeSinceLastSync >= TimeSpan.FromMinutes(_options.MinimumSyncIntervalMinutes);
    }

    [Hangfire.Queue("sync")]
    public async Task ExecuteProjectSyncAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var projectRepository = scope.ServiceProvider.GetRequiredService<IProjectRepository>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var activeProjects = await projectRepository.GetActiveProjectsAsync();

        _logger.LogInformation("Starting automatic sync for {ProjectCount} active projects", activeProjects.Count());

        foreach (var project in activeProjects)
        {
            try
            {
                if (ShouldScheduleSync(project))
                {
                    var command = new SyncProjectCommand(project.Id, project.SyncDirection);
                    await mediator.Send(command);

                    _logger.LogInformation("Triggered sync for project {ProjectName} ({ProjectId})",
                        project.Name, project.Id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to sync project {ProjectName} ({ProjectId})",
                    project.Name, project.Id);
            }
        }
    }

    [Hangfire.Queue("default")]
    public async Task CleanupOldOperationsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var syncOperationRepository = scope.ServiceProvider.GetRequiredService<ISyncOperationRepository>();

        _logger.LogInformation("Starting cleanup of old sync operations");

        try
        {
            var cutoffDate = DateTime.UtcNow.AddDays(-_options.OperationRetentionDays);
            var oldOperations = await syncOperationRepository.GetRecentOperationsAsync(1000);
            
            var operationsToCleanup = oldOperations
                .Where(op => op.StartTime < cutoffDate && op.Status != Domain.Enums.SyncStatus.InProgress)
                .ToList();

            foreach (var operation in operationsToCleanup)
            {
                // In a real implementation, you might want to archive instead of delete
                _logger.LogDebug("Would archive/cleanup operation {OperationId} from {StartTime}",
                    operation.Id, operation.StartTime);
            }

            _logger.LogInformation("Cleanup completed. {Count} operations processed", operationsToCleanup.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cleanup old sync operations");
        }
    }
}