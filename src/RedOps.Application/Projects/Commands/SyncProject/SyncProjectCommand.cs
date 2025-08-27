using Mediator;
using RedOps.Domain.Enums;

namespace RedOps.Application.Projects.Commands.SyncProject;

public record SyncProjectCommand(
    Guid ProjectId,
    SyncDirection SyncDirection
) : ICommand<Guid>;