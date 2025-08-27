CREATE VIEW [dbo].[vw_PendingConflicts]
AS
SELECT 
    sc.[Id],
    sc.[SyncOperationId],
    so.[ProjectId],
    p.[Name] AS [ProjectName],
    sc.[WorkItemId],
    wi.[Title] AS [WorkItemTitle],
    sc.[EntityType],
    CASE sc.[EntityType]
        WHEN 0 THEN 'Project'
        WHEN 1 THEN 'Work Item'
        WHEN 2 THEN 'Comment'
        WHEN 3 THEN 'Attachment'
        ELSE 'Unknown'
    END AS [EntityTypeText],
    sc.[ConflictType],
    CASE sc.[ConflictType]
        WHEN 0 THEN 'Data Mismatch'
        WHEN 1 THEN 'Duplicate Creation'
        WHEN 2 THEN 'Deleted in Source'
        WHEN 3 THEN 'Modified in Both'
        ELSE 'Unknown'
    END AS [ConflictTypeText],
    sc.[FieldName],
    sc.[RedmineValue],
    sc.[AzureDevOpsValue],
    sc.[LocalValue],
    sc.[AutoResolvable],
    sc.[CreatedAt] AS [ConflictDetectedAt],
    so.[StartTime] AS [SyncOperationStarted],
    so.[Status] AS [SyncOperationStatus],
    CASE so.[Status]
        WHEN 0 THEN 'Pending'
        WHEN 1 THEN 'In Progress'
        WHEN 2 THEN 'Completed'
        WHEN 3 THEN 'Failed'
        WHEN 4 THEN 'Cancelled'
        ELSE 'Unknown'
    END AS [SyncOperationStatusText],
    DATEDIFF(HOUR, sc.[CreatedAt], GETUTCDATE()) AS [ConflictAgeHours],
    CASE 
        WHEN sc.[AutoResolvable] = 1 THEN 'Auto-Resolvable'
        WHEN DATEDIFF(DAY, sc.[CreatedAt], GETUTCDATE()) > 7 THEN 'Old'
        WHEN DATEDIFF(DAY, sc.[CreatedAt], GETUTCDATE()) > 1 THEN 'Recent'
        ELSE 'New'
    END AS [ConflictPriority]
FROM [dbo].[SyncConflicts] sc
INNER JOIN [dbo].[SyncOperations] so ON sc.[SyncOperationId] = so.[Id]
INNER JOIN [dbo].[Projects] p ON so.[ProjectId] = p.[Id]
LEFT JOIN [dbo].[WorkItems] wi ON sc.[WorkItemId] = wi.[Id]
WHERE sc.[ResolvedAt] IS NULL
  AND p.[IsActive] = 1;