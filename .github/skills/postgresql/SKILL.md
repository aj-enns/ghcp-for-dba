# PostgreSQL DBA Skill

## PL/pgSQL Patterns

### Query Style
- Use `EXPLAIN (ANALYZE, BUFFERS, FORMAT TEXT)` for execution plan analysis
- Prefer CTEs with `MATERIALIZED` / `NOT MATERIALIZED` hints when appropriate (PG 12+)
- Use `RETURNING` clause to avoid separate SELECT after INSERT/UPDATE/DELETE
- Use `ON CONFLICT` (upsert) instead of check-then-insert patterns
- Prefer `generate_series()` over loops for set generation
- Use `FILTER (WHERE ...)` clause with aggregates instead of CASE expressions

### Connection Management
- Always use connection pooling (PgBouncer or pgpool-II)
- Set `idle_in_transaction_session_timeout` to prevent abandoned transactions
- Use `statement_timeout` to prevent runaway queries
- Set application name in connection string for monitoring: `application_name=myapp`

### Data Types
- Use `UUID` (with `gen_random_uuid()`) for distributed-safe primary keys
- Use `TIMESTAMPTZ` not `TIMESTAMP` for all timestamps
- Use `TEXT` instead of `VARCHAR(n)` unless you need a length constraint
- Use `JSONB` not `JSON` for storage (supports indexing and operators)
- Use `NUMERIC` for money, never `FLOAT` or `REAL`

## Essential Diagnostic Queries

```sql
-- Active queries and their state
SELECT pid, now() - pg_stat_activity.query_start AS duration,
       query, state, wait_event_type, wait_event
FROM pg_stat_activity
WHERE state != 'idle'
  AND pid != pg_backend_pid()
ORDER BY duration DESC;

-- Table bloat estimation
SELECT schemaname, tablename,
       pg_size_pretty(pg_total_relation_size(schemaname || '.' || tablename)) AS total_size,
       n_dead_tup,
       n_live_tup,
       CASE WHEN n_live_tup > 0
            THEN round(n_dead_tup * 100.0 / n_live_tup, 1)
            ELSE 0 END AS dead_pct,
       last_autovacuum,
       last_autoanalyze
FROM pg_stat_user_tables
ORDER BY n_dead_tup DESC;

-- Index usage statistics
SELECT schemaname, tablename, indexrelname,
       idx_scan, idx_tup_read, idx_tup_fetch,
       pg_size_pretty(pg_relation_size(indexrelid)) AS index_size
FROM pg_stat_user_indexes
ORDER BY idx_scan ASC;

-- Cache hit ratio (should be > 99%)
SELECT
    sum(heap_blks_read) AS heap_read,
    sum(heap_blks_hit) AS heap_hit,
    CASE WHEN sum(heap_blks_hit) > 0
         THEN round(sum(heap_blks_hit) * 100.0 / (sum(heap_blks_hit) + sum(heap_blks_read)), 2)
         ELSE 0 END AS hit_ratio
FROM pg_statio_user_tables;

-- Lock monitoring
SELECT pid, locktype, relation::regclass, mode, granted,
       pg_blocking_pids(pid) AS blocked_by
FROM pg_locks
WHERE NOT granted;
```

## Configuration Tuning

### Memory Settings
- `shared_buffers` — 25% of total RAM (up to ~8 GB)
- `effective_cache_size` — 50-75% of total RAM
- `work_mem` — start at 256 MB, increase for sort-heavy workloads (caution: per-operation)
- `maintenance_work_mem` — 1-2 GB for vacuum, index creation

### WAL & Checkpoint Settings
- `wal_buffers` — 64 MB for write-heavy workloads
- `checkpoint_completion_target` — 0.9 (spread checkpoint I/O)
- `max_wal_size` — 2-4 GB (default 1 GB is often too low)

### Autovacuum Tuning
- `autovacuum_vacuum_scale_factor` — lower to 0.05-0.1 for large tables (default 0.2)
- `autovacuum_analyze_scale_factor` — lower to 0.02-0.05 for large tables
- `autovacuum_max_workers` — increase to 5-6 for databases with many tables
- `autovacuum_vacuum_cost_delay` — lower to 2ms for faster vacuum on SSD storage

### Query Planner
- `random_page_cost` — set to 1.1-1.5 for SSD storage (default 4.0 is for HDD)
- `effective_io_concurrency` — 200 for SSD storage
- `default_statistics_target` — increase to 500-1000 for tables with skewed data

## Extensions

### Essential Extensions
| Extension | Purpose |
|-----------|---------|
| `pg_stat_statements` | Query performance statistics (must-have) |
| `auto_explain` | Automatic logging of slow query plans |
| `pgcrypto` | Encryption functions |
| `pg_trgm` | Trigram-based text similarity and fuzzy matching |
| `btree_gin` / `btree_gist` | GIN/GiST index support for common types |
| `citext` | Case-insensitive text type |

### Enabling pg_stat_statements
```sql
-- In postgresql.conf
-- shared_preload_libraries = 'pg_stat_statements'
CREATE EXTENSION IF NOT EXISTS pg_stat_statements;

-- Top queries by total time
SELECT query,
       calls,
       total_exec_time / 1000 AS total_sec,
       mean_exec_time AS avg_ms,
       rows
FROM pg_stat_statements
ORDER BY total_exec_time DESC
LIMIT 20;
```

## Backup & Recovery

- Use `pg_basebackup` for physical backups with WAL archiving for PITR
- Use `pg_dump` / `pg_dumpall` for logical backups (schema + data)
- Configure continuous archiving with `archive_mode = on` for point-in-time recovery
- Test restores regularly — untested backups are not backups
- Consider pgBackRest or Barman for enterprise backup management
