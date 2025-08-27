CREATE NONCLUSTERED INDEX [IX_Projects_RedmineId] 
ON [dbo].[Projects] ([RedmineId])
WHERE [RedmineId] IS NOT NULL;