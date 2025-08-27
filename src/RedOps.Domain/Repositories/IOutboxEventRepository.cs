using RedOps.Domain.Entities;

namespace RedOps.Domain.Repositories;

public interface IOutboxEventRepository
{
    Task<OutboxEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxEvent>> GetUnprocessedEventsAsync(int maxCount = 100, CancellationToken cancellationToken = default);
    Task<IEnumerable<OutboxEvent>> GetFailedEventsAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default);
    Task AddAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default);
    Task UpdateAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default);
    Task DeleteProcessedEventsAsync(DateTime olderThan, CancellationToken cancellationToken = default);
}