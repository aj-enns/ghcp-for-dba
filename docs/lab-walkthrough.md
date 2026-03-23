# Hands-On Lab: GitHub Copilot for DBAs

Work through these 7 tasks to learn how agents, prompts, skills, and custom instructions accelerate common DBA workflows. Each task uses the **RetailDb** lab database you deployed earlier.

> **Prerequisites:** RetailDb deployed and `.env` configured. See [lab/README.md](../lab/README.md) if you haven't done that yet.

---

## Tasks

| # | Task | Feature | What You'll Do |
|---|------|---------|---------------|
| 1 | [Run a Health Check](lab-task-01.md) | Prompt | Use `/health-check` to assess storage, performance, integrity, and security |
| 2 | [Optimize a Slow Query](lab-task-02.md) | Agent | Ask `@query-optimizer` to analyze a multi-join query |
| 3 | [Review a Migration Script](lab-task-03.md) | Agent | Have `@migration-specialist` review a stored procedure deployment |
| 4 | [Audit Database Security](lab-task-04.md) | Agent | Run `@security-auditor` to find gaps in access controls and encryption |
| 5 | [Generate Schema Documentation](lab-task-05.md) | Prompt | Use `/schema-documentation` to create a data dictionary and ERD |
| 6 | [Customize Coding Standards](lab-task-06.md) | Custom Instructions | Add a naming rule and see Copilot enforce it immediately |
| 7 | [Incident Response Simulation](lab-task-07.md) | Agent + Skills | Diagnose a simulated high-CPU incident with `@incident-responder` |

---

## What You'll Learn

| Resource | What It Does | When to Use It |
|----------|-------------|----------------|
| **Prompts** (Tasks 1, 5) | Multi-step workflows that produce deliverables | Repeatable processes: health checks, documentation, capacity planning |
| **Agents** (Tasks 2, 3, 4, 7) | Specialized personas with deep domain focus | Interactive analysis: query tuning, code review, security audits, troubleshooting |
| **Skills** (Task 7) | Background knowledge Copilot draws from automatically | Platform-specific expertise: SQL Server DMVs, PostgreSQL tuning, indexing theory |
| **Custom Instructions** (Task 6) | Rules Copilot follows on every interaction | Team standards: naming conventions, safety guards, migration formats |

**When you're done:**
- Browse [docs/decision-tree.md](decision-tree.md) to learn when to create each resource type
- Try the other prompts: `/index-analysis`, `/performance-tuning`, `/backup-strategy`, `/capacity-planning`
- Create your own agent for a workflow specific to your team
