# Monitoring Setup — RetailDb

**Report Date:** 2026-03-23  
**Database:** RetailDb @ sql-retail-lab-ug5lhmwwszwla.database.windows.net

---

## Key Metrics & Alert Thresholds

| Metric | Source | Warning Threshold | Critical Threshold | Check Frequency |
|--------|--------|-------------------|-------------------|-----------------|
| DTU Consumption % | Azure Monitor | > 80% avg (5 min) | > 95% avg (5 min) | Continuous |
| Storage Used % | Azure Monitor | > 80% | > 90% | Every 15 min |
| Deadlocks | Azure Monitor | > 0 / hour | > 5 / hour | Continuous |
| Failed Connections | Azure Monitor | > 10 / 5 min | > 50 / 5 min | Continuous |
| Long-Running Queries | Query Store | > 30 sec | > 120 sec | Every 5 min |
| Blocked Processes | DMV | > 0 (30s+) | > 0 (60s+) | Every 30 sec |
| Workers Used % | Azure Monitor | > 70% | > 90% | Continuous |
| Sessions Used % | Azure Monitor | > 70% | > 90% | Continuous |
| Index Fragmentation | DMV | > 30% | > 70% | Weekly |
| Statistics Staleness | DMV | > 7 days | > 30 days | Daily |
| Query Store Space % | DMV | > 80% | > 95% | Daily |
| Backup Age (PITR) | Azure CLI | — | > 24h without diff | Daily |

---

## Recommended Monitoring Tools

### 1. Azure Monitor Alerts (Built-in)

Set up metric alerts in the Azure Portal for the RetailDb resource:

```bash
# Example: DTU consumption > 80% for 5 minutes
az monitor metrics alert create \
  --name "RetailDb-HighDTU" \
  --resource-group rg-retail-lab \
  --scopes "/subscriptions/{sub-id}/resourceGroups/rg-retail-lab/providers/Microsoft.Sql/servers/sql-retail-lab-ug5lhmwwszwla/databases/RetailDb" \
  --condition "avg dtu_consumption_percent > 80" \
  --window-size 5m \
  --evaluation-frequency 1m \
  --action-group "{action-group-id}" \
  --description "DTU consumption exceeds 80%"
```

### 2. Azure SQL Analytics (Log Analytics)

Enable diagnostics to a Log Analytics workspace for deeper analysis:

```bash
az monitor diagnostic-settings create \
  --name "RetailDb-Diagnostics" \
  --resource "/subscriptions/{sub-id}/resourceGroups/rg-retail-lab/providers/Microsoft.Sql/servers/sql-retail-lab-ug5lhmwwszwla/databases/RetailDb" \
  --workspace "{log-analytics-workspace-id}" \
  --logs '[{"category":"SQLInsights","enabled":true},{"category":"QueryStoreRuntimeStatistics","enabled":true},{"category":"QueryStoreWaitStatistics","enabled":true},{"category":"Errors","enabled":true},{"category":"Deadlocks","enabled":true},{"category":"Timeouts","enabled":true},{"category":"Blocks","enabled":true}]' \
  --metrics '[{"category":"Basic","enabled":true}]'
```

### 3. Query Store Monitoring Queries

Save and schedule these queries for regular review:

```sql
-- Top 10 resource-consuming queries (last 24 hours)
SELECT TOP 10
    qt.query_sql_text,
    rs.count_executions,
    rs.avg_duration / 1000.0 AS avg_duration_ms,
    rs.avg_cpu_time / 1000.0 AS avg_cpu_ms,
    rs.avg_logical_io_reads,
    rs.avg_logical_io_writes,
    rs.last_execution_time
FROM sys.query_store_query_text qt
JOIN sys.query_store_query q ON qt.query_text_id = q.query_text_id
JOIN sys.query_store_plan p ON q.query_id = p.query_id
JOIN sys.query_store_runtime_stats rs ON p.plan_id = rs.plan_id
JOIN sys.query_store_runtime_stats_interval rsi ON rs.runtime_stats_interval_id = rsi.runtime_stats_interval_id
WHERE rsi.start_time >= DATEADD(HOUR, -24, GETUTCDATE())
ORDER BY rs.avg_cpu_time DESC;
```

```sql
-- Regressed queries (performance degraded vs. baseline)
SELECT 
    qt.query_sql_text,
    q.query_id,
    rs_recent.avg_duration / 1000.0 AS recent_avg_ms,
    rs_baseline.avg_duration / 1000.0 AS baseline_avg_ms,
    CAST((rs_recent.avg_duration - rs_baseline.avg_duration) * 100.0 / NULLIF(rs_baseline.avg_duration, 0) AS DECIMAL(5,1)) AS pct_regression
FROM sys.query_store_query_text qt
JOIN sys.query_store_query q ON qt.query_text_id = q.query_text_id
JOIN sys.query_store_plan p ON q.query_id = p.query_id
JOIN sys.query_store_runtime_stats rs_recent ON p.plan_id = rs_recent.plan_id
JOIN sys.query_store_runtime_stats rs_baseline ON p.plan_id = rs_baseline.plan_id
JOIN sys.query_store_runtime_stats_interval rsi_recent ON rs_recent.runtime_stats_interval_id = rsi_recent.runtime_stats_interval_id
JOIN sys.query_store_runtime_stats_interval rsi_baseline ON rs_baseline.runtime_stats_interval_id = rsi_baseline.runtime_stats_interval_id
WHERE rsi_recent.start_time >= DATEADD(HOUR, -1, GETUTCDATE())
  AND rsi_baseline.start_time >= DATEADD(DAY, -7, GETUTCDATE())
  AND rsi_baseline.start_time < DATEADD(DAY, -1, GETUTCDATE())
  AND rs_recent.avg_duration > rs_baseline.avg_duration * 1.5
ORDER BY pct_regression DESC;
```

