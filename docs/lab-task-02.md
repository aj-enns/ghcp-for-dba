# Task 2 — Optimize a Slow Query (Agent)

[< Previous: Task 1 — Health Check](lab-task-01.md)  |  [Next: Task 3 — Review a Migration >](lab-task-03.md)

---

**What you'll learn:** How agents act as specialized personas that analyze problems in context.

## Try it

1. Open Copilot Chat
2. Type: `@query-optimizer Can you analyze this query for performance issues?`
   ```sql
   SELECT c.FirstName, c.LastName, o.OrderDate, o.TotalAmount,
          p.Name AS ProductName, oi.Quantity
   FROM Customers c
   JOIN Orders o ON c.CustomerId = o.CustomerId
   JOIN OrderItems oi ON o.OrderId = oi.OrderId
   JOIN Products p ON oi.ProductId = p.ProductId
   WHERE c.Country = 'USA'
   ORDER BY o.TotalAmount DESC
   ```
3. The agent will analyze the query, check for missing indexes, SARGability issues, and suggest optimizations

## What happened

The `query-optimizer` agent ([`.github/agents/query-optimizer.agent.md`](../.github/agents/query-optimizer.agent.md)) has instructions that make Copilot behave like a performance tuning specialist — it knows to look at execution plans, suggest covering indexes, and flag anti-patterns.

## Explore further

Try asking `@query-optimizer What indexes would improve the Orders table for reporting queries?`

---

[< Previous: Task 1 — Health Check](lab-task-01.md)  |  [Next: Task 3 — Review a Migration >](lab-task-03.md)
