using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedOps.Worker.Configuration;

namespace RedOps.Worker.Services;

public class HealthCheckService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<HealthCheckService> _logger;
    private readonly HealthCheckOptions _options;

    public HealthCheckService(
        IServiceProvider serviceProvider,
        ILogger<HealthCheckService> logger,
        IOptions<HealthCheckOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Health Check Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await PerformHealthChecksAsync(stoppingToken);
                await Task.Delay(TimeSpan.FromMinutes(_options.CheckIntervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during health checks");
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Wait before retrying
            }
        }

        _logger.LogInformation("Health Check Service stopped");
    }

    private async Task PerformHealthChecksAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var healthCheckService = scope.ServiceProvider.GetRequiredService<HealthCheckService>();

        try
        {
            var healthCheckContext = new HealthCheckContext();
            
            // Check database connectivity
            await CheckDatabaseHealthAsync(scope, cancellationToken);
            
            // Check external API connectivity
            await CheckExternalApisHealthAsync(scope, cancellationToken);
            
            // Check background job processing
            await CheckBackgroundJobsHealthAsync(cancellationToken);
            
            // Check memory and performance metrics
            CheckSystemHealthMetrics();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Health check failed");
        }
    }

    private async Task CheckDatabaseHealthAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        try
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<RedOps.Infrastructure.Data.ApplicationDbContext>();
            
            // Simple connectivity check
            var canConnect = await dbContext.Database.CanConnectAsync(cancellationToken);
            
            if (canConnect)
            {
                _logger.LogDebug("Database health check passed");
            }
            else
            {
                _logger.LogWarning("Database health check failed - cannot connect");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database health check failed with exception");
        }
    }

    private async Task CheckExternalApisHealthAsync(IServiceScope scope, CancellationToken cancellationToken)
    {
        // Check Redmine API
        try
        {
            var redmineService = scope.ServiceProvider.GetRequiredService<RedOps.Application.Common.Interfaces.IRedmineService>();
            
            // Try to get projects as a health check
            await redmineService.GetProjectsAsync(cancellationToken);
            _logger.LogDebug("Redmine API health check passed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Redmine API health check failed");
        }

        // Check Azure DevOps API
        try
        {
            var azureDevOpsService = scope.ServiceProvider.GetRequiredService<RedOps.Application.Common.Interfaces.IAzureDevOpsService>();
            
            // Try to get projects as a health check
            await azureDevOpsService.GetProjectsAsync(cancellationToken);
            _logger.LogDebug("Azure DevOps API health check passed");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Azure DevOps API health check failed");
        }
    }

    private async Task CheckBackgroundJobsHealthAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Check if background jobs are processing properly
            // This could involve checking Hangfire statistics or job counts
            
            _logger.LogDebug("Background jobs health check completed");
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Background jobs health check failed");
        }
    }

    private void CheckSystemHealthMetrics()
    {
        try
        {
            // Check memory usage
            var workingSet = GC.GetTotalMemory(false);
            var workingSetMB = workingSet / (1024 * 1024);
            
            _logger.LogDebug("Current memory usage: {MemoryMB} MB", workingSetMB);
            
            if (workingSetMB > _options.MaxMemoryMB)
            {
                _logger.LogWarning("Memory usage ({MemoryMB} MB) exceeds threshold ({ThresholdMB} MB)",
                    workingSetMB, _options.MaxMemoryMB);
            }

            // Check CPU and other system metrics as needed
            // This could be extended with performance counters
            
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "System health metrics check failed");
        }
    }
}