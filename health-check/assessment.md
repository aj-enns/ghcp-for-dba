# Database Health Assessment — RetailDb

**Report Date:** 2026-03-23  
**Server:** sql-retail-lab-ug5lhmwwszwla.database.windows.net  
**Database:** RetailDb (Standard S1, 20 DTUs)

---

## Dashboard Summary

| Category | Status | Score | Key Finding |
|----------|--------|-------|-------------|
| Storage & Files | 🟢 Healthy | 9/10 | Data file 78% utilized; 16 MB auto-growth is appropriate |
| Performance | 🟢 Healthy | 8/10 | No user-impacting waits; low query volume |
| Indexes | 🟢 Healthy | 9/10 | 0% fragmentation; all FK columns indexed |
| Query Store | 🟢 Healthy | 9/10 | Active, 1% storage used, AUTO capture |
| Plan Cache | 🟡 Warning | 7/10 | OPTIMIZE_FOR_AD_HOC_WORKLOADS is OFF; 487 ad hoc plans using 101 MB |
| Configuration | 🟡 Warning | 7/10 | MAXDOP=8 may be too high for S1 tier (limited CPU) |
| Integrity | 🟢 Healthy | 10/10 | Azure-managed CHECKDB; TDE enabled (AES-256) |
| Security | 🟡 Warning | 6/10 | Single db_owner user; no role separation; no auditing visible |

**Overall Score: 8.1 / 10**

---

## Storage & Files

### Data File (data_0)
- **Size:** 32 MB | **Used:** 25.06 MB (78.3%) | **Free:** 6.94 MB
- **Auto-Growth:** 16 MB increments (appropriate for this tier)
- **Max Size:** 10 GB (database limit for Standard S1)

### Log File (log)
- **Size:** 8 MB | **Used:** 1.16 MB (14.5%) | **Free:** 6.84 MB
- **Auto-Growth:** 16 MB increments
- **VLF Count:** Low (small log file)

### I/O Latency

| File | Reads | Writes | Avg Read Latency | Avg Write Latency |
|------|-------|--------|------------------|-------------------|
| data_0 | 132 | 819 | 16.23 ms | 8.42 ms |
| log | 30 | 746 | 18.73 ms | 9.47 ms |

> Latency is within acceptable range for Azure SQL S1 tier. Read latency is slightly elevated but expected given the DTU constraints.

### TempDB
- Azure SQL Database manages TempDB automatically. Not user-configurable.

---

## Performance

### Wait Statistics (Top 10)

| Wait Type | Time (ms) | % | Analysis |
|-----------|-----------|---|----------|
| SOS_WORK_DISPATCHER | 969,568,835 | 82.25% | Azure infrastructure — benign |
| HADR_WORK_QUEUE | 77,915,205 | 6.61% | Azure HA replication — benign |
| PREEMPTIVE_XE_DISPATCHER | 41,286,515 | 3.50% | Extended Events — benign |
| QDS_PERSIST_TASK_MAIN_LOOP_SLEEP | 19,502,618 | 1.65% | Query Store flush — benign |
| SP_SERVER_DIAGNOSTICS_SLEEP | 19,500,482 | 1.65% | Health monitoring — benign |
| PWAIT_EXTENSIBILITY_CLEANUP_TASK | 19,500,454 | 1.65% | Cleanup — benign |
| HADR_FABRIC_CALLBACK | 18,008,393 | 1.53% | Azure HA — benign |
| HADR_TIMER_TASK | 6,306,810 | 0.54% | Azure HA timer — benign |
| QDS_ASYNC_QUEUE | 5,239,849 | 0.44% | Query Store — benign |
| PVS_PREALLOCATE | 1,786,960 | 0.15% | Persistent Version Store — benign |

> **Assessment:** All top waits are Azure infrastructure waits. No user-impacting waits (PAGEIOLATCH, LCK_*, CXPACKET, etc.) appear in the top list. This is a healthy wait profile.

### Most Expensive Queries (by CPU)

All top-CPU queries are system metadata queries from tooling (SSMS/VS Code MSSQL extension):
1. `sys.all_views` enumeration — 0.15s CPU, 101K reads
2. Index metadata query — 0.14s CPU, 17K reads
3. Functions metadata query — 0.13s CPU, 34K reads

> **Assessment:** No user-application queries are appearing as expensive. The database currently has very low workload.

### Index Fragmentation

All user table indexes report **0% fragmentation** with 1–2 pages each. This is expected for a freshly seeded database with low data volume.

| Table | Index | Type | Fragmentation | Pages |
|-------|-------|------|--------------|-------|
| Orders | PK_Orders | CLUSTERED | 0% | 1 |
| Inventories | PK_Inventories | CLUSTERED | 0% | 1 |
| Products | PK_Products | CLUSTERED | 0% | 1 |
| Categories | PK_Categories | CLUSTERED | 0% | 1 |
| *(all others)* | *(various)* | *(both)* | 0% | 1 |

### Missing Indexes
- **None reported.** The query optimizer has not identified any missing indexes.

### Statistics Freshness
- All statistics were updated today (2026-03-23) during initial seeding
- Customers table has 15 modifications on auto-created stat `_WA_Sys_0000000A` — will auto-update on next relevant query
- All index statistics are current with 0 modification counters

### Plan Cache Efficiency

| Plan Type | Count | Size (MB) | Total Uses | Avg Uses |
|-----------|-------|-----------|------------|----------|
| Adhoc | 487 | 101.52 | 6,415 | 13.17 |
| View | 352 | 67.58 | 1,995 | 5.67 |
| Prepared | 180 | 37.30 | 1,000 | 5.56 |
| Proc | 35 | 4.48 | 771 | 22.03 |
| Trigger | 1 | 0.08 | 1 | 1.00 |

> **Finding:** `OPTIMIZE_FOR_AD_HOC_WORKLOADS` is OFF. Ad hoc plans represent 487 entries consuming 101 MB. Enabling this setting would stub single-use plans to save plan cache memory.

---

## Integrity

| Check | Status | Details |
|-------|--------|---------|
| DBCC CHECKDB | 🟢 Azure-managed | Automated by platform |
| TDE | 🟢 Encrypted | AES-256 |
| Backup Chain | 🟢 Azure-managed | Automated full/diff/log backups |
| Corruption | 🟢 None detected | No errors reported |

---

## Security

### Database Principals

| Principal | Type | Auth | Roles |
|-----------|------|------|-------|
| dbo | SQL_USER | INSTANCE | db_owner |

### Findings

| Finding | Severity | Detail |
|---------|----------|--------|
| Single db_owner user | 🟡 Warning | Only `dbo` exists. No application-specific users with least-privilege roles. |
| No role separation | 🟡 Warning | No custom database roles for application vs. admin access. |
| No stored procedures | 🟡 Info | Application appears to use ad hoc queries rather than stored procedures, reducing ability to use EXECUTE permissions. |
| Azure AD-only auth | 🟢 Good | SQL authentication is disabled at the server level. |
| TDE enabled | 🟢 Good | Data at rest is encrypted with AES-256. |
| Auditing | ⚪ Unknown | Database-level audit status cannot be checked via DMVs on Azure SQL. Verify via Azure Portal > SQL Server > Auditing. |

### Tables Without Primary Keys
- **None** — All 11 user tables have primary keys.

### Foreign Keys Without Indexes
- **None** — All 14 foreign key columns have supporting indexes.
