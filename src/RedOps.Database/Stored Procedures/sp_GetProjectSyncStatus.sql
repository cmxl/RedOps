CREATE PROCEDURE [dbo].[sp_GetProjectSyncStatus]
    @ProjectId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    -- Project basic information
    SELECT 
        p.[Id],
        p.[Name],
        p.[Description],
        p.[RedmineId],
        p.[AzureDevOpsProject],
        p.[SyncDirection],
        p.[IsActive],
        p.[LastSyncUtc],
        p.[CreatedAt],
        p.[UpdatedAt]
    FROM [dbo].[Projects] p
    WHERE p.[Id] = @ProjectId;
    
    -- Work items summary
    SELECT 
        COUNT(*) AS [TotalWorkItems],
        SUM(CASE WHEN [CompletedAt] IS NOT NULL THEN 1 ELSE 0 END) AS [CompletedWorkItems],
        SUM(CASE WHEN [RedmineId] IS NOT NULL THEN 1 ELSE 0 END) AS [WorkItemsInRedmine],
        SUM(CASE WHEN [AzureDevOpsId] IS NOT NULL THEN 1 ELSE 0 END) AS [WorkItemsInAzureDevOps],
        MAX([LastSyncUtc]) AS [LastWorkItemSync],
        MIN([CreatedAt]) AS [OldestWorkItem],
        MAX([CreatedAt]) AS [NewestWorkItem]
    FROM [dbo].[WorkItems]
    WHERE [ProjectId] = @ProjectId;
    
    -- Recent sync operations
    SELECT TOP 10
        so.[Id],
        so.[OperationType],
        so.[SyncDirection],
        so.[Status],
        so.[StartTime],
        so.[EndTime],
        so.[ItemsProcessed],
        so.[ItemsCreated],
        so.[ItemsUpdated],
        so.[ItemsSkipped],
        so.[ItemsFailed],
        so.[ConflictsDetected],
        so.[ErrorMessage],
        so.[TriggerSource],
        so.[InitiatedBy],
        COALESCE(conflict_count.[ConflictCount], 0) AS [TotalConflicts],
        COALESCE(conflict_count.[UnresolvedConflicts], 0) AS [UnresolvedConflicts]
    FROM [dbo].[SyncOperations] so
    LEFT JOIN (
        SELECT 
            [SyncOperationId],
            COUNT(*) AS [ConflictCount],
            SUM(CASE WHEN [ResolvedAt] IS NULL THEN 1 ELSE 0 END) AS [UnresolvedConflicts]
        FROM [dbo].[SyncConflicts]
        GROUP BY [SyncOperationId]
    ) conflict_count ON so.[Id] = conflict_count.[SyncOperationId]
    WHERE so.[ProjectId] = @ProjectId
    ORDER BY so.[StartTime] DESC;
    
    -- Active conflicts
    SELECT 
        sc.[Id],
        sc.[SyncOperationId],
        sc.[WorkItemId],
        wi.[Title] AS [WorkItemTitle],
        sc.[EntityType],
        sc.[ConflictType],
        sc.[FieldName],
        sc.[AutoResolvable],
        sc.[CreatedAt],
        DATEDIFF(HOUR, sc.[CreatedAt], GETUTCDATE()) AS [ConflictAgeHours]
    FROM [dbo].[SyncConflicts] sc
    INNER JOIN [dbo].[SyncOperations] so ON sc.[SyncOperationId] = so.[Id]
    LEFT JOIN [dbo].[WorkItems] wi ON sc.[WorkItemId] = wi.[Id]
    WHERE so.[ProjectId] = @ProjectId
      AND sc.[ResolvedAt] IS NULL
    ORDER BY sc.[CreatedAt] DESC;
    
    -- Field mappings
    SELECT 
        fm.[Id],
        fm.[EntityType],
        fm.[RedmineField],
        fm.[AzureDevOpsField],
        fm.[MappingType],
        fm.[IsRequired],
        fm.[IsActive],
        fm.[UpdatedAt]
    FROM [dbo].[FieldMappings] fm
    WHERE fm.[ProjectId] = @ProjectId
      AND fm.[IsActive] = 1
    ORDER BY fm.[EntityType], fm.[RedmineField];
END;