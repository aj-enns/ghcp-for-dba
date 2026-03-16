---
mode: agent
description: Performs a comprehensive database health check with actionable findings
---

# Database Health Check

Run a comprehensive health assessment of your database environment.

## Step 1 — Collect System Information

Gather and document the database environment details:

- Server version, edition, and patch level
- Operating system and hardware specs (CPU, RAM, storage)
- Database sizes, file locations, and growth settings
- Configuration settings (max memory, max degree of parallelism, cost threshold for parallelism)
- Active maintenance plans and job schedules

Save the inventory to `health-check/system-inventory.md`.

## Step 2 — Assess Database Health

Run diagnostic checks across these categories:

### Storage & Files
- Data and log file sizes, free space, and auto-growth settings
- Disk latency and throughput metrics
- TempDB configuration (number of files, sizing)

### Performance
- Top wait statistics and trends
- Most expensive queries (CPU, reads, duration)
- Index fragmentation levels
- Statistics freshness and accuracy
- Plan cache efficiency (plan reuse ratio)

### Integrity
- Last successful DBCC CHECKDB run date
- Any reported corruption or consistency errors
- Backup chain integrity and last backup dates

### Security
- Orphaned users and unused logins
- Overly permissive roles
- Audit configuration status

Present findings in `health-check/assessment.md` using a dashboard format:

| Category | Status | Score | Key Finding |
|----------|--------|-------|-------------|
| Storage | 🟡 Warning | 7/10 | Log file auto-growth set to 1MB |
| Performance | 🔴 Critical | 4/10 | 3 queries causing 80% of I/O |

## Step 3 — Generate Remediation Plan

For each finding, provide a prioritized action item in `health-check/remediation-plan.md`:

| Priority | Category | Issue | Action | Effort | Impact |
|----------|----------|-------|--------|--------|--------|
| 1 | Performance | Missing index | Create covering index | Low | High |

## Step 4 — Create Ongoing Monitoring Recommendations

Document recommended monitoring setup in `health-check/monitoring-setup.md`:

- Key metrics to track with alert thresholds
- Recommended monitoring tools and queries
- Weekly/monthly DBA checklist
- Automated health check script (if applicable)
