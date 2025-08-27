CREATE NONCLUSTERED INDEX [IX_Projects_AzureDevOpsProject] 
ON [dbo].[Projects] ([AzureDevOpsProject])
WHERE [AzureDevOpsProject] IS NOT NULL;