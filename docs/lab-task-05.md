# Task 5 — Generate Schema Documentation (Prompt)

[< Previous: Task 4 — Audit Security](lab-task-04.md)  |  [Next: Task 6 — Customize Coding Standards >](lab-task-06.md)

---

**What you'll learn:** How prompts can create deliverables from your live schema.

## Try it

1. In Copilot Chat, type `/schema-documentation`
2. Copilot will connect to RetailDb, inspect every table, and generate:
   - A data dictionary with column types and constraints
   - A relationship map showing all foreign keys
   - A Mermaid ER diagram you can render in markdown

## What happened

The prompt [`.github/prompts/schema-documentation.prompt.md`](../.github/prompts/schema-documentation.prompt.md) automated what typically takes hours of manual DDL scripting and diagramming.

## Explore further

Copy the Mermaid diagram output into a `.md` file and preview it in VS Code (install the Mermaid extension if needed).

---

[< Previous: Task 4 — Audit Security](lab-task-04.md)  |  [Next: Task 6 — Customize Coding Standards >](lab-task-06.md)
