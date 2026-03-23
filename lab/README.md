# Retail Lab Environment

A self-contained lab environment for the **GitHub Copilot for DBAs** demos. It provisions an **Azure SQL Database** via Bicep and populates it with a realistic retail dataset managed by **EF Core** (code-first migrations).

---

## What Gets Created

### Azure Infrastructure (`infra/`)
| Resource | Details |
|---|---|
| Azure SQL Server | Latest engine (v12), TLS 1.2, public endpoint |
| Azure SQL Database | `RetailDb`, Standard S1 (10 GB), Latin1 collation |
| Firewall rule | Allow Azure services (0.0.0.0 → 0.0.0.0) |

### Database Schema (10 tables)

```
Category ──< Product >── Supplier
                │
     ┌──────────┴─────────┐
     │                    │
  OrderItem >── Order     Inventory
                │   \
             Customer  Store ──< Employee
                            │
                         Review ──< Customer
                                 \── Product
```

| Table | Rows (seed) | Purpose |
|---|---|---|
| `Category` | 16 | Product taxonomy (6 top-level + 10 sub-categories) |
| `Supplier` | 6 | Vendor/supplier companies |
| `Product` | 26 | SKUs with price, cost, reorder levels |
| `Customer` | 15 | Registered shoppers |
| `Store` | 5 | Physical store locations |
| `Employee` | 12 | Staff with manager hierarchy |
| `Order` | 21 | Sales orders with status lifecycle |
| `OrderItem` | ~45 | Line items per order |
| `Inventory` | 130 | Stock levels per product per store |
| `Review` | 10 | Star ratings and text reviews |

---

## Prerequisites

| Tool | Version | Install |
|---|---|---|
| Azure CLI | ≥ 2.57 | [docs.microsoft.com](https://learn.microsoft.com/cli/azure/install-azure-cli) |
| Bicep CLI | ≥ 0.27 | Bundled with Azure CLI (`az bicep install`) |
| .NET SDK | 8.x | [dotnet.microsoft.com](https://dotnet.microsoft.com/download) |

---

## Quick Start

### 1 — Log in to Azure

```bash
az login
az account set --subscription "<your-subscription-id>"
```

### 2 — Deploy infrastructure + seed data

**macOS / Linux**
```bash
cd lab
./scripts/deploy.sh \
  --resource-group rg-retail-lab \
  --password "YourStrong!Passw0rd" \
  --location eastus
```

**Windows (PowerShell)**
```powershell
cd lab
.\scripts\deploy.ps1 `
  -ResourceGroup rg-retail-lab `
  -SqlPassword "YourStrong!Passw0rd" `
  -Location eastus
```

The script will:
1. Create the resource group (if it does not exist)
2. Deploy the Bicep template (SQL Server + Database)
3. Print the connection string
4. Optionally run EF migrations and seed data

### 3 — Manual EF commands

If you want to run the database steps separately:

```bash
export RETAILDB_CONNECTION_STRING="Server=tcp:<server>.database.windows.net,1433;..."

# Apply schema migrations only
dotnet run --project src/RetailDb -- migrate

# Seed retail data (idempotent — skips if data already exists)
dotnet run --project src/RetailDb -- seed

# Full setup (migrate + seed)
dotnet run --project src/RetailDb -- setup

# Drop the database (destructive!)
dotnet run --project src/RetailDb -- drop
```

### 4 — Adding your IP to the firewall (if needed)

```bash
MY_IP=$(curl -s https://api.ipify.org)
az sql server firewall-rule create \
  --resource-group rg-retail-lab \
  --server sql-retail-lab-<suffix> \
  --name "MyDev" \
  --start-ip-address "$MY_IP" \
  --end-ip-address "$MY_IP"
```

---

## Schema Migration Workflow

This project uses **EF Core code-first migrations**. To create a new migration after changing an entity model:

```bash
# From lab/src/RetailDb/
export RETAILDB_CONNECTION_STRING="..."
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

---

## Useful Sample Queries

### Top products by revenue
```sql
SELECT
    p.Name,
    SUM(oi.LineTotal) AS TotalRevenue,
    SUM(oi.Quantity)  AS UnitsSold
FROM OrderItem oi
JOIN Product p ON oi.ProductId = p.ProductId
GROUP BY p.Name
ORDER BY TotalRevenue DESC;
```

### Orders by store and status
```sql
SELECT
    s.StoreName,
    o.Status,
    COUNT(*)           AS OrderCount,
    SUM(o.TotalAmount) AS TotalSales
FROM [Order] o
JOIN Store s ON o.StoreId = s.StoreId
GROUP BY s.StoreName, o.Status
ORDER BY s.StoreName, o.Status;
```

### Low-stock products (below reorder level)
```sql
SELECT
    p.Sku,
    p.Name,
    s.StoreName,
    i.QuantityOnHand,
    p.ReorderLevel
FROM Inventory i
JOIN Product p ON i.ProductId = p.ProductId
JOIN Store   s ON i.StoreId   = s.StoreId
WHERE i.QuantityOnHand < p.ReorderLevel
ORDER BY i.QuantityOnHand ASC;
```

### Customer lifetime value
```sql
SELECT
    c.CustomerId,
    c.FirstName + ' ' + c.LastName AS CustomerName,
    COUNT(DISTINCT o.OrderId)      AS TotalOrders,
    SUM(o.TotalAmount)             AS LifetimeValue,
    AVG(o.TotalAmount)             AS AvgOrderValue
FROM Customer c
JOIN [Order] o ON c.CustomerId = o.CustomerId
WHERE o.Status = 'Delivered'
GROUP BY c.CustomerId, c.FirstName, c.LastName
ORDER BY LifetimeValue DESC;
```

### Employee hierarchy
```sql
SELECT
    e.FirstName + ' ' + e.LastName  AS Employee,
    e.JobTitle,
    e.Department,
    m.FirstName + ' ' + m.LastName  AS Manager,
    st.StoreName
FROM Employee e
LEFT JOIN Employee m ON e.ManagerId = m.EmployeeId
JOIN Store st ON e.StoreId = st.StoreId
ORDER BY st.StoreName, e.Department;
```

---

## Tear Down

```bash
az group delete --name rg-retail-lab --yes --no-wait
```

---

## Project Structure

```
lab/
├── infra/
│   ├── main.bicep          # Azure SQL Server + Database template
│   └── parameters.json     # Example parameters (reference Key Vault for password)
├── scripts/
│   ├── deploy.sh           # Bash deployment script
│   └── deploy.ps1          # PowerShell deployment script
└── src/
    └── RetailDb/
        ├── RetailDb.csproj
        ├── Program.cs              # CLI entry point (migrate / seed / setup / drop)
        ├── Data/
        │   ├── RetailDbContext.cs          # EF Core DbContext + Fluent API config
        │   └── RetailDbContextFactory.cs  # Design-time factory for CLI tools
        ├── Models/
        │   ├── Category.cs
        │   ├── Customer.cs
        │   ├── Employee.cs
        │   ├── Inventory.cs
        │   ├── Order.cs
        │   ├── OrderItem.cs
        │   ├── Product.cs
        │   ├── Review.cs
        │   ├── Store.cs
        │   └── Supplier.cs
        ├── Migrations/             # Auto-generated EF Core migrations
        └── Seed/
            └── DatabaseSeeder.cs  # Retail test data
```
