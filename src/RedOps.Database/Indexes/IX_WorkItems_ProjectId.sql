CREATE NONCLUSTERED INDEX [IX_WorkItems_ProjectId] 
ON [dbo].[WorkItems] ([ProjectId])
INCLUDE ([Title], [Status], [CreatedAt], [UpdatedAt]);