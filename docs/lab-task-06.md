# Task 6 — Customize the Coding Standards (Custom Instructions)

[< Previous: Task 5 — Schema Documentation](lab-task-05.md)  |  [Next: Task 7 — Incident Response >](lab-task-07.md)

---

**What you'll learn:** How custom instructions shape Copilot's behavior across all interactions.

## Try it

1. Open [`.github/copilot-instructions.md`](../.github/copilot-instructions.md)
2. Find the **Naming Conventions** section
3. Add a new rule:
   ```
   - Temporal tables: suffix with `History` (e.g., `OrderHistory`)
   ```
4. Now in Copilot Chat, ask: `Can you create a temporal table to track changes to the Products table?`
5. Notice Copilot names the history table `ProductHistory` — following your new rule

## What happened

Copilot reads `copilot-instructions.md` on every interaction. Your new naming rule was immediately applied without restarting anything. This is how teams enforce consistency across every script Copilot generates.

## Explore further

Try adding a rule like `- Always include a comment explaining WHY an index is needed, not just WHAT it indexes` and then ask Copilot to create an index. See how the output changes.

---

[< Previous: Task 5 — Schema Documentation](lab-task-05.md)  |  [Next: Task 7 — Incident Response >](lab-task-07.md)
