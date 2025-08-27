using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Mediator;
using RedOps.Application.Projects.Commands.SyncProject;
using RedOps.Application.Projects.Queries.GetSyncStatus;
using RedOps.Application.Common.DTOs;
using RedOps.Domain.Enums;
using RedOps.WebApi.Hubs;

namespace RedOps.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SyncController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IHubContext<SyncProgressHub> _hubContext;
    private readonly ILogger<SyncController> _logger;

    public SyncController(
        IMediator mediator, 
        IHubContext<SyncProgressHub> hubContext,
        ILogger<SyncController> logger)
    {
        _mediator = mediator;
        _hubContext = hubContext;
        _logger = logger;
    }

    [HttpPost("projects/{projectId}/sync")]
    public async Task<ActionResult<Guid>> SyncProject(
        Guid projectId,
        [FromBody] SyncProjectRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Starting sync for project {ProjectId} with direction {SyncDirection}", 
            projectId, request.SyncDirection);

        var command = new SyncProjectCommand(projectId, request.SyncDirection);
        var operationId = await _mediator.Send(command, cancellationToken);

        // Notify clients via SignalR
        await _hubContext.Clients.All.SendAsync("SyncStarted", new
        {
            ProjectId = projectId,
            OperationId = operationId,
            SyncDirection = request.SyncDirection
        }, cancellationToken);

        return Accepted(new { OperationId = operationId });
    }

    [HttpGet("projects/{projectId}/status")]
    public async Task<ActionResult<SyncOperationDto>> GetSyncStatus(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting sync status for project {ProjectId}", projectId);

        var query = new GetSyncStatusQuery(projectId);
        var status = await _mediator.Send(query, cancellationToken);

        return Ok(status);
    }

    [HttpGet("operations/{operationId}")]
    public async Task<ActionResult<SyncOperationDto>> GetOperationStatus(
        Guid operationId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting operation status for {OperationId}", operationId);

        // This would need a GetOperationByIdQuery
        return NotFound();
    }

    [HttpPost("projects/{projectId}/cancel")]
    public async Task<IActionResult> CancelSync(
        Guid projectId,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Cancelling sync for project {ProjectId}", projectId);

        // This would need a CancelSyncCommand
        // Notify clients via SignalR
        await _hubContext.Clients.All.SendAsync("SyncCancelled", new
        {
            ProjectId = projectId
        }, cancellationToken);

        return Ok();
    }

    [HttpGet("conflicts")]
    public async Task<ActionResult<IEnumerable<ConflictDto>>> GetConflicts(
        [FromQuery] Guid? projectId = null,
        [FromQuery] bool unresolved = true,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting conflicts for project {ProjectId}, unresolved: {Unresolved}", 
            projectId, unresolved);

        // This would need a GetConflictsQuery
        return Ok(new List<ConflictDto>());
    }

    [HttpPost("conflicts/{conflictId}/resolve")]
    public async Task<IActionResult> ResolveConflict(
        Guid conflictId,
        [FromBody] ResolveConflictRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Resolving conflict {ConflictId} with strategy {Strategy}", 
            conflictId, request.Strategy);

        // This would need a ResolveConflictCommand
        return Ok();
    }
}

public record SyncProjectRequest(SyncDirection SyncDirection);
public record ResolveConflictRequest(string Strategy);