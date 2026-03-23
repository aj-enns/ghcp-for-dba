# GitHub Copilot for Database Analysts

A collection of agents, prompts, custom instructions, and practical tips I've put together from my own DBA workflows with GitHub Copilot. There are certainly other approaches out there — these are just the patterns that have worked well for me. I hope they help you get started and inspire you to create your own.

**Who is this for?** DBAs, data engineers, and anyone who manages databases and wants to use GitHub Copilot more effectively. Bring your own database or [deploy the included lab](#-lab-environment-optional) to follow along with a sample retail schema.

---

## Getting Started

> **Prerequisites:** This repo assumes you already have GitHub Copilot installed, configured, and know the basics. If not, see [Get started with GitHub Copilot](https://docs.github.com/en/copilot/get-started) first.

1. **Browse the docs** — click any link below to learn how to use each resource
2. **Copy what you need** — drop agents, prompts, or instruction files into your project's `.github/` folder
3. **Use in VS Code** — with the GitHub Copilot Chat extension installed, agents and prompts are automatically available in your workspace

**Not sure which resource type you need?** See the [Agent vs Skill vs Prompt decision tree →](docs/decision-tree.md)

---

## What's Inside

### 🤖 Agents

Reusable Copilot Chat participants scoped to specific DBA concerns. [Learn more →](docs/agents.md)

- **[query-optimizer](.github/agents/query-optimizer.agent.md)** — Analyzes SQL queries for performance issues, reads execution plans, recommends index strategies
- **[schema-reviewer](.github/agents/schema-reviewer.agent.md)** — Reviews schema designs for normalization, naming, data types, and constraint completeness
- **[incident-responder](.github/agents/incident-responder.agent.md)** — Diagnoses deadlocks, blocking chains, resource exhaustion, and outages
- **[migration-specialist](.github/agents/migration-specialist.agent.md)** — Reviews migration scripts for safety, lock impact, and rollback capability
- **[security-auditor](.github/agents/security-auditor.agent.md)** — Audits database security: access controls, encryption, injection risks, compliance gaps

### 💬 Prompts

Multi-step prompt files that kick off complex DBA workflows. [Learn more →](docs/prompts.md)

- **[Index Analysis](.github/prompts/index-analysis.prompt.md)** — Analyzes schema and query workload to recommend optimal indexes
- **[Backup & Recovery Strategy](.github/prompts/backup-strategy.prompt.md)** — Creates comprehensive backup plans with schedules, procedures, and monitoring
- **[Performance Tuning](.github/prompts/performance-tuning.prompt.md)** — Systematic bottleneck diagnosis with baseline → analysis → recommendations → implementation
- **[Data Migration](.github/prompts/data-migration.prompt.md)** — Generates migration scripts with column mapping, batching, validation, and rollback
- **[Database Health Check](.github/prompts/health-check.prompt.md)** — Full environment assessment with scored findings and remediation plan
- **[Schema Documentation](.github/prompts/schema-documentation.prompt.md)** — Generates data dictionaries, relationship maps, and Mermaid ERDs from DDL
- **[Capacity Planning](.github/prompts/capacity-planning.prompt.md)** — Growth projections, risk assessment, and scaling recommendations

### 📋 Custom Instructions

Project-level rules that shape Copilot's behavior across all interactions. [How-to guide →](docs/instructions-how-to.md)

- **[copilot-instructions.md](.github/copilot-instructions.md)** — A real-world example enforcing SQL naming conventions, migration safety patterns, and code review checklists

### 🧠 Skills

Reference guides that give Copilot domain-specific DBA knowledge. Copy the ones you need into your project's `.github/skills/` folder — Copilot picks them up automatically as background context. No special syntax needed.

- **[Skill Index](.github/skills/skill.md)** — Overview and links to all individual skill files

<details>
<summary>Individual skill files</summary>

- **[SQL Server](.github/skills/sql-server/SKILL.md)** — T-SQL patterns, DMVs, configuration tuning, maintenance schedules
- **[PostgreSQL](.github/skills/postgresql/SKILL.md)** — PL/pgSQL, extensions (pg_stat_statements), vacuuming, config tuning
- **[MySQL](.github/skills/mysql/SKILL.md)** — InnoDB internals, replication, character sets, performance schema
- **[Query Optimization](.github/skills/query-optimization/SKILL.md)** — Execution plans, SARGability, join strategies, parameter sniffing
- **[Indexing Strategies](.github/skills/indexing-strategies/SKILL.md)** — B-tree design, covering indexes, filtered/partial indexes, maintenance
- **[Security Hardening](.github/skills/security-hardening/SKILL.md)** — Least privilege, encryption, audit logging, compliance checklists
- **[High Availability](.github/skills/high-availability/SKILL.md)** — AlwaysOn AGs, streaming replication, clustering, DR planning
- **[Monitoring & Alerting](.github/skills/monitoring/SKILL.md)** — Wait stats analysis, key metrics, alert thresholds, dashboard queries
- **[ETL & Data Pipelines](.github/skills/etl-patterns/SKILL.md)** — Bulk loading, CDC, incremental loads, merge patterns, validation

</details>

---

## 💡 Tips & Tricks

Practical patterns to get better DBA results from Copilot.

- **[Reading Execution Plans](docs/tips-explain-plans.md)** — Paste plan output into Copilot for analysis, learn what operators to watch for
- **[Naming Conventions](docs/tips-naming-conventions.md)** — Enforce consistent naming via custom instructions across your entire schema
- **[Safe Database Deployments](docs/tips-safe-deployments.md)** — A prompting pattern that wraps every migration in pre-checks, transactions, and rollback scripts

---

<details>
<summary><strong>Repo Structure</strong></summary>

```
docs/
  agents.md                    # How to use agents
  prompts.md                   # How to use prompts
  instructions-how-to.md       # How to set up custom instructions
  decision-tree.md             # When to use an agent vs skill vs prompt
  ef-quickstart.md             # EF Core commands for the lab
  tips-explain-plans.md        # Execution plan analysis tip
  tips-naming-conventions.md   # Database naming conventions tip
  tips-safe-deployments.md     # Safe deployment patterns tip
.github/
  agents/
    query-optimizer.agent.md       # Query performance analysis agent
    schema-reviewer.agent.md       # Schema design review agent
    incident-responder.agent.md    # Database incident diagnosis agent
    migration-specialist.agent.md  # Migration safety review agent
    security-auditor.agent.md      # Database security audit agent
  prompts/
    index-analysis.prompt.md       # Index recommendation workflow
    backup-strategy.prompt.md      # Backup & recovery planning
    performance-tuning.prompt.md   # Performance bottleneck diagnosis
    data-migration.prompt.md       # Data migration script generation
    health-check.prompt.md         # Database health assessment
    schema-documentation.prompt.md # Schema documentation generation
    capacity-planning.prompt.md    # Capacity planning & projections
  copilot-instructions.md         # SQL coding standards & safety rules
  skills/
    skill.md                       # Skill index — links to all skills
    sql-server/SKILL.md            # SQL Server DBA skill
    postgresql/SKILL.md            # PostgreSQL DBA skill
    mysql/SKILL.md                 # MySQL/MariaDB DBA skill
    query-optimization/SKILL.md    # Query tuning skill
    indexing-strategies/SKILL.md   # Indexing strategies skill
    security-hardening/SKILL.md    # Security hardening skill
    high-availability/SKILL.md     # HA/DR skill
    monitoring/SKILL.md            # Monitoring & alerting skill
    etl-patterns/SKILL.md          # ETL & data pipeline skill
lab/                               # Optional Azure SQL lab environment
  infra/main.bicep                 # Bicep template for Azure SQL
  scripts/deploy.ps1               # PowerShell deployment script
  scripts/deploy.sh                # Bash deployment script
  src/RetailDb/                    # EF Core project (migrations + seed)
```

</details>

---

## 🧪 Lab Environment (Optional)

Want a real database to practice on? The `lab/` folder provisions an **Azure SQL Database** with a 10-table retail schema and realistic seed data — ready for testing every agent, prompt, and skill in this repo.

**What you get:** Categories, Products, Customers, Orders, Employees, Stores, Inventories, Reviews, and more (279 rows of interconnected data).

| | |
|---|---|
| **Deploy the lab** | [lab/README.md](lab/README.md) — Bicep + EF Core, deploy + seed |
| **Hands-on walkthrough** | [docs/lab-walkthrough.md](docs/lab-walkthrough.md) — 7 guided tasks using agents, prompts, skills & instructions |
| **EF Core reference** | [docs/ef-quickstart.md](docs/ef-quickstart.md) — Migration and seed commands |
| **Cost** | Standard S1 (20 DTUs) — ~$0.98/day. Tear down when done. |

> **Don't need a lab?** Everything above (agents, prompts, skills, tips) works with your own databases — no lab required.

---

## Contributing

Contributions welcome! If you have a useful agent, prompt, skill, or tip for DBA workflows, feel free to open a PR.

**Ways to contribute:**
- Add a new agent, prompt, or skill file
- Improve existing documentation or fix typos
- Share a tip or prompting pattern that works well for your DBA workflow
- Report issues or suggest improvements via [GitHub Issues](https://github.com/aj-enns/ghcp-for-dba/issues)

Please follow the existing file structure and naming conventions when adding new resources.

## Further Reading

Looking for more examples and community resources? Check out [awesome-copilot](https://github.com/github/awesome-copilot) — a curated list of GitHub Copilot resources, tools, and guides.

## License

This project is licensed under the [MIT License](LICENSE).
