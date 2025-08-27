CREATE TABLE [dbo].[Projects] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [Name] NVARCHAR(255) NOT NULL,
    [Description] NVARCHAR(MAX) NULL,
    [RedmineId] INT NULL,
    [AzureDevOpsProject] NVARCHAR(255) NULL,
    [SyncDirection] INT NOT NULL DEFAULT 0, -- 0=None, 1=RedmineToAzure, 2=AzureToRedmine, 3=Bidirectional
    [IsActive] BIT NOT NULL DEFAULT 1,
    [LastSyncUtc] DATETIME2(7) NULL,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy] NVARCHAR(255) NULL,
    [UpdatedBy] NVARCHAR(255) NULL,
    
    CONSTRAINT [PK_Projects] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_Projects_RedmineId] UNIQUE ([RedmineId]),
    CONSTRAINT [UQ_Projects_AzureDevOpsProject] UNIQUE ([AzureDevOpsProject]),
    CONSTRAINT [CK_Projects_SyncDirection] CHECK ([SyncDirection] IN (0, 1, 2, 3)),
    CONSTRAINT [CK_Projects_RedmineOrAzure] CHECK (
        ([RedmineId] IS NOT NULL AND [AzureDevOpsProject] IS NOT NULL) OR
        ([RedmineId] IS NULL AND [AzureDevOpsProject] IS NULL)
    )
);