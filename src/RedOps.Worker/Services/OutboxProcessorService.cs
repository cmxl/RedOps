using Mediator;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RedOps.Domain.Repositories;
using RedOps.Worker.Configuration;
using System.Text.Json;

namespace RedOps.Worker.Services;

public class OutboxProcessorService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OutboxProcessorService> _logger;
    private readonly OutboxProcessorOptions _options;
    private readonly TimeSpan _processingInterval;

    public OutboxProcessorService(
        IServiceProvider serviceProvider,
        ILogger<OutboxProcessorService> logger,
        IOptions<OutboxProcessorOptions> options)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _options = options.Value;
        _processingInterval = TimeSpan.FromSeconds(_options.ProcessingIntervalSeconds);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Outbox Processor Service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxEventsAsync(stoppingToken);
                await Task.Delay(_processingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                // Expected when cancellation is requested
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while processing outbox events");
                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken); // Wait before retrying
            }
        }

        _logger.LogInformation("Outbox Processor Service stopped");
    }

    private async Task ProcessOutboxEventsAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxEventRepository>();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

        var unprocessedEvents = await outboxRepository.GetUnprocessedEventsAsync(
            _options.BatchSize, cancellationToken);

        if (!unprocessedEvents.Any())
        {
            return;
        }

        _logger.LogInformation("Processing {Count} outbox events", unprocessedEvents.Count());

        foreach (var outboxEvent in unprocessedEvents)
        {
            try
            {
                await ProcessSingleEventAsync(outboxEvent, mediator, cancellationToken);
                
                outboxEvent.MarkAsProcessed();
                await outboxRepository.UpdateAsync(outboxEvent, cancellationToken);
                
                _logger.LogDebug("Successfully processed outbox event {EventId} of type {EventType}", 
                    outboxEvent.Id, outboxEvent.EventType);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process outbox event {EventId} of type {EventType}", 
                    outboxEvent.Id, outboxEvent.EventType);

                outboxEvent.IncrementRetryCount(ex.Message);
                await outboxRepository.UpdateAsync(outboxEvent, cancellationToken);

                if (outboxEvent.RetryCount >= _options.MaxRetryAttempts)
                {
                    _logger.LogError("Outbox event {EventId} exceeded maximum retry attempts ({MaxRetries}). Moving to dead letter queue.",
                        outboxEvent.Id, _options.MaxRetryAttempts);
                }
            }
        }

        // Clean up old processed events
        await CleanupProcessedEventsAsync(outboxRepository, cancellationToken);
    }

    private async Task ProcessSingleEventAsync(Domain.Entities.OutboxEvent outboxEvent, 
        IMediator mediator, CancellationToken cancellationToken)
    {
        var eventType = Type.GetType(outboxEvent.EventType);
        if (eventType == null)
        {
            _logger.LogWarning("Unknown event type: {EventType}", outboxEvent.EventType);
            return;
        }

        var domainEvent = JsonSerializer.Deserialize(outboxEvent.EventData, eventType);
        if (domainEvent == null)
        {
            _logger.LogWarning("Failed to deserialize event data for event {EventId}", outboxEvent.Id);
            return;
        }

        // Publish the domain event through mediator
        if (domainEvent is INotification notification)
        {
            await mediator.Publish(notification, cancellationToken);
        }
        else
        {
            _logger.LogWarning("Event {EventType} does not implement INotification", eventType.Name);
        }
    }

    private async Task CleanupProcessedEventsAsync(IOutboxEventRepository outboxRepository, 
        CancellationToken cancellationToken)
    {
        var cutoffTime = DateTime.UtcNow.AddDays(-_options.RetentionDays);
        
        try
        {
            await outboxRepository.DeleteProcessedEventsAsync(cutoffTime, cancellationToken);
            _logger.LogDebug("Cleaned up processed events older than {CutoffTime}", cutoffTime);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clean up processed events");
        }
    }
}