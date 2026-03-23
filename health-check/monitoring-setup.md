# Monitoring Setup — RetailDb

**Report Date:** 2026-03-23  
**Database:** RetailDb (Standard S1, 20 DTUs)

---

## Key Metrics to Track

| Metric | Threshold (Warning) | Threshold (Critical) | Check Frequency |
|--------|---------------------|----------------------|-----------------|
| DTU % Utilization | > 70% sustained | > 90% sustained | Every 5 min |
| Data Space Used % | > 80% | > 90% | Daily |
| Log Space Used % | > 70% | > 85% | Every 15 min |
| Deadlocks | > 1/hour | > 5/hour | Real-time |
| Blocked Sessions | > 5 sec duration | > 30 sec duration | Every 1 min |
| Failed Connections | > 10/hour | > 50/hour | Every 5 min |
| Long-Running Queries | > 30 sec | > 120 sec | Every 1 min |
| Index Fragmentation | > 30% (pages > 1000) | > 50% | Weekly |
| Query Store Space Used | > 70% | > 90% | Daily |
| Statistics Modification Counter | > 20% of rows | > 50% of rows | Daily |

---

## Azure Monitor Alerts (Recommended)

Configure these via Azure Portal > SQL Database > Alerts:

1. **DTU Percentage** — Alert when avg > 80% over 15 minutes
2. **Storage Percentage** — Alert when > 85%
3. **Deadlocks** — Alert on any occurrence
4. **Failed Connections** — Alert when count > 10 in 5 minutes
5. **Workers Percentage** — Alert when > 80%
6. **Sessions Percentage** — Alert when > 80%

---

## Recommended Monitoring Queries

### Active Sessions & Blocking

```sql
-- Check for blocked sessions
SELECT
    r.session_id,
    r.blocking_session_id,
    r.wait_type,
    r.wait_time,
    r.status,
    SUBSTRING(t.text, (r.statement_start_offset/2)+1,
        ((CASE r.statement_end_offset
            WHEN -1 THEN DATALENGTH(t.text)
            ELSE r.statement_end_offset
        END - r.statement_start_offset)/2)+1) AS CurrentQuery,
    r.cpu_time,
    r.total_elapsed_time
FROM sys.dm_exec_requests r
CROSS APPLY sys.dm_exec_sql_text(r.sql_handle) t
WHERE r.session_id > 50
ORDER BY r.total_elapsed_time DESC;
```

### DTU Consumption (Last Hour)

```sql
SELECT TOP 60
    end_time,
    avg_cpu_percent,
    avg_data_io_percent,
    avg_log_write_percent,
    CAST((avg_cpu_percent + avg_data_io_percent + avg_log_write_percent) / 3.0 AS DECIMAL(5,2)) AS approx_dtu_pct
FROM sys.dm_db_resource_stats
ORDER BY end_time DESC;
```

### Index Fragmentation Check

```sql
SELECT
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.avg_fragmentation_in_percent,
    ips.page_count
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10
  AND ips.page_count > 100
ORDER BY ips.avg_fragmentation_in_percent DESC;
```

### Stale Statistics Detection

```sql
SELECT
    OBJECT_NAME(s.object_id) AS TableName,
    s.name AS StatName,
    STATS_DATE(s.object_id, s.stats_id) AS LastUpdated,
    sp.rows,
    sp.modification_counter,
    CAST(sp.modification_counter * 100.0 / NULLIF(sp.rows, 0) AS DECIMAL(5,2)) AS ModifiedPct
FROM sys.stats s
CROSS APPLY sys.dm_db_stats_properties(s.object_id, s.stats_id) sp
WHERE OBJECTPROPERTY(s.object_id, 'IsUserTable') = 1
  AND sp.modification_counter > 0
ORDER BY ModifiedPct DESC;
```

### Query Store — Top Resource Consumers (Last 24h)

```sql
SELECT TOP 10
    q.query_id,
    qt.query_sql_text,
    rs.count_executions,
    CAST(rs.avg_cpu_time / 1000.0 AS DECIMAL(10,2)) AS avg_cpu_ms,
    CAST(rs.avg_duration / 1000.0 AS DECIMAL(10,2)) AS avg_duration_ms,
    rs.avg_logical_io_reads,
    rs.avg_logical_io_writes
FROM sys.query_store_query q
JOIN sys.query_store_query_text qt ON q.query_text_id = qt.query_text_id
JOIN sys.query_store_plan p ON q.query_id = p.query_id
JOIN sys.query_store_runtime_stats rs ON p.plan_id = rs.plan_id
JOIN sys.query_store_runtime_stats_interval rsi ON rs.runtime_stats_interval_id = rsi.runtime_stats_interval_id
WHERE rsi.start_time > DATEADD(HOUR, -24, GETUTCDATE())
ORDER BY rs.avg_cpu_time * rs.count_executions DESC;
```

---

## Weekly DBA Checklist

- [ ] Review DTU utilization trends in Azure Portal
- [ ] Check Query Store for regressed queries
- [ ] Review index fragmentation levels
- [ ] Verify statistics freshness (modification counters)
- [ ] Check for any failed logins or security alerts
- [ ] Review storage growth trend
- [ ] Validate Azure SQL backup retention is appropriate
- [ ] Check for any Azure Service Health advisories

## Monthly DBA Checklist

- [ ] Review and tune top 10 resource-consuming queries
- [ ] Assess whether current S1 tier is appropriate for workload
- [ ] Review database user permissions and role assignments
- [ ] Check for unused indexes (zero seeks/scans since last review)
- [ ] Review Query Store space usage and cleanup settings
- [ ] Validate that auditing is active and logs are being retained
- [ ] Review any schema changes and their performance impact
- [ ] Update system inventory documentation if changes occurred

---

## Recommended Tools

| Tool | Purpose | Notes |
|------|---------|-------|
| **Azure Monitor** | DTU, storage, connection alerts | Built-in, configure alert rules |
| **Query Store** | Query performance regression detection | Already enabled (READ_WRITE) |
| **Azure SQL Intelligent Insights** | AI-driven performance diagnostics | Available for Standard tier |
| **VS Code MSSQL Extension** | Ad hoc query execution, object browsing | Already configured (RetailLab profile) |
| **Azure SQL Analytics (Log Analytics)** | Centralized monitoring dashboard | Requires Log Analytics workspace |
| **Query Performance Insight** | Visual top queries in Azure Portal | Built-in for Azure SQL Database |
