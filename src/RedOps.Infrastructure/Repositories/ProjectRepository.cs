using Microsoft.EntityFrameworkCore;
using RedOps.Domain.Entities;
using RedOps.Domain.Repositories;
using RedOps.Domain.ValueObjects;
using RedOps.Infrastructure.Data;

namespace RedOps.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.WorkItems)
            .Include(p => p.FieldMappings)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project?> GetByRedmineIdAsync(RedmineId redmineId, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.WorkItems)
            .Include(p => p.FieldMappings)
            .FirstOrDefaultAsync(p => p.RedmineId == redmineId, cancellationToken);
    }

    public async Task<Project?> GetByAzureDevOpsProjectAsync(string azureDevOpsProject, CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Include(p => p.WorkItems)
            .Include(p => p.FieldMappings)
            .FirstOrDefaultAsync(p => p.AzureDevOpsProject == azureDevOpsProject, cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .Where(p => p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        await _context.Projects.AddAsync(project, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var project = await _context.Projects.FindAsync([id], cancellationToken);
        if (project != null)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}