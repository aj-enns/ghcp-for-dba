# Tip: Reading Execution Plans

A practical pattern for using Copilot to analyze and learn from execution plans.

## The Pattern

When you have a slow query, paste the query and its execution plan output into Copilot Chat and ask:

> "Analyze this execution plan. Identify the most expensive operators, explain why they're costly, and suggest specific optimizations with before/after SQL."

## Why It Works

Copilot can parse execution plan text and correlate operators with known optimization patterns. By asking for "before/after SQL," you get concrete changes rather than vague advice.

## What to Paste

### SQL Server
```sql
SET STATISTICS IO ON;
SET STATISTICS TIME ON;

-- Your query here

SET STATISTICS IO OFF;
SET STATISTICS TIME OFF;
```

Or use the graphical plan — right-click → "Show Execution Plan" and paste the XML.

### PostgreSQL
```sql
EXPLAIN (ANALYZE, BUFFERS, FORMAT TEXT)
-- Your query here
```

### MySQL
```sql
EXPLAIN ANALYZE
-- Your query here
```

## What to Look For

| Indicator | Meaning | Action |
|-----------|---------|--------|
| **Table Scan / Seq Scan** | No useful index found | Check WHERE/JOIN columns for index candidates |
| **Estimated vs actual rows differ wildly** | Stale statistics | Update statistics or use `OPTION (RECOMPILE)` |
| **Key Lookup** | Index doesn't cover all columns | Add INCLUDE columns to the index |
| **Sort operator** | Explicit sort required | Add index that provides the sort order |
| **Spill to tempdb** | Insufficient memory grant | Review query structure, consider `OPTION (HASH JOIN)` |

## Variations

| Prompt variation | When to use |
|-----------------|-------------|
| "...and include the index CREATE statements" | When you want ready-to-run DDL |
| "...explain it as if I'm a junior DBA" | When you're learning plan reading |
| "...compare these two plans side by side" | When testing a query rewrite |
| "...focus only on the I/O statistics" | When diagnosing disk bottlenecks |
