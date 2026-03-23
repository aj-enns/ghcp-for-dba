# Task 1 — Run a Health Check (Prompt)

[< Back to Lab Overview](lab-walkthrough.md)  |  [Next: Task 2 — Optimize a Slow Query >](lab-task-02.md)

---

**What you'll learn:** How reusable prompt files automate multi-step workflows.

## Try it

1. Open Copilot Chat in VS Code
2. Type `/health-check` and press Enter
3. Watch Copilot connect to RetailDb, run diagnostic queries, and generate scored findings across storage, performance, integrity, and security

## What happened

The prompt file [`.github/prompts/health-check.prompt.md`](../.github/prompts/health-check.prompt.md) defined a 4-step workflow. Copilot followed the steps — collecting system info, running assessments, generating a remediation plan, and creating monitoring recommendations — without you writing a single query.

## Explore further

Open the prompt file and read how each step is structured. Try modifying Step 2 to add a new check category.

---

[< Back to Lab Overview](lab-walkthrough.md)  |  [Next: Task 2 — Optimize a Slow Query >](lab-task-02.md)
