---
name: dba-skills-index
description: "Index of all DBA skill files for GitHub Copilot. Reference this file to find the right skill for SQL Server query optimization, PowerShell scripting, GitHub Actions database pipelines, troubleshooting, and schema design."
---

# Database Analyst Skills for GitHub Copilot

A collection of domain-specific skill files that give Copilot context for different database disciplines. Each skill lives in its own subdirectory under `.github/skills/` with a `SKILL.md` file — Copilot picks them up automatically as background context. No special syntax needed.

---

## Available Skills

| Skill | What it covers |
|-------|---------------|
| [sql-server](sql-server/SKILL.md) | T-SQL best practices, execution plans, index strategies, SARGability, query optimization, DMVs |
| [powershell-dba](powershell-dba/SKILL.md) | dbatools, Invoke-Sqlcmd, error handling, logging, scripting patterns for SQL Server administration |
| [gh-actions-db](gh-actions-db/SKILL.md) | CI/CD workflows for schema migrations, DACPAC, Flyway, Liquibase, drift detection, backup validation |
| [db-troubleshooting](db-troubleshooting/SKILL.md) | Blocking chains, deadlocks, wait stats, DMV analysis, Query Store, index fragmentation |
| [schema-design](schema-design/SKILL.md) | Normalization, naming conventions, referential integrity, migration safety, temporal tables, partitioning |

---

## General Tips for DBAs with Copilot

- **Paste context** — drop your T-SQL, execution plan XML, or DMV output directly into chat so Copilot can analyze it
- **Ask for before/after** — "Show me the current query and the optimized version side by side"
- **Be specific about the environment** — mention SQL Server version (e.g., 2019, 2022, Azure SQL) so Copilot gives version-appropriate advice
- **Ask for rollback scripts** — always request a rollback alongside any migration script
- **Iterate on PowerShell** — start with a working script, then ask Copilot to add error handling, logging, and parameter validation
- **Request DMV queries** — "Write a DMV query to find the top 10 most expensive queries by CPU" is a great starting point
- **Use the troubleshooter pattern** — Capture diagnostic output → Paste into chat → Ask Copilot to interpret and recommend fixes