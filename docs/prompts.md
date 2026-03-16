# Prompts

Prompt files define multi-step workflows that Copilot executes in sequence. They're ideal for repeatable DBA processes — tasks you'd otherwise document in a runbook and forget.

## How to Use

1. Copy the `.prompt.md` file into your project at `.github/prompts/`
2. Open VS Code with the GitHub Copilot Chat extension
3. Run the prompt from Copilot Chat — it will execute the steps in sequence

## Available Prompts

### Index Analysis & Recommendations

**File:** [`.github/prompts/index-analysis.prompt.md`](../.github/prompts/index-analysis.prompt.md)

Analyzes your schema and query workload to recommend optimal indexing strategies.

**What it does (4 steps):**

1. **Gathers schema & query context** — reads SQL files and identifies tables, columns, existing indexes, and query patterns
2. **Identifies missing & redundant indexes** — flags full scans, unused indexes, overlapping indexes, and over-indexed tables
3. **Generates index recommendations** — provides CREATE INDEX statements with comments explaining the problem and expected impact
4. **Creates implementation plan** — prioritized list with estimated build times and validation queries

---

### Backup & Recovery Strategy

**File:** [`.github/prompts/backup-strategy.prompt.md`](../.github/prompts/backup-strategy.prompt.md)

Creates a comprehensive backup and disaster recovery plan tailored to your environment.

**What it does (4 steps):**

1. **Assesses environment** — identifies database engines, sizes, RPO/RTO requirements, and compliance needs
2. **Designs backup schedule** — full, differential, and log backup frequencies with retention policies
3. **Creates recovery procedures** — step-by-step instructions for point-in-time recovery, full restore, and DR failover
4. **Generates monitoring plan** — backup health alerts, monthly restore tests, and annual DR drill checklists

---

### Performance Tuning Workflow

**File:** [`.github/prompts/performance-tuning.prompt.md`](../.github/prompts/performance-tuning.prompt.md)

Systematic approach to diagnosing and resolving database performance issues.

**What it does (4 steps):**

1. **Establishes baseline** — collects wait stats, resource utilization, top queries, and connection metrics
2. **Identifies bottlenecks** — categorizes issues as CPU, I/O, memory, locking, or network bound
3. **Generates recommendations** — query rewrites, index changes, configuration tuning, and schema changes ranked by effort and impact
4. **Creates implementation plan** — ordered changes with dependencies, validation queries, and rollback procedures

---

### Data Migration

**File:** [`.github/prompts/data-migration.prompt.md`](../.github/prompts/data-migration.prompt.md)

Generates safe data migration scripts with full integrity verification.

**What it does (4 steps):**

1. **Analyzes source & target** — maps columns, identifies type differences, and determines migration order
2. **Generates migration scripts** — batched INSERT/SELECT with transformations, error handling, and transaction management
3. **Creates validation scripts** — row counts, checksums, FK verification, and business rule checks
4. **Creates rollback scripts** — reverse migration in dependency order with verification queries

---

### Database Health Check

**File:** [`.github/prompts/health-check.prompt.md`](../.github/prompts/health-check.prompt.md)

Comprehensive health assessment of your database environment.

**What it does (4 steps):**

1. **Collects system inventory** — server version, hardware, database sizes, configuration settings
2. **Assesses health** — storage, performance, integrity, and security checks with dashboard-style scoring
3. **Generates remediation plan** — prioritized action items with effort and impact ratings
4. **Creates monitoring recommendations** — key metrics, alert thresholds, and ongoing DBA checklists

---

### Schema Documentation

**File:** [`.github/prompts/schema-documentation.prompt.md`](../.github/prompts/schema-documentation.prompt.md)

Generates complete documentation from your database schema definitions.

**What it does (4 steps):**

1. **Extracts metadata** — tables, columns, constraints, indexes, views, and stored procedures
2. **Generates data dictionary** — organized by schema/table with descriptions, types, and relationships
3. **Creates relationship map** — FK relationships with Mermaid ERD diagrams
4. **Creates change log template** — schema change documentation format and review checklist

---

### Capacity Planning

**File:** [`.github/prompts/capacity-planning.prompt.md`](../.github/prompts/capacity-planning.prompt.md)

Analyzes current usage and projects future capacity needs.

**What it does (4 steps):**

1. **Collects current usage** — storage, compute, memory, I/O, and connection metrics
2. **Analyzes growth patterns** — projects future needs at 6-month and 12-month horizons
3. **Identifies capacity risks** — flags resources approaching critical thresholds with time-to-threshold
4. **Generates scaling recommendations** — vertical, horizontal, optimization, and cloud migration options

---

## Writing Your Own Prompts

A good prompt file includes:

- **Frontmatter** with `mode: agent` and a `description`
- **Numbered steps** so Copilot executes them in order
- **Specific output paths** (folder names, file names)
- **Acceptance criteria** for each step
- **A fallback** if external resources are unavailable

See the Health Check prompt for a well-structured example.
