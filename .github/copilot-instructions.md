# Copilot Custom Instructions for DBA Projects

These rules apply to all Copilot interactions in this workspace.

## Lab Environment

> **Note:** Environment-specific values (server name, resource group) are stored in `.env` at the repo root.
> This file is git-ignored. Read `.env` to resolve `<unique-suffix>` placeholders below.

### Connection Details
- **Project:** RetailDb (Retail Lab)
- **Platform:** Azure SQL Database (PaaS)
- **Server:** `sql-retail-lab-<unique-suffix>.database.windows.net`
- **Database:** `RetailDb`
- **Resource Group:** `rg-retail-lab`
- **Region:** Canada Central
- **SKU:** Standard S1 (20 DTUs)

### Authentication
- Authentication is **Azure AD-only** (no SQL auth)
- Use `Authentication=Active Directory Default` in connection strings
- The MSSQL extension connection profile `RetailLab` is pre-configured

### How to Connect
- Use the **MSSQL extension** in VS Code — connect via the `RetailLab` profile
- Or use Azure CLI: `az sql query --server sql-retail-lab-<unique-suffix> --database RetailDb --resource-group rg-retail-lab`

### How to Query
- Use the `mssql_run_query` tool with `connectionId` from `mssql_connect`
- Always include `queryTypes` (e.g., `["SELECT"]`) and `queryIntent` (e.g., `"troubleshooting"`)
- Connect first: `mssql_connect` with server `sql-retail-lab-<unique-suffix>.database.windows.net` and database `RetailDb`

### Schema Overview
The database has 10 business tables plus EF Core migrations tracking:

| Table | Description | Key Relationships |
|-------|-------------|-------------------|
| Categories | Product categories (self-referencing hierarchy) | ParentCategoryId → Categories |
| Suppliers | Product suppliers | — |
| Products | Product catalog | CategoryId → Categories, SupplierId → Suppliers |
| Customers | Customer registry | — |
| Stores | Retail store locations | — |
| Employees | Store employees (self-referencing manager hierarchy) | StoreId → Stores, ManagerId → Employees |
| Orders | Customer orders | CustomerId → Customers, StoreId → Stores, EmployeeId → Employees |
| OrderItems | Order line items | OrderId → Orders, ProductId → Products |
| Inventories | Stock levels per product per store | ProductId → Products, StoreId → Stores |
| Reviews | Product reviews by customers | ProductId → Products, CustomerId → Customers |

### Key Configuration
- Compatibility Level: 170 (SQL Server 2022)
- Query Store: Enabled (READ_WRITE)
- MAXDOP: 8
- Collation: SQL_Latin1_General_CP1_CI_AS
- TDE: Enabled

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
