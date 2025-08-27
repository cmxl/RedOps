using Microsoft.EntityFrameworkCore;
using RedOps.Domain.Entities;
using RedOps.Domain.Enums;
using RedOps.Domain.Repositories;
using RedOps.Infrastructure.Data;

namespace RedOps.Infrastructure.Repositories;

public class SyncOperationRepository : ISyncOperationRepository
{
    private readonly ApplicationDbContext _context;

    public SyncOperationRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SyncOperation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SyncOperations
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SyncOperation>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncOperations
            .Where(s => s.ProjectId == projectId)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SyncOperation>> GetRecentOperationsAsync(int count = 50, CancellationToken cancellationToken = default)
    {
        return await _context.SyncOperations
            .OrderByDescending(s => s.StartTime)
            .Take(count)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SyncOperation>> GetByStatusAsync(SyncStatus status, CancellationToken cancellationToken = default)
    {
        return await _context.SyncOperations
            .Where(s => s.Status == status)
            .OrderByDescending(s => s.StartTime)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SyncOperation syncOperation, CancellationToken cancellationToken = default)
    {
        await _context.SyncOperations.AddAsync(syncOperation, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SyncOperation syncOperation, CancellationToken cancellationToken = default)
    {
        _context.SyncOperations.Update(syncOperation);
        await _context.SaveChangesAsync(cancellationToken);
    }
}