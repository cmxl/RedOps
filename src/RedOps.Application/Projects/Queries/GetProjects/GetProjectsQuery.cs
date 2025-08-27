using Mediator;
using RedOps.Application.Common.DTOs;

namespace RedOps.Application.Projects.Queries.GetProjects;

public record GetProjectsQuery(bool IncludeInactive = false) : IQuery<IEnumerable<ProjectDto>>;