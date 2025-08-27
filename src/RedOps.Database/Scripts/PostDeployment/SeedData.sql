-- Seed data for RedOps database
PRINT 'Seeding initial data for RedOps...';

-- Sample project for demonstration (only if no projects exist)
IF NOT EXISTS (SELECT 1 FROM [dbo].[Projects])
BEGIN
    PRINT 'Creating sample project...';
    
    DECLARE @SampleProjectId UNIQUEIDENTIFIER = NEWID();
    
    INSERT INTO [dbo].[Projects] (
        [Id], [Name], [Description], [RedmineId], [AzureDevOpsProject], 
        [SyncDirection], [IsActive], [CreatedBy]
    )
    VALUES (
        @SampleProjectId, 
        'Sample Integration Project', 
        'A sample project to demonstrate Redmine and Azure DevOps integration capabilities.',
        NULL, -- Will be configured later
        NULL, -- Will be configured later
        0, -- None - to be configured
        0, -- Inactive until properly configured
        'System'
    );
    
    -- Sample field mappings for work items
    INSERT INTO [dbo].[FieldMappings] (
        [ProjectId], [EntityType], [RedmineField], [AzureDevOpsField], 
        [MappingType], [IsRequired], [CreatedBy]
    )
    VALUES 
    -- Work Item field mappings
    (@SampleProjectId, 1, 'subject', 'System.Title', 0, 1, 'System'),
    (@SampleProjectId, 1, 'description', 'System.Description', 0, 0, 'System'),
    (@SampleProjectId, 1, 'status', 'System.State', 1, 1, 'System'),
    (@SampleProjectId, 1, 'priority', 'Microsoft.VSTS.Common.Priority', 1, 0, 'System'),
    (@SampleProjectId, 1, 'assigned_to', 'System.AssignedTo', 0, 0, 'System'),
    (@SampleProjectId, 1, 'author', 'System.CreatedBy', 0, 0, 'System'),
    (@SampleProjectId, 1, 'tracker', 'System.WorkItemType', 1, 1, 'System'),
    (@SampleProjectId, 1, 'due_date', 'Microsoft.VSTS.Scheduling.DueDate', 0, 0, 'System'),
    (@SampleProjectId, 1, 'estimated_hours', 'Microsoft.VSTS.Scheduling.OriginalEstimate', 0, 0, 'System'),
    (@SampleProjectId, 1, 'done_ratio', 'Microsoft.VSTS.Scheduling.CompletedWork', 1, 0, 'System'),
    
    -- Comment field mappings
    (@SampleProjectId, 2, 'notes', 'System.History', 0, 1, 'System'),
    (@SampleProjectId, 2, 'user', 'System.ChangedBy', 0, 0, 'System'),
    (@SampleProjectId, 2, 'created_on', 'System.ChangedDate', 0, 0, 'System'),
    
    -- Attachment field mappings
    (@SampleProjectId, 3, 'filename', 'System.AttachedFiles.Name', 0, 1, 'System'),
    (@SampleProjectId, 3, 'filesize', 'System.AttachedFiles.Size', 0, 0, 'System'),
    (@SampleProjectId, 3, 'content_type', 'System.AttachedFiles.ContentType', 0, 0, 'System'),
    (@SampleProjectId, 3, 'description', 'System.AttachedFiles.Comment', 0, 0, 'System');
    
    PRINT 'Sample project and field mappings created.';
END
ELSE
BEGIN
    PRINT 'Projects already exist, skipping sample data creation.';
END

-- Create any missing indexes that might not be in separate files
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SyncConflicts_Unresolved' AND object_id = OBJECT_ID('dbo.SyncConflicts'))
BEGIN
    PRINT 'Creating index for unresolved conflicts...';
    CREATE NONCLUSTERED INDEX [IX_SyncConflicts_Unresolved] 
    ON [dbo].[SyncConflicts] ([SyncOperationId], [ResolvedAt])
    WHERE [ResolvedAt] IS NULL;
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_SyncOperations_Status_StartTime' AND object_id = OBJECT_ID('dbo.SyncOperations'))
BEGIN
    PRINT 'Creating index for active sync operations...';
    CREATE NONCLUSTERED INDEX [IX_SyncOperations_Status_StartTime] 
    ON [dbo].[SyncOperations] ([Status], [StartTime] DESC)
    INCLUDE ([ProjectId], [LastHeartbeat]);
END

-- Update statistics for better query performance
IF EXISTS (SELECT 1 FROM [dbo].[Projects])
BEGIN
    PRINT 'Updating statistics...';
    UPDATE STATISTICS [dbo].[Projects];
    UPDATE STATISTICS [dbo].[WorkItems];
    UPDATE STATISTICS [dbo].[SyncOperations];
    UPDATE STATISTICS [dbo].[OutboxEvents];
END

PRINT 'Seed data script completed.';

-- Display summary
SELECT 
    'Database Objects' AS [Category],
    'Projects' AS [Object],
    COUNT(*) AS [Count]
FROM [dbo].[Projects]

UNION ALL

SELECT 
    'Database Objects' AS [Category],
    'Field Mappings' AS [Object],
    COUNT(*) AS [Count]
FROM [dbo].[FieldMappings]

UNION ALL

SELECT 
    'Database Objects' AS [Category],
    'Work Items' AS [Object],
    COUNT(*) AS [Count]
FROM [dbo].[WorkItems]

UNION ALL

SELECT 
    'Database Objects' AS [Category],
    'Sync Operations' AS [Object],
    COUNT(*) AS [Count]
FROM [dbo].[SyncOperations]

UNION ALL

SELECT 
    'Database Objects' AS [Category],
    'Outbox Events' AS [Object],
    COUNT(*) AS [Count]
FROM [dbo].[OutboxEvents];

PRINT 'RedOps database is ready for use.';