-- Migration Script: Update Task Status Values After Postponed Removal
-- Execute this AFTER running the EF Core migration
-- This migrates existing data to the new status values

-- Step 1: Backup current data (recommended)
-- Run this in a transaction for safety

BEGIN TRANSACTION;

-- Step 2: Show current status distribution (for verification)
SELECT 
    Status,
    COUNT(*) as TaskCount,
    CASE 
        WHEN Status = 1 THEN 'ToDo'
        WHEN Status = 2 THEN 'Pending'
        WHEN Status = 3 THEN 'Postponed'
        WHEN Status = 4 THEN 'Completed'
        ELSE 'Unknown'
    END as StatusName
FROM UserTasks
GROUP BY Status
ORDER BY Status;

-- Step 3: Migrate Postponed tasks to Pending
-- All tasks with Status = 3 (Postponed) will become Status = 2 (Pending)
UPDATE UserTasks 
SET Status = 2,
    UpdatedAt = GETUTCDATE()
WHERE Status = 3;

PRINT 'Postponed tasks migrated to Pending: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- Step 4: Update Completed tasks from 4 to 3
UPDATE UserTasks 
SET Status = 3
WHERE Status = 4;

PRINT 'Completed tasks updated: ' + CAST(@@ROWCOUNT AS VARCHAR(10));

-- Step 5: Verify new status distribution
SELECT 
    Status,
    COUNT(*) as TaskCount,
    CASE 
        WHEN Status = 1 THEN 'ToDo'
        WHEN Status = 2 THEN 'Pending'
        WHEN Status = 3 THEN 'Completed'
        ELSE 'Unknown'
    END as StatusName
FROM UserTasks
GROUP BY Status
ORDER BY Status;

-- Step 6: Verify no invalid status values exist
IF EXISTS (SELECT 1 FROM UserTasks WHERE Status NOT IN (1, 2, 3))
BEGIN
    PRINT 'WARNING: Invalid status values found!';
    SELECT * FROM UserTasks WHERE Status NOT IN (1, 2, 3);
    ROLLBACK TRANSACTION;
END
ELSE
BEGIN
    PRINT 'All status values are valid. Migration successful!';
    COMMIT TRANSACTION;
END

GO
