---
mode: agent
description: Systematic database performance tuning workflow — from baseline to optimization
---

# Performance Tuning Workflow

A structured approach to diagnosing and resolving database performance issues.

## Step 1 — Establish Baseline

Collect current performance metrics to establish a baseline:

- **Wait statistics** — identify the top wait types consuming time
- **Resource utilization** — CPU, memory, disk I/O, network throughput
- **Top queries** — most expensive queries by CPU, reads, duration, and execution count
- **Connection metrics** — active sessions, connection pool utilization, blocking chains

Generate diagnostic queries appropriate for the user's RDBMS and save results to `performance-tuning/baseline-metrics.md`.

## Step 2 — Identify Bottlenecks

Analyze the baseline data to identify the primary bottleneck category:

| Category | Indicators | Common Causes |
|----------|-----------|---------------|
| **CPU** | High CPU %, `SOS_SCHEDULER_YIELD` waits | Missing indexes, implicit conversions, excessive recompilation |
| **I/O** | High disk latency, `PAGEIOLATCH_*` waits | Missing indexes, large scans, insufficient memory |
| **Memory** | Page life expectancy drops, `RESOURCE_SEMAPHORE` | Insufficient RAM, memory-hungry queries, excessive plan cache |
| **Locking** | `LCK_*` waits, blocking chains | Long transactions, missing indexes, poor isolation level choice |
| **Network** | High network waits, large result sets | `SELECT *`, unnecessary data transfer, chatty application patterns |

Save the analysis to `performance-tuning/bottleneck-analysis.md`.

## Step 3 — Generate Optimization Recommendations

For each identified bottleneck, provide actionable recommendations:

- **Query-level fixes** — rewritten queries with before/after execution plan comparisons
- **Index changes** — new indexes to create, redundant indexes to remove
- **Configuration changes** — server/database settings to tune (with current vs recommended values)
- **Schema changes** — denormalization, partitioning, or archiving strategies

Rate each recommendation by effort and impact:

| Priority | Recommendation | Effort | Impact | Risk |
|----------|---------------|--------|--------|------|
| 1 | Add covering index on `orders` | Low | High | Low |

Save to `performance-tuning/recommendations.md`.

## Step 4 — Create Implementation & Validation Plan

Generate a step-by-step implementation plan in `performance-tuning/implementation-plan.md`:

1. Ordered list of changes with dependencies
2. Pre-implementation validation queries
3. Post-implementation validation queries
4. Rollback procedures for each change
5. Success criteria with measurable targets
