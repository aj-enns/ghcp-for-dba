-- ============================================
-- Migration: Create stored procedure usp_AddProduct
-- Author: DBA Team
-- Date: 2026-03-23
-- Ticket: DEMO-001
-- ============================================

-- Pre-check: verify procedure does not already exist
-- SELECT OBJECT_ID('dbo.usp_AddProduct', 'P');

BEGIN TRANSACTION;
BEGIN TRY

    CREATE OR ALTER PROCEDURE dbo.usp_AddProduct
        @Sku            NVARCHAR(50),
        @Name           NVARCHAR(200),
        @Description    NVARCHAR(1000) = NULL,
        @CategoryId     INT,
        @SupplierId     INT,
        @UnitPrice      DECIMAL(18,2),
        @CostPrice      DECIMAL(18,2) = NULL,
        @ReorderLevel   INT = 10,
        @ReorderQuantity INT = 50,
        @ProductId      INT OUTPUT
    AS
    BEGIN
        SET NOCOUNT ON;

        -- Validate SKU is unique
        IF EXISTS (SELECT 1 FROM dbo.Products WHERE Sku = @Sku)
        BEGIN
            RAISERROR('A product with SKU ''%s'' already exists.', 16, 1, @Sku);
            RETURN;
        END

        -- Validate foreign keys exist
        IF NOT EXISTS (SELECT 1 FROM dbo.Categories WHERE CategoryId = @CategoryId)
        BEGIN
            RAISERROR('CategoryId %d does not exist.', 16, 1, @CategoryId);
            RETURN;
        END

        IF NOT EXISTS (SELECT 1 FROM dbo.Suppliers WHERE SupplierId = @SupplierId)
        BEGIN
            RAISERROR('SupplierId %d does not exist.', 16, 1, @SupplierId);
            RETURN;
        END

        -- Validate price
        IF @UnitPrice < 0
        BEGIN
            RAISERROR('UnitPrice cannot be negative.', 16, 1);
            RETURN;
        END

        INSERT INTO dbo.Products (
            Sku,
            Name,
            Description,
            CategoryId,
            SupplierId,
            UnitPrice,
            CostPrice,
            ReorderLevel,
            ReorderQuantity,
            IsDiscontinued,
            CreatedAt,
            UpdatedAt
        )
        VALUES (
            @Sku,
            @Name,
            @Description,
            @CategoryId,
            @SupplierId,
            @UnitPrice,
            @CostPrice,
            @ReorderLevel,
            @ReorderQuantity,
            0,              -- IsDiscontinued = false
            SYSUTCDATETIME(),
            NULL
        );

        SET @ProductId = SCOPE_IDENTITY();
    END;

    COMMIT TRANSACTION;
    PRINT 'Migration completed successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH

-- Post-check: verify procedure exists
-- SELECT OBJECT_ID('dbo.usp_AddProduct', 'P');
