using Mediator;
using RedOps.Application.Common.DTOs;
using RedOps.Application.Common.Interfaces;
using RedOps.Domain.Enums;
using RedOps.Domain.Repositories;

namespace RedOps.Application.Projects.Queries.GetSyncStatus;

public class GetSyncStatusQueryHandler : IQueryHandler<GetSyncStatusQuery, SyncStatusDto>
{
    private readonly IProjectRepository _projectRepository;
    private readonly ISyncOperationRepository _syncOperationRepository;
    private readonly ISyncOrchestrator _syncOrchestrator;
    private readonly IWorkItemRepository _workItemRepository;
    private readonly ISyncConflictRepository _syncConflictRepository;

    public GetSyncStatusQueryHandler(
        IProjectRepository projectRepository,
        ISyncOperationRepository syncOperationRepository,
        ISyncOrchestrator syncOrchestrator,
        IWorkItemRepository workItemRepository,
        ISyncConflictRepository syncConflictRepository)
    {
        _projectRepository = projectRepository;
        _syncOperationRepository = syncOperationRepository;
        _syncOrchestrator = syncOrchestrator;
        _workItemRepository = workItemRepository;
        _syncConflictRepository = syncConflictRepository;
    }

    public async ValueTask<SyncStatusDto> Handle(GetSyncStatusQuery request, CancellationToken cancellationToken)
    {
        var project = await _projectRepository.GetByIdAsync(request.ProjectId, cancellationToken);
        if (project == null)
        {
            throw new InvalidOperationException($"Project with ID {request.ProjectId} not found.");
        }

        var isSyncInProgress = await _syncOrchestrator.IsSyncInProgressAsync(request.ProjectId, cancellationToken);
        var recentOperations = await _syncOperationRepository.GetByProjectIdAsync(request.ProjectId, cancellationToken);
        var pendingItems = await _workItemRepository.GetPendingSyncAsync(cancellationToken);
        var conflictsCount = await _syncConflictRepository.CountUnresolvedByProjectIdAsync(request.ProjectId, cancellationToken);

        var currentOperation = recentOperations
            .Where(op => op.Status == SyncStatus.InProgress)
            .OrderByDescending(op => op.StartTime)
            .FirstOrDefault();

        return new SyncStatusDto
        {
            ProjectId = project.Id,
            ProjectName = project.Name,
            IsSyncInProgress = isSyncInProgress,
            LastSyncUtc = project.LastSyncUtc,
            CurrentOperation = currentOperation != null ? MapToDto(currentOperation, project.Name) : null,
            RecentOperations = recentOperations
                .OrderByDescending(op => op.StartTime)
                .Take(10)
                .Select(op => MapToDto(op, project.Name))
                .ToList(),
            PendingItemsCount = pendingItems.Count(wi => wi.ProjectId == request.ProjectId),
            ConflictsCount = conflictsCount
        };
    }

    private static SyncOperationDto MapToDto(Domain.Entities.SyncOperation operation, string projectName)
    {
        return new SyncOperationDto
        {
            Id = operation.Id,
            ProjectId = operation.ProjectId,
            ProjectName = projectName,
            OperationType = operation.OperationType,
            SyncDirection = operation.SyncDirection,
            StartTime = operation.StartTime,
            EndTime = operation.EndTime,
            Status = operation.Status,
            ItemsProcessed = operation.ItemsProcessed,
            ErrorCount = operation.ErrorCount,
            Details = operation.Details,
            ErrorMessage = operation.ErrorMessage
        };
    }
}