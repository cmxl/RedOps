CREATE NONCLUSTERED INDEX [IX_WorkItems_AzureDevOpsId] 
ON [dbo].[WorkItems] ([ProjectId], [AzureDevOpsId])
WHERE [AzureDevOpsId] IS NOT NULL;