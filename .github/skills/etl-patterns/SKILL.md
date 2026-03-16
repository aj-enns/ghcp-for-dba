# ETL & Data Pipeline Skill

## Loading Strategies

### Bulk Load Methods
| Method | RDBMS | Best For |
|--------|-------|----------|
| `BULK INSERT` | SQL Server | Loading from flat files |
| `bcp` utility | SQL Server | Command-line bulk operations |
| `COPY` command | PostgreSQL | Fast CSV/binary loading |
| `LOAD DATA INFILE` | MySQL | Fast file-based loading |
| SSIS | SQL Server | Complex ETL workflows |
| `pg_bulkload` | PostgreSQL | Maximum load speed |

### SQL Server Bulk Load Best Practices
```sql
-- Minimal logging with BULK INSERT
BULK INSERT staging.raw_data
FROM 'data.csv'
WITH (
    FIELDTERMINATOR = ',',
    ROWTERMINATOR = '\n',
    FIRSTROW = 2,           -- skip header
    BATCHSIZE = 100000,     -- commit every 100K rows
    TABLOCK,                -- table lock for minimal logging
    ERRORFILE = 'errors.csv'
);
```

### PostgreSQL COPY
```sql
-- Fast load from CSV
COPY staging.raw_data
FROM '/path/to/data.csv'
WITH (FORMAT csv, HEADER true, DELIMITER ',', NULL '');

-- Fast unload to CSV
COPY (SELECT * FROM production.orders WHERE order_date > '2024-01-01')
TO '/path/to/export.csv'
WITH (FORMAT csv, HEADER true);
```

## Incremental Load Patterns

### Change Data Capture (CDC)
Track changes automatically without modifying application code.

**SQL Server CDC:**
```sql
-- Enable CDC on database
EXEC sys.sp_cdc_enable_db;

-- Enable CDC on table
EXEC sys.sp_cdc_enable_table
    @source_schema = 'dbo',
    @source_name = 'orders',
    @role_name = 'cdc_reader';

-- Query changes since last load
DECLARE @from_lsn binary(10) = sys.fn_cdc_get_min_lsn('dbo_orders');
DECLARE @to_lsn binary(10) = sys.fn_cdc_get_max_lsn();

SELECT * FROM cdc.fn_cdc_get_net_changes_dbo_orders(@from_lsn, @to_lsn, 'all');
```

### High-Water Mark Pattern
```sql
-- Track last loaded timestamp
CREATE TABLE etl.watermarks (
    source_table VARCHAR(128) PRIMARY KEY,
    last_loaded_at DATETIME2 NOT NULL,
    last_loaded_id BIGINT NULL,
    rows_loaded BIGINT NOT NULL DEFAULT 0
);

-- Incremental load using watermark
DECLARE @last_loaded DATETIME2;
SELECT @last_loaded = last_loaded_at
FROM etl.watermarks WHERE source_table = 'orders';

INSERT INTO warehouse.orders
SELECT * FROM source.orders
WHERE modified_at > @last_loaded;

-- Update watermark
UPDATE etl.watermarks
SET last_loaded_at = GETUTCDATE(),
    rows_loaded = @@ROWCOUNT
WHERE source_table = 'orders';
```

### Merge Pattern (Upsert)
```sql
-- SQL Server MERGE
MERGE INTO target.customers AS t
USING staging.customers AS s
ON t.customer_id = s.customer_id
WHEN MATCHED AND (t.email != s.email OR t.name != s.name) THEN
    UPDATE SET t.email = s.email, t.name = s.name, t.updated_at = GETUTCDATE()
WHEN NOT MATCHED BY TARGET THEN
    INSERT (customer_id, email, name, created_at)
    VALUES (s.customer_id, s.email, s.name, GETUTCDATE());

-- PostgreSQL upsert
INSERT INTO target.customers (customer_id, email, name, updated_at)
SELECT customer_id, email, name, NOW()
FROM staging.customers
ON CONFLICT (customer_id)
DO UPDATE SET
    email = EXCLUDED.email,
    name = EXCLUDED.name,
    updated_at = NOW()
WHERE target.customers.email != EXCLUDED.email
   OR target.customers.name != EXCLUDED.name;
```

## Data Validation

### Pre-Load Validation Checks
| Check | Query Pattern | Purpose |
|-------|--------------|---------|
| Row count | `SELECT COUNT(*) FROM staging` | Compare against source |
| Null checks | `WHERE required_col IS NULL` | Catch missing data |
| Range checks | `WHERE amount < 0 OR amount > 1000000` | Catch outliers |
| Duplicate check | `GROUP BY pk HAVING COUNT(*) > 1` | Prevent duplicate loads |
| FK validation | `LEFT JOIN parent WHERE parent.id IS NULL` | Orphan detection |
| Type validation | `TRY_CAST(col AS INT) IS NULL` | Catch format errors |

### Post-Load Reconciliation
```sql
-- Row count reconciliation
SELECT
    'source' AS dataset, COUNT(*) AS row_count FROM source.orders
UNION ALL
SELECT
    'target' AS dataset, COUNT(*) AS row_count FROM target.orders;

-- Checksum comparison (SQL Server)
SELECT CHECKSUM_AGG(CHECKSUM(*)) AS data_checksum FROM source.orders;
SELECT CHECKSUM_AGG(CHECKSUM(*)) AS data_checksum FROM target.orders;

-- Hash comparison (PostgreSQL)
SELECT md5(string_agg(t::text, '')) FROM source.orders t;
SELECT md5(string_agg(t::text, '')) FROM target.orders t;
```

## ETL Anti-Patterns

| Anti-Pattern | Problem | Better Approach |
|-------------|---------|-----------------|
| Row-by-row processing | Extremely slow for large datasets | Set-based operations with batch commits |
| No error handling | Silent failures, partial loads | TRY/CATCH with error logging table |
| Loading directly to production | No validation, no rollback | Stage → validate → load pattern |
| No idempotency | Re-running causes duplicates | Use MERGE or delete-then-insert |
| Hardcoded connection strings | Security risk, environment inflexibility | Use environment variables or secret managers |
| No monitoring | Failures go unnoticed | Log start/end times, row counts, and errors |
