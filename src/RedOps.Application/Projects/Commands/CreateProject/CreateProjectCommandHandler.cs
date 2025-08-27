using Mediator;
using Microsoft.Extensions.Logging;
using RedOps.Application.Common.DTOs;
using RedOps.Application.Common.Interfaces;
using RedOps.Domain.Entities;
using RedOps.Domain.Repositories;

namespace RedOps.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : ICommandHandler<CreateProjectCommand, ProjectDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ILogger<CreateProjectCommandHandler> _logger;

    public CreateProjectCommandHandler(
        IProjectRepository projectRepository,
        ILogger<CreateProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _logger = logger;
    }

    public async ValueTask<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Creating new project: {ProjectName}", request.Name);

        if (request.RedmineId.HasValue)
        {
            var existingRedmineProject = await _projectRepository
                .GetByRedmineIdAsync(request.RedmineId.Value, cancellationToken);
            
            if (existingRedmineProject != null)
            {
                throw new InvalidOperationException($"A project with Redmine ID {request.RedmineId} already exists.");
            }
        }

        if (!string.IsNullOrEmpty(request.AzureDevOpsProject))
        {
            var existingAzureProject = await _projectRepository
                .GetByAzureDevOpsProjectAsync(request.AzureDevOpsProject, cancellationToken);
            
            if (existingAzureProject != null)
            {
                throw new InvalidOperationException($"A project mapped to Azure DevOps project '{request.AzureDevOpsProject}' already exists.");
            }
        }

        var project = Project.Create(request.Name, request.Description, request.SyncDirection);

        if (request.RedmineId.HasValue)
        {
            project.UpdateRedmineMapping(request.RedmineId.Value);
        }

        if (!string.IsNullOrEmpty(request.AzureDevOpsProject))
        {
            project.UpdateAzureDevOpsMapping(request.AzureDevOpsProject);
        }

        await _projectRepository.AddAsync(project, cancellationToken);

        _logger.LogInformation("Project created successfully: {ProjectId}", project.Id);

        return new ProjectDto
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
            WorkItemCount = 0,
            UnresolvedConflictsCount = 0
        };
    }
}