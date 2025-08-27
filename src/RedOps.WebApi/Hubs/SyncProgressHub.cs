using Microsoft.AspNetCore.SignalR;

namespace RedOps.WebApi.Hubs;

public class SyncProgressHub : Hub
{
    private readonly ILogger<SyncProgressHub> _logger;

    public SyncProgressHub(ILogger<SyncProgressHub> logger)
    {
        _logger = logger;
    }

    public override async Task OnConnectedAsync()
    {
        _logger.LogDebug("Client connected to SyncProgressHub: {ConnectionId}", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("Client disconnected from SyncProgressHub: {ConnectionId}", Context.ConnectionId);
        
        if (exception != null)
        {
            _logger.LogWarning(exception, "Client disconnected with exception: {ConnectionId}", Context.ConnectionId);
        }
        
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinProjectGroup(string projectId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"project_{projectId}");
        _logger.LogDebug("Client {ConnectionId} joined project group {ProjectId}", Context.ConnectionId, projectId);
    }

    public async Task LeaveProjectGroup(string projectId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"project_{projectId}");
        _logger.LogDebug("Client {ConnectionId} left project group {ProjectId}", Context.ConnectionId, projectId);
    }

    public async Task JoinOperationGroup(string operationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"operation_{operationId}");
        _logger.LogDebug("Client {ConnectionId} joined operation group {OperationId}", Context.ConnectionId, operationId);
    }

    public async Task LeaveOperationGroup(string operationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"operation_{operationId}");
        _logger.LogDebug("Client {ConnectionId} left operation group {OperationId}", Context.ConnectionId, operationId);
    }
}