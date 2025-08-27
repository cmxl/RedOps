CREATE PROCEDURE [dbo].[sp_CleanupOldSyncOperations]
    @RetentionDays INT = 30,
    @BatchSize INT = 1000,
    @DryRun BIT = 0
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @CutoffDate DATETIME2(7) = DATEADD(DAY, -@RetentionDays, GETUTCDATE());
    DECLARE @DeletedCount INT = 0;
    DECLARE @TotalDeleted INT = 0;
    DECLARE @MaxIterations INT = 100; -- Safety limit
    DECLARE @Iteration INT = 0;
    
    -- Log cleanup start
    PRINT 'Starting cleanup of sync operations older than ' + CAST(@RetentionDays AS VARCHAR(10)) + ' days (before ' + CONVERT(VARCHAR(23), @CutoffDate, 121) + ')';
    
    IF @DryRun = 1
    BEGIN
        PRINT 'DRY RUN MODE - No data will be deleted';
        
        -- Show what would be deleted
        SELECT 
            COUNT(*) AS [OperationsToDelete],
            MIN([StartTime]) AS [OldestOperation],
            MAX([StartTime]) AS [NewestOperationToDelete]
        FROM [dbo].[SyncOperations]
        WHERE [StartTime] < @CutoffDate
          AND [Status] IN (2, 3, 4); -- Only completed, failed, or cancelled operations
        
        SELECT 
            COUNT(sc.[Id]) AS [ConflictsToDelete]
        FROM [dbo].[SyncConflicts] sc
        INNER JOIN [dbo].[SyncOperations] so ON sc.[SyncOperationId] = so.[Id]
        WHERE so.[StartTime] < @CutoffDate
          AND so.[Status] IN (2, 3, 4);
          
        RETURN;
    END
    
    BEGIN TRANSACTION;
    
    BEGIN TRY
        -- Delete in batches to avoid locking issues
        WHILE @Iteration < @MaxIterations
        BEGIN
            SET @Iteration = @Iteration + 1;
            
            -- Delete sync conflicts first (foreign key dependency)
            DELETE TOP(@BatchSize) sc
            FROM [dbo].[SyncConflicts] sc
            INNER JOIN [dbo].[SyncOperations] so ON sc.[SyncOperationId] = so.[Id]
            WHERE so.[StartTime] < @CutoffDate
              AND so.[Status] IN (2, 3, 4); -- Only completed, failed, or cancelled operations
            
            SET @DeletedCount = @@ROWCOUNT;
            SET @TotalDeleted = @TotalDeleted + @DeletedCount;
            
            PRINT 'Batch ' + CAST(@Iteration AS VARCHAR(10)) + ': Deleted ' + CAST(@DeletedCount AS VARCHAR(10)) + ' conflict records';
            
            -- If we deleted fewer than the batch size, we're done with conflicts
            IF @DeletedCount < @BatchSize
                BREAK;
        END
        
        SET @Iteration = 0;
        SET @DeletedCount = 0;
        
        -- Now delete sync operations
        WHILE @Iteration < @MaxIterations
        BEGIN
            SET @Iteration = @Iteration + 1;
            
            DELETE TOP(@BatchSize)
            FROM [dbo].[SyncOperations]
            WHERE [StartTime] < @CutoffDate
              AND [Status] IN (2, 3, 4); -- Only completed, failed, or cancelled operations
            
            SET @DeletedCount = @@ROWCOUNT;
            SET @TotalDeleted = @TotalDeleted + @DeletedCount;
            
            PRINT 'Batch ' + CAST(@Iteration AS VARCHAR(10)) + ': Deleted ' + CAST(@DeletedCount AS VARCHAR(10)) + ' sync operation records';
            
            -- If we deleted fewer than the batch size, we're done
            IF @DeletedCount < @BatchSize
                BREAK;
        END
        
        -- Clean up old processed outbox events
        DELETE TOP(@BatchSize * 2)
        FROM [dbo].[OutboxEvents]
        WHERE [ProcessedAt] IS NOT NULL
          AND [ProcessedAt] < @CutoffDate;
        
        SET @DeletedCount = @@ROWCOUNT;
        SET @TotalDeleted = @TotalDeleted + @DeletedCount;
        
        PRINT 'Deleted ' + CAST(@DeletedCount AS VARCHAR(10)) + ' processed outbox events';
        
        COMMIT TRANSACTION;
        
        PRINT 'Cleanup completed successfully. Total records deleted: ' + CAST(@TotalDeleted AS VARCHAR(10));
        
        -- Show remaining counts
        SELECT 
            'Summary' AS [Type],
            COUNT(*) AS [RemainingOperations],
            MIN([StartTime]) AS [OldestRemaining]
        FROM [dbo].[SyncOperations]
        
        UNION ALL
        
        SELECT 
            'Conflicts' AS [Type],
            COUNT(*) AS [RemainingConflicts],
            MIN([CreatedAt]) AS [OldestRemaining]
        FROM [dbo].[SyncConflicts]
        
        UNION ALL
        
        SELECT 
            'OutboxEvents' AS [Type],
            COUNT(*) AS [RemainingEvents],
            MIN([CreatedAt]) AS [OldestRemaining]
        FROM [dbo].[OutboxEvents];
        
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        DECLARE @ErrorSeverity INT = ERROR_SEVERITY();
        DECLARE @ErrorState INT = ERROR_STATE();
        
        PRINT 'Cleanup failed: ' + @ErrorMessage;
        RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
    END CATCH
END;