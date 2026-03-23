-- ============================================
-- Rollback: Drop stored procedure usp_AddProduct
-- Author: DBA Team
-- Date: 2026-03-23
-- Ticket: DEMO-001
-- ============================================

-- Pre-check: verify procedure exists
-- SELECT OBJECT_ID('dbo.usp_AddProduct', 'P');

BEGIN TRANSACTION;
BEGIN TRY

    DROP PROCEDURE IF EXISTS dbo.usp_AddProduct;

    COMMIT TRANSACTION;
    PRINT 'Rollback completed successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH

-- Post-check: verify procedure is gone
-- SELECT OBJECT_ID('dbo.usp_AddProduct', 'P');
