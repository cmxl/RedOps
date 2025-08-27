using MediatR;
using RedOps.Application.Common.DTOs;
using RedOps.Domain.Enums;

namespace RedOps.Application.Projects.Commands.CreateProject;

public record CreateProjectCommand(
    string Name,
    string? Description,
    int? RedmineId,
    string? AzureDevOpsProject,
    SyncDirection SyncDirection
) : IRequest<ProjectDto>;