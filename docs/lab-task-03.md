# Task 3 — Review a Migration Script (Agent)

[< Previous: Task 2 — Optimize a Query](lab-task-02.md)  |  [Next: Task 4 — Audit Security >](lab-task-04.md)

---

**What you'll learn:** How agents can review your work before it hits production.

## Try it

1. Open [data-patches/002_create_usp_add_product.sql](../lab/scripts/data-patches/002_create_usp_add_product.sql)
2. In Copilot Chat, type: `@migration-specialist Can you review this migration script for safety and correctness?`
3. The agent will evaluate lock impact, rollback coverage, error handling, and data safety

## What happened

The `migration-specialist` agent ([`.github/agents/migration-specialist.agent.md`](../.github/agents/migration-specialist.agent.md)) reviewed the script against a checklist of migration safety concerns — exactly what a senior DBA would check in a code review.

## Explore further

Try introducing a deliberate issue (e.g., remove the `BEGIN TRY` / `BEGIN CATCH` block) and ask the agent to review again. See how it catches the regression.

---

[< Previous: Task 2 — Optimize a Query](lab-task-02.md)  |  [Next: Task 4 — Audit Security >](lab-task-04.md)
