# EF Core Quick Start — RetailDb

A quick-reference guide for working with EF Core in the RetailDb lab project.

---

## Prerequisites

```bash
# Ensure .NET 8 SDK is installed
dotnet --version

# Install the EF Core CLI tool (one-time)
dotnet tool install --global dotnet-ef
```

---

## 1. Set the Connection String

All EF Core commands require the `RETAILDB_CONNECTION_STRING` environment variable.

**PowerShell**
```powershell
$env:RETAILDB_CONNECTION_STRING = "Server=tcp:<server>.database.windows.net,1433;Initial Catalog=RetailDb;Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

**Bash**
```bash
export RETAILDB_CONNECTION_STRING="Server=tcp:<server>.database.windows.net,1433;Initial Catalog=RetailDb;Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
```

> **Tip:** The `RetailDbContextFactory` provides a local fallback (`localhost:1433`) when the variable is unset, which works for the `dotnet ef migrations add` workflow without a running database.

---

## 2. Migrations

### List migrations and their status
```bash
dotnet ef migrations list --project src/RetailDb
```

### Add a new migration
After modifying a model in `Models/`, generate a migration:
```bash
dotnet ef migrations add AddLoyaltyPoints --project src/RetailDb
```
This creates a new file under `src/RetailDb/Migrations/` with `Up()` and `Down()` methods.

### Remove the last unapplied migration
Made a mistake? Remove the most recent migration (must not be applied yet):
```bash
dotnet ef migrations remove --project src/RetailDb
```

### Apply pending migrations to the database
```bash
dotnet ef database update --project src/RetailDb
```

### Roll back to a specific migration
```bash
dotnet ef database update InitialCreate --project src/RetailDb
```

### Generate a SQL script (for review or production deployment)
```bash
# Full script from empty → latest
dotnet ef migrations script --project src/RetailDb -o migrations.sql

# Script between two specific migrations
dotnet ef migrations script InitialCreate AddLoyaltyPoints --project src/RetailDb -o delta.sql

# Idempotent script (safe to run multiple times)
dotnet ef migrations script --idempotent --project src/RetailDb -o idempotent.sql
```

---

## 3. Custom CLI Commands (Program.cs)

The project includes a custom CLI entry point with these commands:

```bash
# Apply schema migrations only
dotnet run --project src/RetailDb -- migrate

# Seed retail data (idempotent — skips if data exists)
dotnet run --project src/RetailDb -- seed

# Migrate + seed in one step
dotnet run --project src/RetailDb -- setup

# Drop the database (destructive!)
dotnet run --project src/RetailDb -- drop
```

---

## 4. DbContext Info

```bash
# Show DbContext configuration details
dotnet ef dbcontext info --project src/RetailDb

# List registered DbContext types
dotnet ef dbcontext list --project src/RetailDb
```

---

## 5. Common Model Change Workflow

Here's the typical workflow when modifying the schema:

### Step 1 — Edit the model
```csharp
// Models/Customer.cs — add a new property
public int LoyaltyPoints { get; set; } = 0;
```

### Step 2 — Configure in DbContext (if needed)
```csharp
// Data/RetailDbContext.cs — inside OnModelCreating
modelBuilder.Entity<Customer>(entity =>
{
    // ... existing config ...
    entity.Property(e => e.LoyaltyPoints).HasDefaultValue(0);
});
```

### Step 3 — Generate the migration
```bash
dotnet ef migrations add AddLoyaltyPoints --project src/RetailDb
```

### Step 4 — Review the generated migration
Check the `Up()` and `Down()` methods in the new migration file under `Migrations/`.

### Step 5 — Apply to the database
```bash
dotnet ef database update --project src/RetailDb
```

---

## 6. Schema at a Glance

| DbSet | Table | Key Entity Relationships |
|---|---|---|
| `Categories` | Categories | Self-referencing (ParentCategoryId) → subcategories |
| `Suppliers` | Suppliers | One-to-many → Products |
| `Products` | Products | FK → Category, Supplier |
| `Customers` | Customers | One-to-many → Orders, Reviews |
| `Stores` | Stores | One-to-many → Employees, Orders, Inventories |
| `Employees` | Employees | FK → Store; self-referencing (ManagerId) |
| `Orders` | Orders | FK → Customer, Store, Employee |
| `OrderItems` | OrderItems | FK → Order (cascade delete), Product |
| `Inventories` | Inventories | FK → Product, Store (unique composite) |
| `Reviews` | Reviews | FK → Product, Customer |

---

## 7. Key Design Patterns Used

| Pattern | Where | Why |
|---|---|---|
| Fluent API config | `RetailDbContext.OnModelCreating` | Explicit control over column types, indexes, delete behavior |
| `decimal(18,2)` | Prices, totals, salary | Avoids floating-point precision errors for currency |
| `DeleteBehavior.Restrict` | Most FKs | Prevents accidental cascade deletes across entities |
| `DeleteBehavior.Cascade` | OrderItem → Order | Deleting an order removes its line items |
| Named indexes | `IX_`, `UQ_` prefixes | Follows DBA naming conventions from copilot-instructions |
| Design-time factory | `RetailDbContextFactory` | Enables `dotnet ef` CLI without a running host |

---

## 8. Useful Tips

- **All `dotnet ef` commands** should be run from the `lab/` directory
- **Migrations are code** — review the generated `Up()` / `Down()` methods before applying
- **`--idempotent` flag** generates safe scripts that check migration history before applying
- **`dotnet ef migrations script`** is ideal for generating SQL to hand to a DBA for review
- The seed is **idempotent** — running it twice won't duplicate data
