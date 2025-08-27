CREATE NONCLUSTERED INDEX [IX_SyncOperations_ProjectId_StartTime] 
ON [dbo].[SyncOperations] ([ProjectId], [StartTime] DESC)
INCLUDE ([Status], [OperationType], [SyncDirection], [EndTime], [ItemsProcessed]);