using Mediator;
using Microsoft.Extensions.Logging;
using RedOps.Application.Common.Interfaces;
using RedOps.Domain.Repositories;

namespace RedOps.Application.Projects.Commands.SyncProject;

public class SyncProjectCommandHandler : ICommandHandler<SyncProjectCommand, Guid>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ISyncOrchestrator _syncOrchestrator;
    private readonly ILogger<SyncProjectCommandHandler> _logger;

    public SyncProjectCommandHandler(
        IProjectRepository projectRepository,
        ISyncOrchestrator syncOrchestrator,
        ILogger<SyncProjectCommandHandler> logger)
    {
        _projectRepository = projectRepository;
        _syncOrchestrator = syncOrchestrator;
        _logger = logger;
    }

    public async ValueTask<Guid> Handle(SyncProjectCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting sync for project: {ProjectId} in direction: {SyncDirection}", 
            request.ProjectId, request.SyncDirection);

        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {request.ProjectId} not found.");
        }

        if (!project.IsActive)
        {
            throw new InvalidOperationException($"Cannot sync inactive project {project.Name}.");
        }

        var isSyncInProgress = await _syncOrchestrator.IsSyncInProgressAsync(request.ProjectId, cancellationToken);
        if (isSyncInProgress)
        {
            throw new InvalidOperationException($"Sync is already in progress for project {project.Name}.");
        }

        var operationId = await _syncOrchestrator.StartSyncAsync(
            request.ProjectId, 
            request.SyncDirection, 
            cancellationToken);

        _logger.LogInformation("Sync operation started: {OperationId} for project: {ProjectId}", 
            operationId, request.ProjectId);

        return operationId;
    }
}