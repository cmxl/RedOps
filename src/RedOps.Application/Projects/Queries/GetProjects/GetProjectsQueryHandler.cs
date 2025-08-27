using Mediator;
using RedOps.Application.Common.DTOs;
using RedOps.Domain.Repositories;

namespace RedOps.Application.Projects.Queries.GetProjects;

public class GetProjectsQueryHandler : IQueryHandler<GetProjectsQuery, IEnumerable<ProjectDto>>
{
    private readonly IProjectRepository _projectRepository;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly ISyncConflictRepository _syncConflictRepository;

    public GetProjectsQueryHandler(
        IProjectRepository projectRepository,
        IWorkItemRepository workItemRepository,
        ISyncConflictRepository syncConflictRepository)
    {
        _projectRepository = projectRepository;
        _workItemRepository = workItemRepository;
        _syncConflictRepository = syncConflictRepository;
    }

    public async ValueTask<IEnumerable<ProjectDto>> Handle(GetProjectsQuery request, CancellationToken cancellationToken)
    {
        var projects = request.IncludeInactive 
            ? await _projectRepository.GetAllAsync(cancellationToken)
            : await _projectRepository.GetActiveProjectsAsync(cancellationToken);

        var projectDtos = new List<ProjectDto>();

        foreach (var project in projects)
        {
            var workItemCount = await _workItemRepository.CountByProjectIdAsync(project.Id, cancellationToken);
            var conflictsCount = await _syncConflictRepository.CountUnresolvedByProjectIdAsync(project.Id, cancellationToken);

            projectDtos.Add(new ProjectDto
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                RedmineId = project.RedmineId,
                AzureDevOpsProject = project.AzureDevOpsProject,
                SyncDirection = project.SyncDirection,
                LastSyncUtc = project.LastSyncUtc,
                IsActive = project.IsActive,
                CreatedUtc = project.CreatedUtc,
                ModifiedUtc = project.ModifiedUtc,
                WorkItemCount = workItemCount,
                UnresolvedConflictsCount = conflictsCount
            });
        }

        return projectDtos.OrderBy(p => p.Name);
    }
}