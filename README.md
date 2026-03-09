# GitHub Copilot for Database Analysts

A collection of agents, prompts, custom instructions, and practical tips I've put together from my own DBA workflows with GitHub Copilot. There are certainly other approaches out there — these are just the patterns that have worked well for me. I hope they help you get started and inspire you to create your own.

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

Reusable Copilot Chat participants scoped to specific database concerns. [Learn more →](docs/agents.md)

- **[query-optimizer](.github/agents/query-optimizer.agent.md)** — Analyzes slow queries, missing indexes, and execution plan inefficiencies
- **[schema-reviewer](.github/agents/schema-reviewer.agent.md)** — Reviews schema changes for referential integrity, naming conventions, and migration safety
- **[dba-troubleshooter](.github/agents/dba-troubleshooter.agent.md)** — Diagnoses blocking sessions, deadlocks, and performance bottlenecks in SQL Server
- **[powershell-dba](.github/agents/powershell-dba.agent.md)** — Generates and reviews PowerShell scripts for SQL Server administration using dbatools
- **[pr-db-reviewer](.github/agents/pr-db-reviewer.agent.md)** — Reviews PRs containing database changes for safety, rollback risk, and data integrity impact

### 💬 Prompts

Multi-step prompt files that kick off complex DBA workflows. [Learn more →](docs/prompts.md)

- **[GitHub Actions DB Pipeline](.github/prompts/db-actions-pipeline.prompt.md)** — Generates a complete CI/CD pipeline for SQL Server schema migrations and validation
- **[Schema Migration](.github/prompts/schema-migration.prompt.md)** — Creates safe, reversible schema migration scripts with rollback procedures
- **[Query Analysis & Optimization](.github/prompts/query-analysis.prompt.md)** — Analyzes T-SQL queries, execution plans, and suggests index strategies
- **[PowerShell DBA Scripting](.github/prompts/powershell-dba.prompt.md)** — Generates PowerShell scripts for SQL Server tasks using dbatools and sqlcmd
- **[Database Documentation](.github/prompts/db-documentation.prompt.md)** — Auto-generates data dictionaries, ERD descriptions, and schema docs from existing tables

### 📋 Custom Instructions

Project-level rules that shape Copilot's behavior across all interactions. [How-to guide →](docs/instructions-how-to.md)

- **[copilot-instructions.md](.github/copilot-instructions.md)** — A real-world example enforcing T-SQL naming conventions, migration checklists, and PR standards for database teams

### 🧠 Skills

Reference guides that give Copilot domain-specific database knowledge. Copy the ones you need into your project's `.github/skills/` folder — Copilot picks them up automatically as background context. No special syntax needed.

- **[Skill Index](.github/skills/skill.md)** — Overview and links to all individual skill files

<details>
<summary>Individual skill files</summary>

- **[SQL Server](.github/skills/sql-server/SKILL.md)** — T-SQL best practices, execution plans, index strategies, query optimization
- **[PowerShell DBA](.github/skills/powershell-dba/SKILL.md)** — dbatools, sqlcmd, Invoke-Sqlcmd, scripting patterns for SQL Server administration
- **[GitHub Actions for Databases](.github/skills/gh-actions-db/SKILL.md)** — CI/CD workflows for schema migrations, drift detection, backup validation
- **[Database Troubleshooting](.github/skills/db-troubleshooting/SKILL.md)** — Blocking, deadlocks, wait stats, DMVs, performance diagnostics
- **[Schema Design](.github/skills/schema-design/SKILL.md)** — Normalization, naming conventions, referential integrity, migration safety

</details>

---

## 💡 Tips & Tricks

Practical patterns to get better database results from Copilot.

- **[VS Code & MSSQL Extension](docs/tips-vscode-mssql.md)** — Connecting to SQL Server, running queries, SQLCMD mode, and Copilot integration
- **[Database Troubleshooting with Copilot](docs/tips-troubleshooting.md)** — Prompting patterns for diagnosing slow queries, blocking, and deadlocks
- **[PowerShell Scripting with Copilot](docs/tips-powershell.md)** — Patterns for generating and iterating on DBA PowerShell scripts

---

<details>
<summary><strong>Repo Structure</strong></summary>

```
docs/
  agents.md                  # How to use agents
  prompts.md                 # How to use prompts
  instructions-how-to.md    # How to set up custom instructions
  decision-tree.md           # When to use an agent vs skill vs prompt
  tips-vscode-mssql.md      # VS Code & MSSQL extension tips
  tips-troubleshooting.md   # Database troubleshooting with Copilot
  tips-powershell.md        # PowerShell scripting with Copilot
.github/
  agents/
    query-optimizer.agent.md   # Query performance analysis agent
    schema-reviewer.agent.md   # Schema change review agent
    dba-troubleshooter.agent.md # Blocking & deadlock diagnosis agent
    powershell-dba.agent.md    # PowerShell DBA scripting agent
    pr-db-reviewer.agent.md    # PR database change review agent
  prompts/
    db-actions-pipeline.prompt.md  # GitHub Actions CI/CD pipeline prompt
    schema-migration.prompt.md     # Safe schema migration prompt
    query-analysis.prompt.md       # Query analysis & optimization prompt
    powershell-dba.prompt.md       # PowerShell DBA scripting prompt
    db-documentation.prompt.md     # Database documentation prompt
  copilot-instructions.md    # Real-world custom instructions example
  skills/
    skill.md                       # Skill index — links to all individual skills
    sql-server/SKILL.md            # SQL Server T-SQL skill
    powershell-dba/SKILL.md        # PowerShell DBA skill
    gh-actions-db/SKILL.md         # GitHub Actions for databases skill
    db-troubleshooting/SKILL.md    # Database troubleshooting skill
    schema-design/SKILL.md         # Schema design skill
```

</details>

## Contributing

Have a useful agent, prompt, or instruction file for database workflows? Open a PR to add it.

## Further Reading

Looking for more examples and community resources? Check out [awesome-copilot](https://github.com/github/awesome-copilot) — a curated list of GitHub Copilot resources, tools, and guides.

## License

This project is open source. See the repository for license details.
