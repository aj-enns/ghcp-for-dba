# Query Optimization Skill

## Execution Plan Reading

### Key Operators to Watch
| Operator | Concern Level | What It Means |
|----------|:------------:|---------------|
| **Table Scan / Seq Scan** | 🔴 | Reading every row — usually means missing index |
| **Key Lookup / Bookmark Lookup** | 🟠 | Extra I/O to fetch non-covered columns |
| **Hash Match (Aggregate)** | 🟡 | Hashing for GROUP BY — may benefit from index |
| **Sort** | 🟡 | Explicit sort operation — check if index can eliminate it |
| **Nested Loops** | 🟢 | Good for small outer sets with indexed inner |
| **Hash Join** | 🟢 | Good for large unsorted datasets |
| **Merge Join** | 🟢 | Efficient when both inputs are sorted |

### What to Look For
- **Estimated vs actual rows** — large discrepancies indicate stale statistics
- **Fat arrows (SQL Server)** — wide data flow means lots of rows being processed
- **Warnings** — implicit conversions, missing column statistics, spills to tempdb
- **Parallelism** — is it beneficial or causing overhead?

## SARGability Rules

A predicate is **SARGable** (Search ARGument able) when it can use an index seek.

### SARGable (Good ✅)
```sql
WHERE order_date >= '2024-01-01'
WHERE customer_id = 42
WHERE last_name LIKE 'Smith%'
WHERE status IN ('active', 'pending')
```

### Non-SARGable (Bad ❌)
```sql
WHERE YEAR(order_date) = 2024          -- Function on column
WHERE CAST(id AS VARCHAR) = '42'       -- Type conversion on column
WHERE last_name LIKE '%Smith'          -- Leading wildcard
WHERE price * 1.1 > 100               -- Calculation on column
WHERE ISNULL(status, '') = 'active'    -- Function on column
WHERE column1 + column2 > 100         -- Expression on columns
```

### Rewrites for SARGability
```sql
-- Instead of: WHERE YEAR(order_date) = 2024
WHERE order_date >= '2024-01-01' AND order_date < '2025-01-01'

-- Instead of: WHERE ISNULL(status, '') = 'active'
WHERE status = 'active'

-- Instead of: WHERE price * 1.1 > 100
WHERE price > 100 / 1.1
```

## Join Optimization

### Join Order Matters
- Start with the most selective table (fewest rows after filtering)
- The optimizer usually handles this, but hints may help in complex queries

### Anti-Patterns
```sql
-- ❌ Implicit cross join
SELECT * FROM orders, customers WHERE orders.customer_id = customers.id

-- ✅ Explicit join
SELECT * FROM orders JOIN customers ON orders.customer_id = customers.id

-- ❌ Correlated subquery for every row
SELECT *, (SELECT name FROM customers c WHERE c.id = o.customer_id) AS name
FROM orders o

-- ✅ Rewrite as join
SELECT o.*, c.name FROM orders o JOIN customers c ON o.customer_id = c.id
```

### Join Type Selection Guide
| Scenario | Best Join Type | Index Needed |
|----------|---------------|--------------|
| Small outer, indexed inner | Nested Loops | Yes, on join column |
| Large unsorted tables | Hash Join | Not required |
| Both inputs pre-sorted | Merge Join | On join columns |
| Existence check | Semi Join (EXISTS) | On join column |

## Parameter Sniffing

### What It Is
SQL Server caches the first execution plan and reuses it. If the first execution has atypical parameter values, all subsequent executions use a suboptimal plan.

### Detection
- Query performs well sometimes, poorly other times
- `SET STATISTICS IO ON` shows vastly different read counts for same query
- Estimated row counts are far from actual in execution plan

### Mitigation Strategies
1. **`OPTION (RECOMPILE)`** — forces new plan each time (CPU cost)
2. **`OPTIMIZE FOR UNKNOWN`** — uses average statistics instead of sniffed value
3. **`OPTIMIZE FOR (@param = value)`** — optimize for a typical value
4. **Plan guides** — force a specific plan shape
5. **Query Store** — monitor and force good plans (SQL Server 2016+)

## Common Anti-Patterns

| Anti-Pattern | Problem | Fix |
|-------------|---------|-----|
| `SELECT *` | Extra I/O, prevents covering indexes | List specific columns |
| `SELECT DISTINCT` to fix duplicates | Masks a join bug | Fix the join logic |
| `ORDER BY` without `TOP`/`LIMIT` | Sorting entire result set | Add `TOP` or remove sort |
| Scalar function in WHERE | Called per row, prevents parallelism | Inline the logic or use TVF |
| `NOLOCK` everywhere | Dirty reads, inconsistent results | Use `READ COMMITTED SNAPSHOT` |
| Row-by-row processing | Cursor/loop overhead | Rewrite as set-based |
