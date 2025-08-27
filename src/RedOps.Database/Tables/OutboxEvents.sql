CREATE TABLE [dbo].[OutboxEvents] (
    [Id] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [EventId] UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    [EventType] NVARCHAR(255) NOT NULL,
    [EventData] NVARCHAR(MAX) NOT NULL, -- JSON serialized event data
    [AggregateId] UNIQUEIDENTIFIER NULL, -- ID of the aggregate that produced this event
    [AggregateType] NVARCHAR(255) NULL,
    [EventVersion] INT NOT NULL DEFAULT 1,
    [CorrelationId] UNIQUEIDENTIFIER NULL,
    [CausationId] UNIQUEIDENTIFIER NULL,
    [UserId] NVARCHAR(255) NULL,
    [ProcessedAt] DATETIME2(7) NULL,
    [RetryCount] INT NOT NULL DEFAULT 0,
    [MaxRetries] INT NOT NULL DEFAULT 3,
    [NextRetryAt] DATETIME2(7) NULL,
    [LastError] NVARCHAR(MAX) NULL,
    [IsProcessed] AS CASE WHEN [ProcessedAt] IS NOT NULL THEN CAST(1 AS BIT) ELSE CAST(0 AS BIT) END,
    [CreatedAt] DATETIME2(7) NOT NULL DEFAULT GETUTCDATE(),
    
    CONSTRAINT [PK_OutboxEvents] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [UQ_OutboxEvents_EventId] UNIQUE ([EventId]),
    CONSTRAINT [CK_OutboxEvents_EventVersion] CHECK ([EventVersion] > 0),
    CONSTRAINT [CK_OutboxEvents_RetryCount] CHECK ([RetryCount] >= 0),
    CONSTRAINT [CK_OutboxEvents_MaxRetries] CHECK ([MaxRetries] >= 0),
    CONSTRAINT [CK_OutboxEvents_RetryCount_MaxRetries] CHECK ([RetryCount] <= [MaxRetries]),
    CONSTRAINT [CK_OutboxEvents_EventData_JSON] CHECK ([EventData] IS NOT NULL AND ISJSON([EventData]) = 1),
    CONSTRAINT [CK_OutboxEvents_LastError_JSON] CHECK ([LastError] IS NULL OR ISJSON([LastError]) = 1)
);