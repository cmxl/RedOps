using RedOps.Domain.Entities;
using RedOps.Domain.ValueObjects;

namespace RedOps.Domain.Repositories;

public interface IProjectRepository
{
    Task<Project?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Project?> GetByRedmineIdAsync(RedmineId redmineId, CancellationToken cancellationToken = default);
    Task<Project?> GetByAzureDevOpsProjectAsync(string azureDevOpsProject, CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetActiveProjectsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}