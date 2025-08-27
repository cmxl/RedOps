CREATE VIEW [dbo].[vw_ActiveProjects]
AS
SELECT 
    p.[Id],
    p.[Name],
    p.[Description],
    p.[RedmineId],
    p.[AzureDevOpsProject],
    p.[SyncDirection],
    CASE p.[SyncDirection]
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'Redmine → Azure DevOps'
        WHEN 2 THEN 'Azure DevOps → Redmine'
        WHEN 3 THEN 'Bidirectional'
        ELSE 'Unknown'
    END AS [SyncDirectionText],
    p.[LastSyncUtc],
    p.[CreatedAt],
    p.[UpdatedAt],
    COALESCE(wi_stats.[TotalWorkItems], 0) AS [TotalWorkItems],
    COALESCE(wi_stats.[CompletedWorkItems], 0) AS [CompletedWorkItems],
    COALESCE(sync_stats.[TotalSyncOperations], 0) AS [TotalSyncOperations],
    sync_stats.[LastSyncOperation],
    sync_stats.[LastSyncStatus],
    CASE 
        WHEN sync_stats.[LastSyncStatus] = 1 THEN 'In Progress'
        WHEN sync_stats.[LastSyncStatus] = 2 THEN 'Completed'
        WHEN sync_stats.[LastSyncStatus] = 3 THEN 'Failed'
        WHEN sync_stats.[LastSyncStatus] = 4 THEN 'Cancelled'
        ELSE 'No Sync History'
    END AS [LastSyncStatusText],
    COALESCE(conflict_stats.[UnresolvedConflicts], 0) AS [UnresolvedConflicts]
FROM [dbo].[Projects] p
LEFT JOIN (
    SELECT 
        [ProjectId],
        COUNT(*) AS [TotalWorkItems],
        SUM(CASE WHEN [CompletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS [CompletedWorkItems]
    FROM [dbo].[WorkItems]
    GROUP BY [ProjectId]
) wi_stats ON p.[Id] = wi_stats.[ProjectId]
LEFT JOIN (
    SELECT 
        so.[ProjectId],
        COUNT(*) AS [TotalSyncOperations],
        MAX(so.[StartTime]) AS [LastSyncOperation],
        MAX(CASE WHEN so.[StartTime] = latest.[LatestSync] THEN so.[Status] END) AS [LastSyncStatus]
    FROM [dbo].[SyncOperations] so
    INNER JOIN (
        SELECT [ProjectId], MAX([StartTime]) AS [LatestSync]
        FROM [dbo].[SyncOperations]
        GROUP BY [ProjectId]
    ) latest ON so.[ProjectId] = latest.[ProjectId]
    GROUP BY so.[ProjectId]
) sync_stats ON p.[Id] = sync_stats.[ProjectId]
LEFT JOIN (
    SELECT 
        so.[ProjectId],
        COUNT(sc.[Id]) AS [UnresolvedConflicts]
    FROM [dbo].[SyncOperations] so
    INNER JOIN [dbo].[SyncConflicts] sc ON so.[Id] = sc.[SyncOperationId]
    WHERE sc.[ResolvedAt] IS NULL
    GROUP BY so.[ProjectId]
) conflict_stats ON p.[Id] = conflict_stats.[ProjectId]
WHERE p.[IsActive] = 1;