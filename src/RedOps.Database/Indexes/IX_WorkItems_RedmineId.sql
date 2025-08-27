CREATE NONCLUSTERED INDEX [IX_WorkItems_RedmineId] 
ON [dbo].[WorkItems] ([ProjectId], [RedmineId])
WHERE [RedmineId] IS NOT NULL;