CREATE FUNCTION [dbo].[fn_GetSyncDirectionText](@SyncDirection INT)
RETURNS NVARCHAR(50)
AS
BEGIN
    DECLARE @Result NVARCHAR(50);
    
    SET @Result = CASE @SyncDirection
        WHEN 0 THEN 'None'
        WHEN 1 THEN 'Redmine → Azure DevOps'
        WHEN 2 THEN 'Azure DevOps → Redmine'
        WHEN 3 THEN 'Bidirectional'
        ELSE 'Unknown'
    END;
    
    RETURN @Result;
END;