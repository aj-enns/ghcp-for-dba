# Copilot Custom Instructions for DBA Projects

These rules apply to all Copilot interactions in this workspace.

## SQL Coding Standards

### Naming Conventions
- Tables: `PascalCase` singular nouns (e.g., `Customer`, `OrderItem`)
- Columns: `PascalCase` (e.g., `FirstName`, `OrderDate`)
- Primary keys: `Id` or `TableNameId` (be consistent within a project)
- Foreign keys: `ReferencedTableId` (e.g., `CustomerId` in `Order` table)
- Indexes: `IX_TableName_Column1_Column2`
- Unique constraints: `UQ_TableName_Column`
- Check constraints: `CK_TableName_Description`
- Default constraints: `DF_TableName_Column`
- Stored procedures: `usp_VerbNoun` (e.g., `usp_GetCustomerOrders`)
- Functions: `ufn_VerbNoun` (e.g., `ufn_CalculateDiscount`)
- Views: `vw_Description` (e.g., `vw_ActiveCustomers`)

### Formatting Rules
- Use uppercase for SQL keywords (`SELECT`, `FROM`, `WHERE`, `JOIN`)
- One clause per line for readability
- Indent JOIN conditions, subqueries, and CASE expressions
- Always qualify column names with table aliases in multi-table queries
- Use meaningful table aliases (not single letters unless obvious)

### Safety Rules
- Every DDL script must include a corresponding rollback script
- Never use `DROP` without `IF EXISTS`
- Always include `SET NOCOUNT ON` in stored procedures
- Wrap data-modifying operations in explicit transactions with error handling
- Include a comment header block in every script: author, date, description, ticket reference

## Migration Scripts

### Required Format
```sql
-- ============================================
-- Migration: [description]
-- Author: [name]
-- Date: [YYYY-MM-DD]
-- Ticket: [reference]
-- ============================================

-- Pre-check: verify current state
-- [validation query]

BEGIN TRANSACTION;
BEGIN TRY
    -- [migration logic here]
    COMMIT TRANSACTION;
    PRINT 'Migration completed successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH

-- Post-check: verify migration result
-- [validation query]
```

### Rollback Script
Every migration must have a paired rollback script named `rollback_[original_name].sql`.

## Code Review Checklist

When reviewing SQL changes, verify:

- [ ] No `SELECT *` in production code
- [ ] All new tables have primary keys
- [ ] Foreign keys have appropriate indexes
- [ ] Data types are appropriate (no `VARCHAR(MAX)` for short strings, no `FLOAT` for currency)
- [ ] NULLability is intentional and documented
- [ ] Error handling is present for data modifications
- [ ] Rollback script exists and is tested
- [ ] No hardcoded values that should be parameters
- [ ] Query plans reviewed for expensive operations
