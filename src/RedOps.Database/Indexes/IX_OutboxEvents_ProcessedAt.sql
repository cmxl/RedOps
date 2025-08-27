CREATE NONCLUSTERED INDEX [IX_OutboxEvents_ProcessedAt] 
ON [dbo].[OutboxEvents] ([ProcessedAt])
INCLUDE ([CreatedAt], [RetryCount], [EventType]);