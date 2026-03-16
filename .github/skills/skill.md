# DBA Skills Index

Skills give Copilot domain-specific DBA knowledge. Copy the ones you need into your project's `.github/skills/` folder — Copilot picks them up automatically.

## Available Skills

### Database Platforms
- **[SQL Server](sql-server/SKILL.md)** — T-SQL patterns, DMVs, configuration, maintenance
- **[PostgreSQL](postgresql/SKILL.md)** — PL/pgSQL, extensions, vacuuming, configuration tuning
- **[MySQL](mysql/SKILL.md)** — InnoDB internals, replication, character sets, optimization

### Core DBA Disciplines
- **[Query Optimization](query-optimization/SKILL.md)** — Execution plans, join strategies, SARGability, parameter sniffing
- **[Indexing Strategies](indexing-strategies/SKILL.md)** — B-tree design, covering indexes, filtered indexes, maintenance
- **[Security Hardening](security-hardening/SKILL.md)** — Access control, encryption, auditing, compliance patterns
- **[High Availability](high-availability/SKILL.md)** — Replication, clustering, failover, disaster recovery
- **[Monitoring & Alerting](monitoring/SKILL.md)** — Wait stats, DMV queries, baseline metrics, alert thresholds
- **[ETL & Data Pipelines](etl-patterns/SKILL.md)** — Bulk loading, incremental loads, CDC, data validation
- **Request DMV queries** — "Write a DMV query to find the top 10 most expensive queries by CPU" is a great starting point
- **Use the troubleshooter pattern** — Capture diagnostic output → Paste into chat → Ask Copilot to interpret and recommend fixes