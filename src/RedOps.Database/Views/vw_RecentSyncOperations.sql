CREATE VIEW [dbo].[vw_RecentSyncOperations]
AS
SELECT TOP 1000
    so.[Id],
    so.[ProjectId],
    p.[Name] AS [ProjectName],
    so.[OperationType],
    CASE so.[OperationType]
        WHEN 0 THEN 'Full Sync'
        WHEN 1 THEN 'Incremental Sync'
        WHEN 2 THEN 'Conflict Resolution'
        ELSE 'Unknown'
    END AS [OperationTypeText],
    so.[SyncDirection],
    CASE so.[SyncDirection]
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'Redmine → Azure DevOps'
        WHEN 2 THEN 'Azure DevOps → Redmine'
        WHEN 3 THEN 'Bidirectional'
        ELSE 'Unknown'
    END AS [SyncDirectionText],
    so.[Status],
    CASE so.[Status]
        WHEN 0 THEN 'Pending'
        WHEN 1 THEN 'In Progress'
        WHEN 2 THEN 'Completed'
        WHEN 3 THEN 'Failed'
        WHEN 4 THEN 'Cancelled'
        ELSE 'Unknown'
    END AS [StatusText],
    so.[StartTime],
    so.[EndTime],
    CASE 
        WHEN so.[EndTime] IS NOT NULL THEN DATEDIFF(SECOND, so.[StartTime], so.[EndTime])
        WHEN so.[Status] = 1 THEN DATEDIFF(SECOND, so.[StartTime], GETUTCDATE())
        ELSE NULL
    END AS [DurationSeconds],
    so.[ItemsProcessed],
    so.[ItemsCreated],
    so.[ItemsUpdated],
    so.[ItemsSkipped],
    so.[ItemsFailed],
    so.[ConflictsDetected],
    so.[ErrorMessage],
    so.[TriggerSource],
    so.[InitiatedBy],
    so.[LastHeartbeat],
    CASE 
        WHEN so.[Status] = 1 AND so.[LastHeartbeat] < DATEADD(MINUTE, -10, GETUTCDATE()) THEN 1
        ELSE 0
    END AS [IsStuck],
    COALESCE(conflict_count.[ConflictCount], 0) AS [TotalConflicts],
    COALESCE(conflict_count.[UnresolvedConflicts], 0) AS [UnresolvedConflicts]
FROM [dbo].[SyncOperations] so
INNER JOIN [dbo].[Projects] p ON so.[ProjectId] = p.[Id]
LEFT JOIN (
    SELECT 
        [SyncOperationId],
        COUNT(*) AS [ConflictCount],
        SUM(CASE WHEN [ResolvedAt] IS NULL THEN 1 ELSE 0 END) AS [UnresolvedConflicts]
    FROM [dbo].[SyncConflicts]
    GROUP BY [SyncOperationId]
) conflict_count ON so.[Id] = conflict_count.[SyncOperationId]
ORDER BY so.[StartTime] DESC;