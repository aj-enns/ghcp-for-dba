# MySQL / MariaDB DBA Skill

## SQL Patterns

### Query Style
- Use `EXPLAIN ANALYZE` (MySQL 8.0.18+) for actual execution metrics
- Prefer `JOIN` syntax over comma-separated tables with WHERE conditions
- Use `INSERT ... ON DUPLICATE KEY UPDATE` for upsert patterns
- Avoid `SELECT *` — always specify column names
- Use `LIMIT` with `ORDER BY` for pagination (avoid `OFFSET` for large offsets)
- Prefer `UNION ALL` over `UNION` when duplicates are not a concern

### InnoDB Best Practices
- Always use InnoDB (default since MySQL 5.5) — avoid MyISAM for new tables
- Use auto-increment integer primary keys for InnoDB (clustered index benefits)
- Keep primary keys narrow — all secondary indexes include the PK
- Avoid `UUID` as primary keys unless using `UUID_TO_BIN()` with swap flag (MySQL 8.0+)
- Use `ROW_FORMAT=DYNAMIC` or `COMPRESSED` for large row tables

### Character Sets & Collation
- Use `utf8mb4` not `utf8` (MySQL's `utf8` is only 3-byte, missing emoji and some characters)
- Default collation: `utf8mb4_0900_ai_ci` (MySQL 8.0+) or `utf8mb4_unicode_ci`
- Set at server level to avoid per-table/column overhead

## Essential Diagnostic Queries

```sql
-- Currently running queries
SELECT id, user, host, db, command, time, state,
       LEFT(info, 100) AS query
FROM information_schema.processlist
WHERE command != 'Sleep'
ORDER BY time DESC;

-- InnoDB buffer pool hit ratio (should be > 99%)
SELECT
    (1 - (Innodb_buffer_pool_reads / Innodb_buffer_pool_read_requests)) * 100
    AS buffer_pool_hit_ratio
FROM (
    SELECT
        VARIABLE_VALUE AS Innodb_buffer_pool_reads
    FROM performance_schema.global_status
    WHERE VARIABLE_NAME = 'Innodb_buffer_pool_reads'
) reads,
(
    SELECT
        VARIABLE_VALUE AS Innodb_buffer_pool_read_requests
    FROM performance_schema.global_status
    WHERE VARIABLE_NAME = 'Innodb_buffer_pool_read_requests'
) requests;

-- Table sizes and row counts
SELECT
    table_schema AS database_name,
    table_name,
    ROUND(data_length / 1024 / 1024, 2) AS data_size_mb,
    ROUND(index_length / 1024 / 1024, 2) AS index_size_mb,
    table_rows
FROM information_schema.tables
WHERE table_schema NOT IN ('mysql', 'information_schema', 'performance_schema', 'sys')
ORDER BY data_length DESC;

-- Unused indexes
SELECT
    object_schema, object_name, index_name,
    count_star AS rows_accessed
FROM performance_schema.table_io_waits_summary_by_index_usage
WHERE index_name IS NOT NULL
  AND count_star = 0
  AND object_schema NOT IN ('mysql', 'performance_schema')
ORDER BY object_schema, object_name;

-- InnoDB lock waits
SELECT
    r.trx_id AS waiting_trx,
    r.trx_mysql_thread_id AS waiting_thread,
    r.trx_query AS waiting_query,
    b.trx_id AS blocking_trx,
    b.trx_mysql_thread_id AS blocking_thread,
    b.trx_query AS blocking_query
FROM information_schema.innodb_lock_waits w
JOIN information_schema.innodb_trx b ON b.trx_id = w.blocking_trx_id
JOIN information_schema.innodb_trx r ON r.trx_id = w.requesting_trx_id;
```

## Configuration Tuning

### InnoDB Settings
- `innodb_buffer_pool_size` — 70-80% of total RAM (most impactful setting)
- `innodb_buffer_pool_instances` — 8 or equal to buffer pool size in GB (whichever is smaller)
- `innodb_log_file_size` — 1-2 GB for write-heavy workloads
- `innodb_flush_log_at_trx_commit` — 1 for ACID compliance, 2 for performance (risk: 1 sec data loss)
- `innodb_file_per_table` — ON (default since 5.6.6)

### Connection Settings
- `max_connections` — set based on actual need, not arbitrarily high (each connection uses RAM)
- `thread_cache_size` — 16-32 (avoids thread creation overhead)
- `wait_timeout` — 300-600 seconds (default 28800 is too high for web apps)

### Query Cache (MySQL 5.7 and below)
- **Disable query cache** — it's been removed in MySQL 8.0 for good reason
- It causes contention under concurrent workloads

### Replication
- Use GTID-based replication (MySQL 5.6+) for simpler failover
- Set `sync_binlog = 1` for crash-safe replication
- Use `binlog_format = ROW` for reliable replication
- Consider Group Replication or InnoDB Cluster for HA

## Maintenance Tasks

| Task | Frequency | Command |
|------|-----------|---------|
| Analyze tables | Weekly | `ANALYZE TABLE tablename;` |
| Optimize tables | Monthly | `ALTER TABLE tablename ENGINE=InnoDB;` (online DDL) |
| Check for corruption | Weekly | `CHECK TABLE tablename;` |
| Purge binary logs | Daily | `PURGE BINARY LOGS BEFORE NOW() - INTERVAL 7 DAY;` |
| Review slow query log | Daily | Enable `slow_query_log`, set `long_query_time = 1` |
