using Microsoft.AspNetCore.Mvc;
using Mediator;
using RedOps.Application.Projects.Commands.CreateProject;
using RedOps.Application.Projects.Queries.GetProjects;
using RedOps.Application.Common.DTOs;

namespace RedOps.WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProjectsController> _logger;

    public ProjectsController(IMediator mediator, ILogger<ProjectsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectDto>>> GetProjects(
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting all projects");
        
        var query = new GetProjectsQuery();
        var projects = await _mediator.Send(query, cancellationToken);
        
        return Ok(projects);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProjectDto>> GetProject(Guid id, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting project {ProjectId}", id);
        
        // This would need a GetProjectByIdQuery
        return NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProject(
        CreateProjectCommand command,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating new project: {ProjectName}", command.Name);
        
        var projectId = await _mediator.Send(command, cancellationToken);
        
        return CreatedAtAction(
            nameof(GetProject), 
            new { id = projectId }, 
            projectId);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(Guid id, 
        [FromBody] object updateCommand,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating project {ProjectId}", id);
        
        // This would need an UpdateProjectCommand
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(Guid id,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Deleting project {ProjectId}", id);
        
        // This would need a DeleteProjectCommand
        return NoContent();
    }
}