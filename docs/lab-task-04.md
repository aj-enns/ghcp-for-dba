# Task 4 — Audit Database Security (Agent)

[< Previous: Task 3 — Review a Migration](lab-task-03.md)  |  [Next: Task 5 — Schema Documentation >](lab-task-05.md)

---

**What you'll learn:** How to use an agent for security assessments.

## Try it

1. In Copilot Chat, type: `@security-auditor Can you audit the security configuration of our RetailDb database? Check for overly permissive roles, encryption status, and auditing configuration.`
2. The agent will connect to the database, inspect principals, TDE, auditing policies, and firewall rules
3. Review the findings — it should flag that auditing is disabled and no least-privilege roles exist

## What happened

The `security-auditor` agent ([`.github/agents/security-auditor.agent.md`](../.github/agents/security-auditor.agent.md)) performed the same checks a security consultant would run, querying DMVs and Azure configurations.

## Explore further

Ask `@security-auditor What RBAC roles should we create for an application that only needs to read Orders and Products?`

---

[< Previous: Task 3 — Review a Migration](lab-task-03.md)  |  [Next: Task 5 — Schema Documentation >](lab-task-05.md)
