CREATE NONCLUSTERED INDEX [IX_OutboxEvents_CreatedAt] 
ON [dbo].[OutboxEvents] ([CreatedAt], [ProcessedAt])
WHERE [ProcessedAt] IS NULL;