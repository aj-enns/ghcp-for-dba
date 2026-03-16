---
mode: agent
description: Analyzes database tables and query patterns to recommend optimal indexes
---

# Index Analysis & Recommendations

Analyze the database schema and query workload to recommend optimal indexing strategies.

## Step 1 — Gather Schema & Query Context

Read the provided SQL files, migration scripts, or schema definitions. Identify:

- All tables with their columns, data types, and existing indexes
- Common query patterns (WHERE clauses, JOIN conditions, ORDER BY, GROUP BY)
- Current index usage statistics if available

Save a summary of findings to `index-analysis/schema-summary.md`.

## Step 2 — Identify Missing & Redundant Indexes

For each table, analyze:

- **Missing indexes** — queries doing full table scans that would benefit from an index
- **Redundant indexes** — indexes that are subsets of other indexes or never used
- **Overlapping indexes** — indexes that can be consolidated
- **Over-indexed tables** — tables with excessive indexes impacting write performance

Rate each finding by impact:
- 🔴 Critical — queries scanning millions of rows without an index
- 🟠 High — frequently executed queries with suboptimal plans
- 🟡 Medium — occasional queries that would benefit from covering indexes
- 🟢 Low — minor improvements or cleanup opportunities

## Step 3 — Generate Index Recommendations

For each recommendation, provide:

```sql
-- Problem: [describe the query pattern and current behavior]
-- Impact: [estimated improvement]
-- Write overhead: [impact on INSERT/UPDATE/DELETE]
CREATE INDEX IX_tablename_columns
ON schema.tablename (column1, column2)
INCLUDE (column3, column4);
```

Save all recommendations to `index-analysis/recommendations.sql`.

## Step 4 — Create Implementation Plan

Generate a prioritized implementation plan in `index-analysis/implementation-plan.md` that includes:

1. Priority-ordered list of indexes to create
2. Indexes to drop (redundant/unused)
3. Estimated build time for large tables
4. Recommended maintenance window requirements
5. Validation queries to confirm improvement
