# Monitoring & Alerting Skill

## Key Metrics to Monitor

### Critical Metrics (Alert Immediately)
| Metric | Threshold | Why |
|--------|-----------|-----|
| Disk space free | < 10% | Database stops accepting writes |
| CPU sustained | > 90% for 5 min | Query degradation, possible runaway query |
| Blocking chain duration | > 30 seconds | Application timeouts |
| Replication lag | > 60 seconds | Data staleness on replicas |
| Failed logins | > 10 in 5 min | Possible brute force attack |
| Backup age | > 25 hours (daily) | Data loss risk |
| DBCC CHECKDB age | > 8 days | Undetected corruption risk |

### Warning Metrics (Investigate Soon)
| Metric | Threshold | Why |
|--------|-----------|-----|
| Page Life Expectancy | < 300 seconds | Memory pressure |
| Buffer cache hit ratio | < 95% | Insufficient memory or poor queries |
| Long-running queries | > 5 minutes | Potential performance issue |
| TempDB usage | > 80% | Risk of tempdb full |
| Connection count | > 80% of max | Connection exhaustion risk |
| Index fragmentation | > 30% | Query performance degradation |
| Transaction log usage | > 80% | Risk of log full |

## SQL Server Monitoring Queries

### Real-Time Dashboard Queries
```sql
-- Server health snapshot
SELECT
    (SELECT cntr_value FROM sys.dm_os_performance_counters
     WHERE counter_name = 'Page life expectancy'
       AND object_name LIKE '%Buffer Manager%') AS PLE,
    (SELECT cntr_value FROM sys.dm_os_performance_counters
     WHERE counter_name = 'Batch Requests/sec'
       AND object_name LIKE '%SQL Statistics%') AS batch_requests_sec,
    (SELECT count(*) FROM sys.dm_exec_requests
     WHERE blocking_session_id > 0) AS blocked_sessions,
    (SELECT count(*) FROM sys.dm_exec_sessions
     WHERE is_user_process = 1 AND status = 'running') AS active_queries;

-- Database sizes and growth
SELECT
    db.name,
    mf.type_desc,
    CAST(mf.size * 8.0 / 1024 AS DECIMAL(10,2)) AS size_mb,
    CAST(mf.size * 8.0 / 1024 - CAST(FILEPROPERTY(mf.name, 'SpaceUsed') AS INT) * 8.0 / 1024
         AS DECIMAL(10,2)) AS free_mb,
    mf.growth,
    mf.is_percent_growth
FROM sys.master_files mf
JOIN sys.databases db ON mf.database_id = db.database_id
ORDER BY db.name, mf.type_desc;

-- Recent backup status
SELECT
    d.name AS database_name,
    MAX(CASE WHEN b.type = 'D' THEN b.backup_finish_date END) AS last_full,
    MAX(CASE WHEN b.type = 'I' THEN b.backup_finish_date END) AS last_diff,
    MAX(CASE WHEN b.type = 'L' THEN b.backup_finish_date END) AS last_log
FROM sys.databases d
LEFT JOIN msdb.dbo.backupset b ON d.name = b.database_name
WHERE d.database_id > 4  -- exclude system databases
GROUP BY d.name
ORDER BY d.name;
```

## PostgreSQL Monitoring Queries

```sql
-- Connection status overview
SELECT state, count(*) FROM pg_stat_activity GROUP BY state;

-- Database sizes
SELECT datname,
       pg_size_pretty(pg_database_size(datname)) AS size
FROM pg_database
WHERE datistemplate = false
ORDER BY pg_database_size(datname) DESC;

-- Long-running queries (> 5 minutes)
SELECT pid, now() - query_start AS duration,
       state, query
FROM pg_stat_activity
WHERE state != 'idle'
  AND now() - query_start > interval '5 minutes'
  AND pid != pg_backend_pid();

-- Table statistics (needs vacuum?)
SELECT schemaname, relname,
       n_live_tup, n_dead_tup,
       last_vacuum, last_autovacuum,
       last_analyze, last_autoanalyze
FROM pg_stat_user_tables
WHERE n_dead_tup > 10000
ORDER BY n_dead_tup DESC;
```

## Wait Statistics Analysis

### SQL Server Top Wait Categories
| Wait Category | Common Waits | Typical Cause | Action |
|:-------------:|-------------|---------------|--------|
| **I/O** | `PAGEIOLATCH_SH`, `PAGEIOLATCH_EX` | Missing indexes, insufficient memory | Add indexes, increase RAM |
| **CPU** | `SOS_SCHEDULER_YIELD`, `CXPACKET` | Expensive queries, parallelism overhead | Tune queries, adjust MAXDOP |
| **Locking** | `LCK_M_S`, `LCK_M_X`, `LCK_M_IX` | Lock contention, long transactions | Optimize transactions, add indexes |
| **Memory** | `RESOURCE_SEMAPHORE` | Memory grants waiting | Reduce sort/hash operations |
| **Network** | `ASYNC_NETWORK_IO` | Client consuming results slowly | Optimize result set size |
| **TempDB** | `PAGELATCH_UP` on 2:1:1 | TempDB contention | Add TempDB data files |

## Alerting Best Practices

### Alert Severity Levels
| Level | Response Time | Examples | Notification |
|:-----:|:------------:|---------|--------------|
| P1 — Critical | < 15 min | Database down, data corruption, storage full | Page on-call DBA |
| P2 — High | < 1 hour | Replication broken, backup failure, blocking > 5 min | Alert DBA team |
| P3 — Warning | Next business day | High fragmentation, PLE dropped, slow query trend | Email/Slack |
| P4 — Info | Weekly review | Growth trends, unused indexes, statistics freshness | Dashboard |

### Alert Anti-Patterns
- ❌ Alerting on every spike — use sustained thresholds (e.g., "CPU > 90% for 5 minutes")
- ❌ Too many P1 alerts — alert fatigue causes real issues to be missed
- ❌ No escalation path — alerts must have a clear owner and escalation
- ❌ Missing "all clear" notifications — alert on recovery too
- ❌ Alerting without context — include current value, threshold, and recent trend
