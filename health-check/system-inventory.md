# System Inventory — RetailDb

**Report Date:** 2026-03-23

---

## Server Environment

| Property | Value |
|----------|-------|
| **Server Name** | sql-retail-lab-ug5lhmwwszwla |
| **FQDN** | sql-retail-lab-ug5lhmwwszwla.database.windows.net |
| **Platform** | Azure SQL Database (PaaS) |
| **Engine Version** | Microsoft SQL Azure (RTM) - 12.0.2000.8 (Feb 7 2026) |
| **Engine Edition** | 5 (SQL Database) |
| **Edition** | Standard |
| **Service Objective** | S1 (20 DTUs) |
| **Region** | Canada Central |
| **Resource Group** | rg-retail-lab |

## Database Configuration

| Property | Value |
|----------|-------|
| **Database Name** | RetailDb |
| **Compatibility Level** | 170 (SQL Server 2022) |
| **Collation** | SQL_Latin1_General_CP1_CI_AS |
| **Max Size** | 10 GB (10,737,418,240 bytes) |
| **TDE (Encryption)** | Enabled — AES-256 |
| **Query Store** | READ_WRITE (Active) |
| **MAXDOP** | 8 |
| **OPTIMIZE_FOR_AD_HOC_WORKLOADS** | OFF |
| **LEGACY_CARDINALITY_ESTIMATION** | OFF |
| **PARAMETER_SNIFFING** | ON |
| **PARAMETER_SENSITIVE_PLAN_OPTIMIZATION** | ON |
| **BATCH_MODE_ON_ROWSTORE** | ON |

## Database Files

| File | Type | Size (MB) | Used (MB) | Free (MB) | Auto-Growth | Max Size |
|------|------|-----------|-----------|-----------|-------------|----------|
| data_0 | ROWS | 32.00 | 25.06 | 6.94 | 16 MB | 10 GB |
| log | LOG | 8.00 | 1.16 | 6.84 | 16 MB | 1 TB |
| XTP | FILESTREAM | Azure-managed | — | — | — | — |

## Table Row Counts

| Schema | Table | Rows | Space (MB) |
|--------|-------|------|-----------|
| dbo | Inventories | 130 | 0.07 |
| dbo | OrderItems | 37 | 0.07 |
| dbo | Products | 26 | 0.07 |
| dbo | Orders | 21 | 0.07 |
| dbo | Categories | 16 | 0.07 |
| dbo | Customers | 15 | 0.07 |
| dbo | Employees | 12 | 0.07 |
| dbo | Reviews | 10 | 0.07 |
| dbo | Suppliers | 6 | 0.07 |
| dbo | Stores | 5 | 0.07 |
| dbo | __EFMigrationsHistory | 1 | 0.07 |

**Total user data:** ~0.77 MB across 11 tables (279 rows)

## Database Objects

| Object Type | Count |
|-------------|-------|
| User Tables | 11 |
| Primary Key Constraints | 11 |
| Foreign Key Constraints | 14 |
| Stored Procedures | 0 |
| Views | 0 |
| Functions | 0 |

## Query Store Configuration

| Setting | Value |
|---------|-------|
| State | READ_WRITE |
| Storage Used | 1 MB / 100 MB (1%) |
| Capture Mode | AUTO |
| Interval Length | 60 minutes |
| Stale Query Threshold | 30 days |
| Size-Based Cleanup | AUTO |

## Key Database-Scoped Configurations

| Setting | Value |
|---------|-------|
| ACCELERATED_PLAN_FORCING | ON |
| BATCH_MODE_ADAPTIVE_JOINS | ON |
| BATCH_MODE_MEMORY_GRANT_FEEDBACK | ON |
| BATCH_MODE_ON_ROWSTORE | ON |
| CE_FEEDBACK | ON |
| DEFERRED_COMPILATION_TV | ON |
| DOP_FEEDBACK | ON |
| IDENTITY_CACHE | ON |
| INTERLEAVED_EXECUTION_TVF | ON |
| LIGHTWEIGHT_QUERY_PROFILING | ON |
| MAXDOP | 8 |
| OPTIMIZE_FOR_AD_HOC_WORKLOADS | OFF |
| PARAMETER_SENSITIVE_PLAN_OPTIMIZATION | ON |

## Authentication

- **Mode:** Azure AD-only (no SQL authentication)
- **Database Users:** 1 (dbo — db_owner)

## Maintenance & Backups

Azure SQL Database provides automated maintenance:
- **Automated backups:** Full (weekly), differential (12-hourly), transaction log (5–10 min)
- **Retention:** 7 days (default for Standard tier; configurable up to 35 days)
- **DBCC CHECKDB:** Managed automatically by Azure
- **Index maintenance:** Not scheduled (no SQL Agent on Azure SQL Database)
