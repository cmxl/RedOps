CREATE TABLE [dbo].[Comments] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [WorkItemId] UNIQUEIDENTIFIER NOT NULL,
    [Content] NVARCHAR(MAX) NOT NULL,
    [Author] NVARCHAR(255) NOT NULL,
    [RedmineId] INT NULL,
    [AzureDevOpsId] INT NULL,
    [IsSystemGenerated] BIT NOT NULL DEFAULT 0,
    [LastSyncUtc] DATETIME2(7) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_Comments] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Comments_WorkItemId] FOREIGN KEY ([WorkItemId]) REFERENCES [dbo].[WorkItems] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_Comments_WorkItemId_RedmineId] UNIQUE ([WorkItemId], [RedmineId]),
    CONSTRAINT [UQ_Comments_WorkItemId_AzureDevOpsId] UNIQUE ([WorkItemId], [AzureDevOpsId])
);