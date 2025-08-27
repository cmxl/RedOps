using Microsoft.AspNetCore.Mvc;
using Mediator;
using RedOps.Application.Common.DTOs;

namespace RedOps.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkItemsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<WorkItemsController> _logger;

    public WorkItemsController(IMediator mediator, ILogger<WorkItemsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WorkItemDto>>> GetWorkItems(
        [FromQuery] Guid? projectId = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting work items for project {ProjectId}, page {Page}, pageSize {PageSize}", 
            projectId, page, pageSize);

        // This would need a GetWorkItemsQuery
        return Ok(new List<WorkItemDto>());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WorkItemDto>> GetWorkItem(Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting work item {WorkItemId}", id);

        // This would need a GetWorkItemByIdQuery
        return NotFound();
    }

    [HttpGet("{id}/attachments")]
    public async Task<ActionResult<IEnumerable<object>>> GetWorkItemAttachments(Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting attachments for work item {WorkItemId}", id);

        // This would need a GetWorkItemAttachmentsQuery
        return Ok(new List<object>());
    }

    [HttpGet("{id}/comments")]
    public async Task<ActionResult<IEnumerable<object>>> GetWorkItemComments(Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting comments for work item {WorkItemId}", id);

        // This would need a GetWorkItemCommentsQuery
        return Ok(new List<object>());
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddWorkItemComment(Guid id,
        [FromBody] AddCommentRequest request,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adding comment to work item {WorkItemId}", id);

        // This would need an AddCommentCommand
        return CreatedAtAction(nameof(GetWorkItemComments), new { id }, null);
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<WorkItemDto>>> SearchWorkItems(
        [FromQuery] string? query = null,
        [FromQuery] Guid? projectId = null,
        [FromQuery] string? status = null,
        [FromQuery] string? assignee = null,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Searching work items with query '{Query}' for project {ProjectId}", 
            query, projectId);

        // This would need a SearchWorkItemsQuery
        return Ok(new List<WorkItemDto>());
    }
}

public record AddCommentRequest(string Content);