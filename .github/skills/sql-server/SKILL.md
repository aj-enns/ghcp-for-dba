# SQL Server DBA Skill

## T-SQL Patterns

### Query Style
- Use `SET NOCOUNT ON` in stored procedures to reduce network traffic
- Prefer `EXISTS` over `IN` for subqueries against large tables
- Use `TRY_CAST` / `TRY_CONVERT` instead of `CAST` / `CONVERT` for safer type conversions
- Avoid cursors — use set-based operations, `CROSS APPLY`, or window functions instead
- Use `MERGE` cautiously — prefer explicit INSERT/UPDATE/DELETE for clarity and debugging
- Always specify schema names (`dbo.TableName` not just `TableName`)

### Stored Procedure Best Practices
- Begin every procedure with `SET NOCOUNT ON; SET XACT_ABORT ON;`
- Use `BEGIN TRY / BEGIN CATCH` with `THROW` (not `RAISERROR`) for error handling
- Include `@@TRANCOUNT` checks in CATCH blocks before rollback
- Avoid `sp_` prefix — it causes SQL Server to check `master` database first
- Use `OPTION (RECOMPILE)` for queries with highly variable parameters, not as a default

### Temp Tables vs Table Variables
- Temp tables (`#temp`): better for large datasets, support statistics and indexes
- Table variables (`@table`): better for small datasets (<1000 rows), no recompilation
- Avoid temp tables in functions — use table variables instead

## Dynamic Management Views (DMVs)

### Essential DMVs for Performance
```sql
-- Top queries by CPU
SELECT TOP 20
    qs.total_worker_time / qs.execution_count AS avg_cpu_time,
    qs.execution_count,
    SUBSTRING(st.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset WHEN -1 THEN DATALENGTH(st.text)
          ELSE qs.statement_end_offset END - qs.statement_start_offset)/2)+1) AS query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) st
ORDER BY qs.total_worker_time DESC;

-- Current blocking chains
SELECT
    blocking.session_id AS blocking_session,
    blocked.session_id AS blocked_session,
    blocked.wait_type,
    blocked.wait_time,
    st.text AS blocking_query
FROM sys.dm_exec_requests blocked
JOIN sys.dm_exec_sessions blocking ON blocked.blocking_session_id = blocking.session_id
CROSS APPLY sys.dm_exec_sql_text(blocking.most_recent_sql_handle) st
WHERE blocked.blocking_session_id > 0;

-- Wait statistics (since last restart)
SELECT TOP 20
    wait_type,
    wait_time_ms / 1000.0 AS wait_time_sec,
    signal_wait_time_ms / 1000.0 AS signal_wait_sec,
    waiting_tasks_count
FROM sys.dm_os_wait_stats
WHERE wait_type NOT IN (
    'SLEEP_TASK', 'BROKER_TO_FLUSH', 'SQLTRACE_BUFFER_FLUSH',
    'CLR_AUTO_EVENT', 'CLR_MANUAL_EVENT', 'LAZYWRITER_SLEEP',
    'CHECKPOINT_QUEUE', 'WAITFOR', 'XE_TIMER_EVENT',
    'BROKER_EVENTHANDLER', 'FT_IFTS_SCHEDULER_IDLE_WAIT',
    'XE_DISPATCHER_WAIT', 'HADR_FILESTREAM_IOMGR_IOCOMPLETION'
)
ORDER BY wait_time_ms DESC;

-- Index usage statistics
SELECT
    OBJECT_NAME(i.object_id) AS table_name,
    i.name AS index_name,
    ius.user_seeks,
    ius.user_scans,
    ius.user_lookups,
    ius.user_updates
FROM sys.dm_db_index_usage_stats ius
JOIN sys.indexes i ON ius.object_id = i.object_id AND ius.index_id = i.index_id
WHERE ius.database_id = DB_ID()
ORDER BY ius.user_seeks + ius.user_scans + ius.user_lookups DESC;
```

## Configuration Best Practices

### Server Settings
- `max server memory` — set to total RAM minus OS overhead (4-8 GB for OS)
- `max degree of parallelism` — set to number of physical cores per NUMA node (typically 4-8)
- `cost threshold for parallelism` — raise from default 5 to 25-50
- `optimize for ad hoc workloads` — enable to reduce plan cache bloat
- Enable `instant file initialization` for faster data file growth

### TempDB Configuration
- Create one data file per CPU core (up to 8 files)
- Size all data files equally
- Pre-size to avoid auto-growth during operations
- Place on fast storage (SSD/NVMe)

### Database Settings
- Set `AUTO_CREATE_STATISTICS` and `AUTO_UPDATE_STATISTICS` to ON
- Set `READ_COMMITTED_SNAPSHOT` to ON for OLTP workloads (reduces blocking)
- Avoid `AUTO_SHRINK` — it causes fragmentation and performance issues
- Set appropriate `RECOVERY MODEL` (FULL for production, SIMPLE for dev/test)

## Maintenance Tasks

### Regular Maintenance Schedule
| Task | Frequency | Window |
|------|-----------|--------|
| Index rebuild (>30% frag) | Weekly | Off-hours |
| Index reorganize (10-30% frag) | Daily | Low-activity |
| Update statistics | Daily | After index maintenance |
| DBCC CHECKDB | Weekly | Off-hours |
| Backup (Full) | Daily | Off-hours |
| Backup (Transaction Log) | Every 15-30 min | Continuous |
| Cleanup old backups | Daily | Any time |

### Index Maintenance Anti-Patterns
- Don't rebuild indexes with < 10% fragmentation — it wastes I/O
- Don't rebuild and then update statistics — rebuild already updates stats
- Don't use `DBCC DBREINDEX` — it's deprecated; use `ALTER INDEX REBUILD`
- Don't shrink databases after rebuilding indexes — it re-fragments them
