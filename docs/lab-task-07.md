# Task 7 — Incident Response Simulation (Agent + Skills)

[< Previous: Task 6 — Customize Coding Standards](lab-task-06.md)  |  [Back to Lab Overview](lab-walkthrough.md)

---

**What you'll learn:** How agents and skills work together for complex troubleshooting.

## Try it

1. In Copilot Chat, type: `@incident-responder We're seeing high CPU on our RetailDb database. Can you help diagnose the issue?`
2. The agent will walk you through:
   - Checking top wait statistics
   - Identifying CPU-intensive queries from Query Store
   - Looking at recent plan regressions
   - Recommending immediate actions
3. When the agent references monitoring concepts, it draws from the skills in `.github/skills/monitoring/` and `.github/skills/sql-server/`

## What happened

The `incident-responder` agent ([`.github/agents/incident-responder.agent.md`](../.github/agents/incident-responder.agent.md)) simulated a real on-call triage workflow. The skills gave Copilot deeper domain knowledge about DMVs, wait types, and SQL Server internals — so its recommendations were specific rather than generic.

## Explore further

Try other scenarios:
- `@incident-responder We have a deadlock between two processes updating Orders and Inventories`
- `@incident-responder A query that normally takes 200ms is now taking 30 seconds`

---

## What You've Learned

| Resource | What It Does | When to Use It |
|----------|-------------|----------------|
| **Prompts** (Tasks 1, 5) | Multi-step workflows that produce deliverables | Repeatable processes: health checks, documentation, capacity planning |
| **Agents** (Tasks 2, 3, 4, 7) | Specialized personas with deep domain focus | Interactive analysis: query tuning, code review, security audits, troubleshooting |
| **Skills** (Task 7) | Background knowledge Copilot draws from automatically | Platform-specific expertise: SQL Server DMVs, PostgreSQL tuning, indexing theory |
| **Custom Instructions** (Task 6) | Rules Copilot follows on every interaction | Team standards: naming conventions, safety guards, migration formats |

**Next steps:**
- Browse [docs/decision-tree.md](decision-tree.md) to learn when to create each resource type
- Try the other prompts: `/index-analysis`, `/performance-tuning`, `/backup-strategy`, `/capacity-planning`
- Create your own agent for a workflow specific to your team

---

[< Previous: Task 6 — Customize Coding Standards](lab-task-06.md)  |  [Back to Lab Overview](lab-walkthrough.md)
