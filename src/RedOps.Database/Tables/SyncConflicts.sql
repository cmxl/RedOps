CREATE TABLE [dbo].[SyncConflicts] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [SyncOperationId] UNIQUEIDENTIFIER NOT NULL,
    [WorkItemId] UNIQUEIDENTIFIER NULL, -- NULL for project-level conflicts
    [EntityType] INT NOT NULL, -- 0=Project, 1=WorkItem, 2=Comment, 3=Attachment
    [ConflictType] INT NOT NULL, -- 0=DataMismatch, 1=DuplicateCreation, 2=DeletedInSource, 3=ModifiedInBoth
    [FieldName] NVARCHAR(100) NULL, -- Specific field that has conflict
    [RedmineValue] NVARCHAR(MAX) NULL,
    [AzureDevOpsValue] NVARCHAR(MAX) NULL,
    [LocalValue] NVARCHAR(MAX) NULL, -- Current value in our system
    [ConflictData] NVARCHAR(MAX) NULL, -- JSON object with additional conflict details
    [Resolution] INT NULL, -- 0=UseRedmine, 1=UseAzureDevOps, 2=UseLocal, 3=Merge, 4=Skip
    [ResolutionNotes] NVARCHAR(MAX) NULL,
    [AutoResolvable] BIT NOT NULL DEFAULT 0,
    [ResolvedAt] DATETIME2(7) NULL,
    [ResolvedBy] NVARCHAR(255) NULL,
    [IsResolved] AS CASE WHEN [ResolvedAt] IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_SyncConflicts] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SyncConflicts_SyncOperationId] FOREIGN KEY ([SyncOperationId]) REFERENCES [dbo].[SyncOperations] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_SyncConflicts_WorkItemId] FOREIGN KEY ([WorkItemId]) REFERENCES [dbo].[WorkItems] ([Id]),
    CONSTRAINT [CK_SyncConflicts_EntityType] CHECK ([EntityType] IN (0, 1, 2, 3)),
    CONSTRAINT [CK_SyncConflicts_ConflictType] CHECK ([ConflictType] IN (0, 1, 2, 3)),
    CONSTRAINT [CK_SyncConflicts_Resolution] CHECK ([Resolution] IS NULL OR [Resolution] IN (0, 1, 2, 3, 4)),
    CONSTRAINT [CK_SyncConflicts_ConflictData_JSON] CHECK ([ConflictData] IS NULL OR ISJSON([ConflictData]) = 1)
);