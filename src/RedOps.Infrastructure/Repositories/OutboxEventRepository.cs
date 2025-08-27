using Microsoft.EntityFrameworkCore;
using RedOps.Domain.Entities;
using RedOps.Domain.Repositories;
using RedOps.Infrastructure.Data;

namespace RedOps.Infrastructure.Repositories;

public class OutboxEventRepository : IOutboxEventRepository
{
    private readonly ApplicationDbContext _context;

    public OutboxEventRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OutboxEvent?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxEvents
            .FirstOrDefaultAsync(oe => oe.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<OutboxEvent>> GetUnprocessedEventsAsync(int maxCount = 100, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxEvents
            .Where(oe => !oe.IsProcessed)
            .OrderBy(oe => oe.CreatedUtc)
            .Take(maxCount)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<OutboxEvent>> GetFailedEventsAsync(int maxRetryCount = 3, CancellationToken cancellationToken = default)
    {
        return await _context.OutboxEvents
            .Where(oe => !oe.IsProcessed && oe.RetryCount >= maxRetryCount)
            .OrderBy(oe => oe.CreatedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default)
    {
        await _context.OutboxEvents.AddAsync(outboxEvent, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(OutboxEvent outboxEvent, CancellationToken cancellationToken = default)
    {
        _context.OutboxEvents.Update(outboxEvent);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteProcessedEventsAsync(DateTime olderThan, CancellationToken cancellationToken = default)
    {
        var eventsToDelete = await _context.OutboxEvents
            .Where(oe => oe.IsProcessed && oe.ProcessedUtc < olderThan)
            .ToListAsync(cancellationToken);

        _context.OutboxEvents.RemoveRange(eventsToDelete);
        await _context.SaveChangesAsync(cancellationToken);
    }
}