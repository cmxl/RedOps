using RedOps.Domain.Common;

namespace RedOps.Domain.Entities;

public class OutboxEvent : Entity
{
    public string EventType { get; private set; } = string.Empty;
    public string AggregateId { get; private set; } = string.Empty;
    public string EventData { get; private set; } = string.Empty;
    public DateTime CreatedUtc { get; private set; }
    public DateTime? ProcessedUtc { get; private set; }
    public int RetryCount { get; private set; }
    public string? LastError { get; private set; }
    public bool IsProcessed { get; private set; }

    private OutboxEvent(Guid id) : base(id) { }

    public static OutboxEvent Create(string eventType, string aggregateId, string eventData)
    {
        return new OutboxEvent(Guid.NewGuid())
        {
            EventType = eventType,
            AggregateId = aggregateId,
            EventData = eventData,
            CreatedUtc = DateTime.UtcNow,
            RetryCount = 0,
            IsProcessed = false
        };
    }

    public void MarkAsProcessed()
    {
        IsProcessed = true;
        ProcessedUtc = DateTime.UtcNow;
    }

    public void IncrementRetryCount(string error)
    {
        RetryCount++;
        LastError = error;
    }
}