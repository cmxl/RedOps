using Microsoft.EntityFrameworkCore;
using RedOps.Domain.Entities;
using RedOps.Domain.Repositories;
using RedOps.Domain.ValueObjects;
using RedOps.Infrastructure.Data;

namespace RedOps.Infrastructure.Repositories;

public class WorkItemRepository : IWorkItemRepository
{
    private readonly ApplicationDbContext _context;

    public WorkItemRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WorkItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems
            .Include(w => w.Comments)
            .Include(w => w.Attachments)
            .FirstOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<WorkItem?> GetByRedmineIdAsync(RedmineId redmineId, CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems
            .Include(w => w.Comments)
            .Include(w => w.Attachments)
            .FirstOrDefaultAsync(w => w.RedmineId == redmineId, cancellationToken);
    }

    public async Task<WorkItem?> GetByAzureDevOpsIdAsync(AzureDevOpsId azureDevOpsId, CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems
            .Include(w => w.Comments)
            .Include(w => w.Attachments)
            .FirstOrDefaultAsync(w => w.AzureDevOpsId == azureDevOpsId, cancellationToken);
    }

    public async Task<IEnumerable<WorkItem>> GetByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems
            .Where(w => w.ProjectId == projectId)
            .OrderByDescending(w => w.ModifiedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WorkItem>> GetModifiedSinceAsync(DateTime since, CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems
            .Where(w => w.ModifiedUtc > since)
            .OrderByDescending(w => w.ModifiedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<WorkItem>> GetPendingSyncAsync(CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems
            .Where(w => w.LastSyncUtc == null || w.ModifiedUtc > w.LastSyncUtc)
            .OrderByDescending(w => w.ModifiedUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        await _context.WorkItems.AddAsync(workItem, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(WorkItem workItem, CancellationToken cancellationToken = default)
    {
        _context.WorkItems.Update(workItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var workItem = await _context.WorkItems.FindAsync([id], cancellationToken);
        if (workItem != null)
        {
            _context.WorkItems.Remove(workItem);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<int> CountByProjectIdAsync(Guid projectId, CancellationToken cancellationToken = default)
    {
        return await _context.WorkItems
            .CountAsync(w => w.ProjectId == projectId, cancellationToken);
    }
}