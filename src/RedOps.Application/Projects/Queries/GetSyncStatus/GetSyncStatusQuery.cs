using Mediator;
using RedOps.Application.Common.DTOs;

namespace RedOps.Application.Projects.Queries.GetSyncStatus;

public record GetSyncStatusQuery(Guid ProjectId) : IQuery<SyncStatusDto>;