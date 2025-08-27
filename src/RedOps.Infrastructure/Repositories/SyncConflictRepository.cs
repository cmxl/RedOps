using Microsoft.EntityFrameworkCore;
using RedOps.Domain.Entities;
using RedOps.Domain.Repositories;
using RedOps.Infrastructure.Data;

namespace RedOps.Infrastructure.Repositories;

public class SyncConflictRepository : ISyncConflictRepository
{
    private readonly ApplicationDbContext _context;

    public SyncConflictRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SyncConflict?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.SyncConflicts
            .FirstOrDefaultAsync(sc => sc.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<SyncConflict>> GetByWorkItemIdAsync(Guid workItemId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncConflicts
            .Where(sc => sc.WorkItemId == workItemId)
            .OrderByDescending(sc => sc.CreatedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SyncConflict>> GetUnresolvedConflictsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SyncConflicts
            .Where(sc => !sc.IsResolved)
            .OrderByDescending(sc => sc.CreatedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<SyncConflict>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncConflicts
            .Join(_context.WorkItems,
                sc => sc.WorkItemId,
                wi => wi.Id,
                (sc, wi) => new { Conflict = sc, WorkItem = wi })
            .Where(joined => joined.WorkItem.ProjectId == projectId)
            .Select(joined => joined.Conflict)
            .OrderByDescending(sc => sc.CreatedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SyncConflict conflict, CancellationToken cancellationToken = default)
    {
        await _context.SyncConflicts.AddAsync(conflict, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SyncConflict conflict, CancellationToken cancellationToken = default)
    {
        _context.SyncConflicts.Update(conflict);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<int> CountUnresolvedByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.SyncConflicts
            .Join(_context.WorkItems,
                sc => sc.WorkItemId,
                wi => wi.Id,
                (sc, wi) => new { Conflict = sc, WorkItem = wi })
            .Where(joined => joined.WorkItem.ProjectId == projectId && !joined.Conflict.IsResolved)
            .CountAsync(cancellationToken);
    }
}