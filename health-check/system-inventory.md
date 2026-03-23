# System Inventory — RetailDb Health Check

**Report Date:** 2026-03-23  
**Assessed By:** GitHub Copilot (Automated Health Check)

---

## Server Information

| Property | Value |
|----------|-------|
| **Platform** | Azure SQL Database (PaaS) |
| **Server FQDN** | `sql-retail-lab-ug5lhmwwszwla.database.windows.net` |
| **Engine Version** | Microsoft SQL Azure (RTM) - 12.0.2000.8 (Feb 7 2026) |
| **Edition** | SQL Azure (Engine Edition 5) |
| **Product Level** | RTM |
| **Server Collation** | SQL_Latin1_General_CP1_CI_AS |
| **Minimum TLS Version** | 1.2 |
| **Region** | Canada Central |
| **Secondary Region** | Canada East |

## Database Configuration

| Property | Value |
|----------|-------|
| **Database Name** | RetailDb |
| **SKU / Tier** | Standard S1 (20 DTUs) |
| **Max Size** | 10 GB |
| **Compatibility Level** | 170 (SQL Server 2022) |
| **Collation** | SQL_Latin1_General_CP1_CI_AS |
| **Zone Redundant** | No |
| **Read Scale-Out** | Disabled |
| **Backup Redundancy** | Local |
| **Created** | 2026-03-23 11:46:32 UTC |

## Storage Allocation

| File Type | Size (MB) | Used (MB) | Free (MB) | % Used |
|-----------|-----------|-----------|-----------|--------|
| Data (ROWS) | 32 | ~2 | ~30 | ~6% |
| Log | 8 | — | — | — |
| **Total** | **40** | — | — | — |

## Database Settings

| Setting | Value |
|---------|-------|
| Auto Create Statistics | Enabled |
| Auto Update Statistics | Enabled |
| Auto Update Stats Async | Disabled |
| Auto Shrink | Disabled |
| Query Store | Enabled (READ_WRITE) |
| Read Committed Snapshot | Enabled |
| Delayed Durability | Disabled |
| MAXDOP | 8 |
| Optimize for Ad Hoc | Off |
| Parameter Sniffing | On |

## Query Store Configuration

| Setting | Value |
|---------|-------|
| State | READ_WRITE (Active) |
| Current Storage | 1 MB |
| Max Storage | 100 MB |
| Flush Interval | 900 seconds |
| Statistics Interval | 60 minutes |
| Stale Query Threshold | 30 days |
| Capture Mode | AUTO |
| Size-Based Cleanup | AUTO |

## Table Inventory

| Schema | Table | Row Count | Total Space (MB) | Used Space (MB) |
|--------|-------|-----------|-------------------|-----------------|
| dbo | Inventories | 130 | 0.07 | 0.02 |
| dbo | OrderItems | 37 | 0.07 | 0.02 |
| dbo | Products | 26 | 0.07 | 0.02 |
| dbo | Orders | 21 | 0.07 | 0.02 |
| dbo | Categories | 16 | 0.07 | 0.02 |
| dbo | Customers | 15 | 0.07 | 0.02 |
| dbo | Employees | 12 | 0.07 | 0.02 |
| dbo | Reviews | 10 | 0.07 | 0.02 |
| dbo | Suppliers | 6 | 0.07 | 0.02 |
| dbo | Stores | 5 | 0.07 | 0.02 |
| dbo | __EFMigrationsHistory | 1 | 0.07 | 0.02 |
| | **Total** | **279** | **0.77** | **0.22** |

## Database Objects

| Object Type | Count |
|-------------|-------|
| User Tables | 11 |
| Primary Key Constraints | 11 |
| Foreign Key Constraints | 14 |
| Indexes (total) | 35 |
| Stored Procedures | 0 |
| Views | 0 |
| Functions | 0 |

## Authentication & Access

| Property | Value |
|----------|-------|
| Authentication Mode | Azure AD-only |
| SQL Authentication | Disabled |
| Public Network Access | Enabled |

## Backup Configuration

| Setting | Value |
|---------|-------|
| PITR Retention | 7 days |
| Diff Backup Interval | 24 hours |
| Long-Term Weekly | Not configured |
| Long-Term Monthly | Not configured |
| Long-Term Yearly | Not configured |
| Backup Storage Redundancy | Local |

## Security Features

| Feature | Status |
|---------|--------|
| TDE (Transparent Data Encryption) | Enabled |
| Auditing | Disabled |
| Advanced Threat Protection | Disabled |
| Threat Detection Alerts | Not configured |
