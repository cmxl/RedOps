CREATE TABLE [dbo].[FieldMappings]
(
    [Id]                  UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
    [ProjectId]           UNIQUEIDENTIFIER NOT NULL,
    [EntityType]          INT              NOT NULL,           -- 0=Project, 1=WorkItem, 2=Comment, 3=Attachment
    [RedmineField]        NVARCHAR(100)    NOT NULL,
    [AzureDevOpsField]    NVARCHAR(100)    NOT NULL,
    [MappingType]         INT              NOT NULL DEFAULT 0, -- 0=Direct, 1=Transform, 2=Lookup, 3=Custom
    [TransformExpression] NVARCHAR(500)    NULL,               -- For complex transformations
    [DefaultValue]        NVARCHAR(255)    NULL,
    [IsRequired]          BIT              NOT NULL DEFAULT 0,
    [IsActive]            BIT              NOT NULL DEFAULT 1,
    [MappingRules]        NVARCHAR(MAX)    NULL,               -- JSON object with mapping rules and conditions
    [CreatedAt]           DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [UpdatedAt]           DATETIME2(7)     NOT NULL DEFAULT GETUTCDATE(),
    [CreatedBy]           NVARCHAR(255)    NULL,
    [UpdatedBy]           NVARCHAR(255)    NULL,

    CONSTRAINT [PK_FieldMappings] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_FieldMappings_ProjectId] FOREIGN KEY ([ProjectId]) REFERENCES [dbo].[Projects] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [UQ_FieldMappings_Project_Entity_Fields] UNIQUE ([ProjectId], [EntityType], [RedmineField], [AzureDevOpsField]),
    CONSTRAINT [CK_FieldMappings_EntityType] CHECK ([EntityType] IN (0, 1, 2, 3)),
    CONSTRAINT [CK_FieldMappings_MappingType] CHECK ([MappingType] IN (0, 1, 2, 3)),
    CONSTRAINT [CK_FieldMappings_MappingRules_JSON] CHECK ([MappingRules] IS NULL OR ISJSON([MappingRules]) = 1)
);