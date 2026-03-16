# Agents

Agents give Copilot a focused persona for interactive conversations. You invoke them explicitly and have a back-and-forth dialogue.

## How to Use

1. Copy the `.agent.md` file into your project at `.github/agents/`
2. Open VS Code with the GitHub Copilot Chat extension
3. Invoke the agent by name in Copilot Chat (e.g., `@query-optimizer`)

## Available Agents

### query-optimizer

**File:** [`.github/agents/query-optimizer.agent.md`](../.github/agents/query-optimizer.agent.md)

A senior database performance engineer that analyzes and optimizes SQL queries.

**What it does:**
- Reads and interprets execution plans
- Identifies anti-patterns: implicit conversions, SARGability violations, unnecessary sorts, key lookups
- Recommends index strategies based on query workload
- Rewrites queries for better performance without changing semantics
- Rates each recommendation by severity

**Output format:** Findings in a severity-ranked table:

| Severity | Issue | Location | Problem | Recommendation |
|----------|-------|----------|---------|----------------|
| 🔴 Critical | Missing index | `WHERE customer_id = ?` | Full table scan on 10M rows | Add index on `customer_id` |

---

### schema-reviewer

**File:** [`.github/agents/schema-reviewer.agent.md`](../.github/agents/schema-reviewer.agent.md)

A database architect that reviews schema designs for normalization, naming, and best practices.

**What it does:**
- Checks normalization levels and flags redundant data
- Reviews naming convention consistency
- Validates data type choices and nullability
- Assesses constraints (FKs, unique, check, defaults)
- Evaluates scalability concerns

**Output format:** Structured findings with an overall schema health score (1-10).

---

### incident-responder

**File:** [`.github/agents/incident-responder.agent.md`](../.github/agents/incident-responder.agent.md)

A senior DBA specializing in database incident diagnosis and resolution.

**What it does:**
- Diagnoses deadlocks, blocking chains, and lock escalation
- Identifies runaway queries and resource exhaustion
- Reads error logs, wait statistics, and system DMVs
- Recommends immediate mitigation and long-term fixes
- Documents root cause for post-incident reviews

**Output format:** Incident summary with severity, root cause, confidence level, immediate actions, and prevention plan.

---

### migration-specialist

**File:** [`.github/agents/migration-specialist.agent.md`](../.github/agents/migration-specialist.agent.md)

A database engineer that reviews migration scripts for safety and correctness.

**What it does:**
- Classifies migration risk based on table size, lock requirements, and reversibility
- Verifies syntax and constraint validity
- Identifies operations that could cause downtime
- Validates rollback scripts
- Recommends deployment strategies (online DDL, expand-contract, etc.)

**Output format:** Migration assessment with risk level, lock impact, reversibility, safety checklist, and issues found.

---

### security-auditor

**File:** [`.github/agents/security-auditor.agent.md`](../.github/agents/security-auditor.agent.md)

A database security specialist that audits configurations and identifies vulnerabilities.

**What it does:**
- Inventories users, roles, and permissions
- Flags overly broad privileges
- Reviews authentication and connection security
- Identifies columns containing PII, PHI, or financial data
- Checks audit logging coverage
- Scans for SQL injection risks

**Output format:** Severity-ranked findings with compliance gaps, quick wins, and a security roadmap.
