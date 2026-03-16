# Indexing Strategies Skill

## Index Types

### B-Tree Indexes (Default)
The standard index type for most workloads. Efficient for equality and range queries.

| Feature | Clustered | Non-Clustered |
|---------|:---------:|:-------------:|
| Table order | Determines physical order | Separate structure |
| Per table | One only | Multiple allowed |
| Leaf nodes | Actual data rows | Pointers to data |
| Best for | Range scans, PK lookups | Selective point queries |

### Specialized Index Types
| Type | When to Use | RDBMS |
|------|------------|-------|
| **Covering index** (INCLUDE) | Query needs extra columns beyond key | SQL Server, PostgreSQL |
| **Filtered index** | Only a subset of rows is queried | SQL Server |
| **Partial index** | Same as filtered | PostgreSQL |
| **Columnstore** | Analytics, aggregations on large tables | SQL Server |
| **GIN** | Full-text search, JSONB, arrays | PostgreSQL |
| **GiST** | Geometric, range, and nearest-neighbor | PostgreSQL |
| **Hash** | Equality-only lookups | PostgreSQL (limited use) |
| **Full-text** | Text search | SQL Server, MySQL |

## Index Design Principles

### Column Order in Composite Indexes
The **leftmost prefix rule** determines which queries can use a composite index:

```sql
-- Index: (country, city, zip_code)
-- ✅ Can use index:
WHERE country = 'US'
WHERE country = 'US' AND city = 'Seattle'
WHERE country = 'US' AND city = 'Seattle' AND zip_code = '98101'

-- ❌ Cannot use index (skips leading column):
WHERE city = 'Seattle'
WHERE zip_code = '98101'
WHERE city = 'Seattle' AND zip_code = '98101'
```

### Column Ordering Guidelines
1. **Equality columns first** — columns used with `=` in WHERE
2. **Range column last** — the column used with `>`, `<`, `BETWEEN`, `LIKE 'x%'`
3. **High selectivity columns earlier** — columns that filter out more rows

```sql
-- Query pattern:
-- WHERE status = 'active' AND region = 'west' AND created_at > '2024-01-01'

-- Optimal index:
CREATE INDEX IX_orders_status_region_created
ON orders (status, region, created_at);
-- Equality (status, region) first, range (created_at) last
```

### Covering Indexes
Include non-key columns to avoid key lookups:

```sql
-- Query:
SELECT order_id, total FROM orders WHERE customer_id = 42 AND status = 'shipped';

-- Covering index (SQL Server):
CREATE INDEX IX_orders_customer_status
ON orders (customer_id, status);
INCLUDE (order_id, total);

-- PostgreSQL equivalent:
CREATE INDEX IX_orders_customer_status
ON orders (customer_id, status)
INCLUDE (order_id, total);
```

### Filtered / Partial Indexes
Index only the rows that matter:

```sql
-- SQL Server: filtered index
CREATE INDEX IX_orders_active
ON orders (customer_id, order_date)
WHERE status = 'active';

-- PostgreSQL: partial index
CREATE INDEX IX_orders_active
ON orders (customer_id, order_date)
WHERE status = 'active';
```

## Index Maintenance

### Fragmentation Thresholds
| Fragmentation | Action | Impact |
|:------------:|--------|--------|
| < 10% | None | Minimal benefit from maintenance |
| 10-30% | REORGANIZE | Online, minimal locking |
| > 30% | REBUILD | More effective, but more resource-intensive |

### SQL Server Index Maintenance
```sql
-- Check fragmentation
SELECT
    OBJECT_NAME(ips.object_id) AS table_name,
    i.name AS index_name,
    ips.avg_fragmentation_in_percent,
    ips.page_count
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.page_count > 1000
  AND ips.avg_fragmentation_in_percent > 10
ORDER BY ips.avg_fragmentation_in_percent DESC;

-- Reorganize (online, lightweight)
ALTER INDEX IX_name ON schema.table REORGANIZE;

-- Rebuild (more thorough, can be online with Enterprise)
ALTER INDEX IX_name ON schema.table REBUILD WITH (ONLINE = ON);
```

### PostgreSQL Index Maintenance
```sql
-- Check index bloat
SELECT schemaname, tablename, indexrelname,
       pg_size_pretty(pg_relation_size(indexrelid)) AS index_size,
       idx_scan AS times_used
FROM pg_stat_user_indexes
ORDER BY pg_relation_size(indexrelid) DESC;

-- Rebuild index (blocks writes)
REINDEX INDEX CONCURRENTLY index_name;

-- Alternative: create new, swap, drop old
CREATE INDEX CONCURRENTLY IX_new ON table (columns);
DROP INDEX CONCURRENTLY IX_old;
```

## Anti-Patterns

| Anti-Pattern | Problem | Better Approach |
|-------------|---------|-----------------|
| Index on every column | Write overhead, storage waste | Index based on query patterns |
| Wide indexes (many columns) | Large index size, slow maintenance | Use INCLUDE for non-key columns |
| Redundant indexes | `(A)` and `(A, B)` — first is redundant | Drop the subset index |
| Index on low-cardinality column | `(status)` with 3 values — rarely selective | Combine with selective column or use filtered index |
| Ignoring index maintenance | Fragmented indexes degrade over time | Regular reorganize/rebuild schedule |
| Not monitoring unused indexes | Wasted write I/O and storage | Review `dm_db_index_usage_stats` quarterly |
