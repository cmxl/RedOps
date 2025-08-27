CREATE TABLE [dbo].[SyncOperations] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ProjectId] UNIQUEIDENTIFIER NOT NULL,
    [OperationType] INT NOT NULL, -- 0=FullSync, 1=IncrementalSync, 2=ConflictResolution
    [SyncDirection] INT NOT NULL, -- 0=None, 1=RedmineToAzure, 2=AzureToRedmine, 3=Bidirectional
    [Status] INT NOT NULL DEFAULT 0, -- 0=Pending, 1=InProgress, 2=Completed, 3=Failed, 4=Cancelled
    [StartTime] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [EndTime] DATETIME2(7) NULL,
    [Duration] AS DATEDIFF(MILLISECOND, [StartTime], ISNULL([EndTime], GETUTCDATE())),
    [ItemsProcessed] INT NOT NULL DEFAULT 0,
    [ItemsCreated] INT NOT NULL DEFAULT 0,
    [ItemsUpdated] INT NOT NULL DEFAULT 0,
    [ItemsSkipped] INT NOT NULL DEFAULT 0,
    [ItemsFailed] INT NOT NULL DEFAULT 0,
    [ConflictsDetected] INT NOT NULL DEFAULT 0,
    [ErrorMessage] NVARCHAR(MAX) NULL,
    [ErrorDetails] NVARCHAR(MAX) NULL, -- JSON object with detailed error info
    [ProgressDetails] NVARCHAR(MAX) NULL, -- JSON object with progress info
    [TriggerSource] NVARCHAR(50) NULL, -- Manual, Scheduled, Webhook, etc.
    [InitiatedBy] NVARCHAR(255) NULL,
    [LastHeartbeat] DATETIME2(7) NULL, -- For detecting stuck operations
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_SyncOperations] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SyncOperations_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [CK_SyncOperations_OperationType] CHECK ([OperationType] IN (0, 1, 2)),
    CONSTRAINT [CK_SyncOperations_SyncDirection] CHECK ([SyncDirection] IN (0, 1, 2, 3)),
    CONSTRAINT [CK_SyncOperations_Status] CHECK ([Status] IN (0, 1, 2, 3, 4)),
    CONSTRAINT [CK_SyncOperations_EndTime] CHECK ([EndTime] IS NULL OR [EndTime] >= [StartTime]),
    CONSTRAINT [CK_SyncOperations_ItemCounts] CHECK (
        [ItemsProcessed] >= 0 AND 
        [ItemsCreated] >= 0 AND 
        [ItemsUpdated] >= 0 AND 
        [ItemsSkipped] >= 0 AND 
        [ItemsFailed] >= 0 AND
        [ConflictsDetected] >= 0
    ),
    CONSTRAINT [CK_SyncOperations_ErrorDetails_JSON] CHECK ([ErrorDetails] IS NULL OR ISJSON([ErrorDetails]) = 1),
    CONSTRAINT [CK_SyncOperations_ProgressDetails_JSON] CHECK ([ProgressDetails] IS NULL OR ISJSON([ProgressDetails]) = 1)
);