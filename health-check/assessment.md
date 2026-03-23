# Database Health Assessment — RetailDb

**Report Date:** 2026-03-23  
**Server:** sql-retail-lab-ug5lhmwwszwla.database.windows.net  
**Database:** RetailDb (Azure SQL Database, Standard S1)

---

## Executive Dashboard

| Category | Status | Score | Key Finding |
|----------|--------|-------|-------------|
| Storage | :green_circle: Healthy | 9/10 | Minimal data (<1 MB used of 10 GB), ample headroom |
| Performance | :green_circle: Healthy | 8/10 | No missing indexes, Query Store active, low query load |
| Integrity | :green_circle: Healthy | 9/10 | All tables have PKs, all FKs indexed, schema well-structured |
| Backup & Recovery | :yellow_circle: Warning | 6/10 | Only 7-day PITR, no long-term retention configured |
| Security | :red_circle: Critical | 4/10 | Auditing disabled, no threat detection, no LTR backups |
| Configuration | :green_circle: Healthy | 8/10 | Good defaults, Query Store active, RCSI enabled |

**Overall Health Score: 7.3 / 10**

---

## Detailed Findings

### Storage & Files

**Status: :green_circle: Healthy (9/10)**

| Check | Result | Notes |
|-------|--------|-------|
| Data file size | 32 MB allocated | Well within 10 GB max |
| Data space used | ~2 MB (6%) | Very light utilization |
| Log file size | 8 MB | Appropriate for workload |
| Max database size | 10 GB | Adequate for lab/dev environment |
| Auto-growth | Azure-managed | Azure SQL manages growth transparently |
| TempDB | Azure-managed | Not configurable in Azure SQL PaaS |

- :white_check_mark: Storage utilization is extremely low — the database is using <1% of its 10 GB allocation
- :white_check_mark: Auto-shrink is disabled (correct — auto-shrink causes fragmentation)
- :information_source: Azure SQL manages file growth and TempDB automatically

### Performance

**Status: :green_circle: Healthy (8/10)**

#### Wait Statistics

| Wait Type | Count | Total Wait (s) | Max Wait (s) |
|-----------|-------|----------------|--------------|
| SOS_WORK_DISPATCHER | 14,540 | 70,521 | 599 |
| HADR_WORK_QUEUE | 700 | 6,685 | 10 |
| PREEMPTIVE_XE_DISPATCHER | 36 | 3,052 | 289 |
| XE_LIVE_TARGET_TVF | 64 | 2,667 | 60 |

- :white_check_mark: No concerning user-impacting waits (PAGEIOLATCH, LCK, CXPACKET, etc.)
- :white_check_mark: All top waits are system/background waits, normal for Azure SQL

#### Top Queries by CPU

| Finding | Detail |
|---------|--------|
| Most expensive queries | System metadata queries (from SSMS/tooling connections) |
| User workload queries | None captured — database is freshly seeded |

- :white_check_mark: No problematic user queries detected
- :information_source: The database was just deployed; Query Store will accumulate data over time

#### Index Health

| Check | Result |
|-------|--------|
| Total indexes | 35 across 11 tables |
| Fragmentation | 0% on all indexes |
| Missing index suggestions | None |
| Unused indexes | Not enough workload data to determine |

- :white_check_mark: All 35 indexes show 0% fragmentation (freshly created)
- :white_check_mark: No missing index recommendations from the optimizer
- :white_check_mark: All foreign keys have supporting indexes (14/14 covered)

#### Statistics

| Check | Result |
|-------|--------|
| Total statistics objects | 36 |
| Auto-created stats | Some (by query optimizer) |
| Stats last updated | 2026-03-23 (today, at seed time) |
| Stats with NULL update date | Some nonclustered indexes (no queries yet) |

- :white_check_mark: Auto create statistics: ON
- :white_check_mark: Auto update statistics: ON
- :yellow_circle: Several index statistics show NULL last-updated date — these will populate as queries execute

#### Query Store

- :white_check_mark: State: READ_WRITE (actively collecting)
- :white_check_mark: Capture mode: AUTO (captures relevant queries)
- :white_check_mark: Current storage: 1 MB of 100 MB max
- :white_check_mark: Size-based cleanup: AUTO

### Integrity

**Status: :green_circle: Healthy (9/10)**

| Check | Result |
|-------|--------|
| Tables with primary keys | 11/11 (100%) |
| Foreign keys with indexes | 14/14 (100%) |
| DBCC CHECKDB | Not applicable — Azure SQL manages automatically |
| Corruption errors | None detected |
| Schema design | Well-normalized, appropriate data types |
| Unique constraints | 3 (Customer.Email, Employee.Email, Product.Sku) |
| Check constraints | 0 |

- :white_check_mark: Every table has a clustered primary key
- :white_check_mark: All 14 foreign keys have proper supporting indexes
- :white_check_mark: Azure SQL automatically runs integrity checks
- :white_check_mark: Appropriate data types (decimal for currency, nvarchar with proper max lengths)
- :yellow_circle: No CHECK constraints defined (e.g., Rating 1-5 range, positive quantities)
- :yellow_circle: No stored procedures, views, or functions — all logic is in the application layer

### Backup & Recovery

**Status: :yellow_circle: Warning (6/10)**

| Setting | Current | Recommended |
|---------|---------|-------------|
| Point-in-Time Restore | 7 days | 14-35 days for production |
| Diff backup interval | 24 hours | 12 hours for faster RPO |
| LTR Weekly | Not configured | W=P4W (4 weeks) |
| LTR Monthly | Not configured | M=P12M (12 months) |
| LTR Yearly | Not configured | Y=P3Y (3 years) |
| Backup redundancy | Local | Geo for production |

- :red_circle: No long-term retention configured — unable to restore beyond 7 days
- :yellow_circle: 7-day PITR retention is minimum default; should be increased for production
- :yellow_circle: Local backup redundancy only — no geo-protection for disaster recovery
- :white_check_mark: Azure SQL performs automatic backups (full weekly, diff every 12-24h, log every 5-10 min)

### Security

**Status: :red_circle: Critical (4/10)**

| Check | Result | Risk |
|-------|--------|------|
| Authentication | Azure AD-only | :white_check_mark: Best practice |
| TDE | Enabled | :white_check_mark: Data encrypted at rest |
| TLS 1.2 minimum | Enabled | :white_check_mark: Encrypted in transit |
| Auditing | **Disabled** | :red_circle: No audit trail |
| Threat Detection | **Disabled** | :red_circle: No SQL injection/anomaly detection |
| Alert Email | Not configured | :red_circle: No one notified of security events |
| Public network | Enabled | :yellow_circle: Consider private endpoint for production |
| Orphaned users | None | :white_check_mark: Clean |
| Database users | dbo only (AD admin) | :yellow_circle: No least-privilege roles configured |
| Firewall rules | AllowAzureServices (0.0.0.0) | :yellow_circle: Overly broad for production |

- :red_circle: **Auditing is disabled** — no record of who accessed/modified data
- :red_circle: **Advanced Threat Protection is off** — no SQL injection or brute-force detection
- :yellow_circle: No application-specific database roles (only dbo/owner access)
- :yellow_circle: "Allow Azure Services" firewall rule is convenient but overly permissive
- :white_check_mark: Azure AD-only authentication (no SQL passwords to manage)
- :white_check_mark: TDE enabled (platform default for Azure SQL)
