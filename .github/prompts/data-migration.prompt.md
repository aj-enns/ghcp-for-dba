---
mode: agent
description: Generates data migration scripts with validation, rollback, and integrity verification
---

# Data Migration Script Generation

Generate safe, validated data migration scripts with full integrity checking.

## Step 1 — Analyze Source & Target

Read the provided source and target schema definitions to understand:

- Table structures and column mappings
- Data type differences requiring transformation
- Constraint differences (FK, unique, check constraints)
- Volume estimates and batching requirements
- Referential integrity dependencies (migration order)

Document the mapping in `data-migration/column-mapping.md` as a table:

| Source Table | Source Column | Target Table | Target Column | Transformation |
|-------------|--------------|-------------|--------------|----------------|
| `old_users` | `full_name` | `users` | `first_name` | `SUBSTRING_INDEX(full_name, ' ', 1)` |

## Step 2 — Generate Migration Scripts

Create migration scripts that include:

- **Pre-migration checks** — verify source data quality, counts, and constraints
- **Migration script** — INSERT/SELECT with appropriate transformations, batched for large tables
- **Referential integrity order** — parent tables before child tables
- **Error handling** — TRY/CATCH blocks, transaction management, error logging

Use batching for large tables:

```sql
-- Migrate in batches of 10,000 rows
DECLARE @BatchSize INT = 10000;
DECLARE @RowCount INT = 1;

WHILE @RowCount > 0
BEGIN
    -- batch migration logic here
END
```

Save scripts to `data-migration/migrate.sql`.

## Step 3 — Generate Validation Scripts

Create post-migration validation in `data-migration/validate.sql`:

- **Row count comparison** — source vs target counts per table
- **Checksum validation** — hash comparison of key columns
- **Referential integrity** — verify all FK relationships are satisfied
- **Business rule validation** — domain-specific checks (e.g., no negative balances)
- **Sample data spot-check** — compare random rows between source and target

## Step 4 — Generate Rollback Scripts

Create a complete rollback plan in `data-migration/rollback.sql`:

- Reverse the migration in dependency order (child tables first)
- Restore original data from backup tables
- Re-enable constraints and triggers
- Verification queries to confirm rollback success