```sql
-- Wait statistics from Query Store
SELECT TOP 10
    ws.wait_category_desc,
    SUM(ws.total_query_wait_time_ms) AS total_wait_ms,
    SUM(ws.avg_query_wait_time_ms * ws.total_query_wait_time_ms) / 
        NULLIF(SUM(ws.total_query_wait_time_ms), 0) AS weighted_avg_wait_ms
FROM sys.query_store_wait_stats ws
JOIN sys.query_store_runtime_stats_interval rsi ON ws.runtime_stats_interval_id = rsi.runtime_stats_interval_id
WHERE rsi.start_time >= DATEADD(HOUR, -24, GETUTCDATE())
GROUP BY ws.wait_category_desc
ORDER BY total_wait_ms DESC;
```

---

## Weekly DBA Checklist

- [ ] Review DTU/CPU utilization trends (Azure Portal > Metrics)
- [ ] Check Query Store for new regressed queries
- [ ] Review top wait statistics for emerging patterns
- [ ] Verify latest backup via PITR restore point availability
- [ ] Check for any Azure Service Health advisories
- [ ] Review deadlock and blocking alerts
- [ ] Verify Query Store is still in READ_WRITE state
- [ ] Check storage consumption trend

## Monthly DBA Checklist

- [ ] Review and update index fragmentation (rebuild where >30%)
- [ ] Verify statistics are up to date on all tables
- [ ] Review security: unused logins, permission changes
- [ ] Audit database users and role memberships
- [ ] Review slow query trends over the past month
- [ ] Validate backup retention and test a restore
- [ ] Review Azure Advisor recommendations
- [ ] Check for available Azure SQL maintenance windows
- [ ] Review DTU utilization — consider scaling if consistently >70%
- [ ] Export and archive audit logs (if auditing enabled)

## Quarterly DBA Checklist

- [ ] Full security audit (access review, firewall rules)
- [ ] Disaster recovery test (restore to secondary region)
- [ ] Review and optimize high-cost queries
- [ ] Evaluate SKU sizing — right-size if over/under-provisioned
- [ ] Update documentation and runbooks
- [ ] Review compliance posture (CIS benchmarks, etc.)

---

## Automated Health Check Script

Run this T-SQL script on demand or schedule via Azure Automation:

```sql
-- ============================================
-- Quick Health Check Script for RetailDb
-- Run on demand to get a snapshot of DB health
-- ============================================

PRINT '=== DATABASE SIZE ===';
SELECT 
    DB_NAME() AS DatabaseName,
    SUM(CASE WHEN type_desc = 'ROWS' THEN size * 8.0 / 1024 END) AS DataMB,
    SUM(CASE WHEN type_desc = 'LOG' THEN size * 8.0 / 1024 END) AS LogMB
FROM sys.database_files;

PRINT '=== TABLE ROW COUNTS ===';
SELECT 
    t.name AS TableName,
    p.rows AS [RowCount]
FROM sys.tables t
JOIN sys.partitions p ON t.object_id = p.object_id AND p.index_id IN (0, 1)
WHERE t.is_ms_shipped = 0
ORDER BY p.rows DESC;

PRINT '=== INDEX FRAGMENTATION (>10%) ===';
SELECT 
    OBJECT_NAME(ips.object_id) AS TableName,
    i.name AS IndexName,
    ips.avg_fragmentation_in_percent AS FragPct,
    ips.page_count AS Pages
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.avg_fragmentation_in_percent > 10 AND ips.page_count > 100;

PRINT '=== STALE STATISTICS (>7 DAYS) ===';
SELECT 
    OBJECT_NAME(st.object_id) AS TableName,
    st.name AS StatName,
    STATS_DATE(st.object_id, st.stats_id) AS LastUpdated,
    DATEDIFF(DAY, STATS_DATE(st.object_id, st.stats_id), GETDATE()) AS DaysStale
FROM sys.stats st
JOIN sys.tables t ON st.object_id = t.object_id
WHERE t.is_ms_shipped = 0
  AND DATEDIFF(DAY, STATS_DATE(st.object_id, st.stats_id), GETDATE()) > 7;

PRINT '=== TOP 5 WAITS ===';
SELECT TOP 5 
    wait_type, 
    wait_time_ms, 
    waiting_tasks_count
FROM sys.dm_os_wait_stats
WHERE wait_type NOT LIKE '%SLEEP%' 
  AND wait_type NOT LIKE '%QUEUE%'
  AND wait_type NOT LIKE '%DISPATCHER%'
  AND waiting_tasks_count > 0
ORDER BY wait_time_ms DESC;

PRINT '=== QUERY STORE STATUS ===';
SELECT 
    actual_state_desc AS QSState,
    current_storage_size_mb AS CurrentMB,
    max_storage_size_mb AS MaxMB,
    CAST(current_storage_size_mb * 100.0 / NULLIF(max_storage_size_mb, 0) AS DECIMAL(5,1)) AS PctUsed
FROM sys.database_query_store_options;

PRINT '=== HEALTH CHECK COMPLETE ===';
```
