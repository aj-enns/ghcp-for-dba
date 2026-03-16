---
mode: agent
description: Creates a comprehensive backup and disaster recovery strategy for your database environment
---

# Backup & Recovery Strategy

Generate a complete backup and disaster recovery plan tailored to your database environment.

## Step 1 — Assess Current Environment

Analyze the user's database environment to understand:

- Database engine(s) in use (SQL Server, PostgreSQL, MySQL, etc.)
- Database sizes and growth rates
- Recovery Point Objective (RPO) and Recovery Time Objective (RTO) requirements
- Current backup infrastructure (if any)
- Compliance requirements (HIPAA, SOC 2, PCI-DSS, GDPR)
- High availability configuration (replication, clustering, AlwaysOn)

Document findings in `backup-strategy/environment-assessment.md`.

## Step 2 — Design Backup Schedule

Based on the environment assessment, create a backup schedule that includes:

- **Full backups** — frequency, retention period, storage location
- **Differential backups** — frequency relative to full backups
- **Transaction log backups** — frequency based on RPO requirements
- **Special considerations** — large tables, filegroups, partitioned data

Present the schedule as a clear table:

| Backup Type | Frequency | Retention | Storage | Encryption |
|-------------|-----------|-----------|---------|------------|
| Full | Weekly (Sunday 2 AM) | 90 days | Off-site + cloud | AES-256 |

Save to `backup-strategy/backup-schedule.md`.

## Step 3 — Create Recovery Procedures

Document step-by-step recovery procedures for common scenarios:

1. **Point-in-time recovery** — restore to a specific timestamp
2. **Full database restore** — complete database recovery from backup
3. **Table-level recovery** — restore individual tables
4. **Page-level repair** — fix corrupt pages without full restore
5. **Disaster recovery failover** — switch to DR site

Each procedure should include exact commands, verification steps, and estimated recovery time.

Save to `backup-strategy/recovery-procedures.md`.

## Step 4 — Generate Monitoring & Testing Plan

Create a plan for ongoing backup health in `backup-strategy/monitoring-plan.md`:

- Backup success/failure alerts
- Backup size trending and storage capacity monitoring
- Monthly restore test procedures with success criteria
- Annual DR drill checklist
- Compliance audit documentation requirements
