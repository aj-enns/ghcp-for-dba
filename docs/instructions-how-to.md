# Custom Instructions

Custom instructions shape how Copilot behaves across all interactions in your repo. They act as persistent context — rules Copilot follows every time it generates SQL, reviews a script, or responds in chat.

## How to Use

1. Create a file named `.github/copilot-instructions.md` in your project
2. Add your rules in markdown format
3. Copilot will automatically pick up the instructions for all interactions in that workspace

## Included Example

**File:** [`.github/copilot-instructions.md`](../.github/copilot-instructions.md)

This real-world example enforces **SQL coding standards**, **migration safety**, and **code review practices** for DBA workflows. It includes:

- **Naming conventions** — PascalCase tables/columns, prefixed constraints and procedures
- **Formatting rules** — uppercase keywords, one clause per line, qualified column names
- **Safety rules** — rollback scripts required, no DROP without IF EXISTS, explicit transactions
- **Migration format** — header blocks, pre/post validation, TRY/CATCH structure
- **Review checklist** — no SELECT *, PKs on all tables, appropriate data types, plans reviewed

## Writing Your Own Instructions

Effective custom instructions are:

- **Specific** — tell Copilot exactly what naming pattern to use and what format scripts should follow
- **Scoped** — focus on rules that apply project-wide, not one-off tasks
- **Actionable** — include concrete examples, not vague guidelines
- **Maintained** — update them as your team's conventions evolve

Common things to include for DBA projects:
- SQL formatting and naming conventions
- Migration script requirements (header blocks, rollback scripts)
- Data type preferences (`DECIMAL` for money, `NVARCHAR` vs `VARCHAR`)
- Testing and validation requirements
- Environment-specific rules (SQL Server version, compatibility level)

## Tip: Rule Order Matters

Copilot processes instructions top-to-bottom. Put your most critical rules first:

1. **Safety rules** — transaction handling, rollback requirements, no-drop guards
2. **SQL standards** — naming conventions, formatting, data types
3. **Process rules** — migration format, review checklists, documentation
4. **Style preferences** — comment style, alias conventions, keyword casing
