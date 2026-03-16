# Tip: Database Object Naming Conventions

Consistent naming reduces cognitive load and makes schemas self-documenting. Use custom instructions to enforce your naming standard automatically.

## The Pattern

Add naming rules to your `.github/copilot-instructions.md` so Copilot follows them in every interaction:

```markdown
## Naming Conventions
- Tables: PascalCase singular (Customer, OrderItem)
- Columns: PascalCase (FirstName, OrderDate)
- Indexes: IX_TableName_Column1_Column2
- Foreign keys: FK_ChildTable_ParentTable
- Stored procedures: usp_VerbNoun
```

## Why It Matters

| What happens without conventions | What happens with conventions |
|--------------------------------|-------------------------------|
| `tbl_customer_orders` mixed with `OrderDetail` | Consistent `OrderItem` across the schema |
| `idx1`, `idx2` index names | `IX_Order_CustomerId_OrderDate` tells you what it's for |
| `GetData`, `proc_fetch_stuff` | `usp_GetCustomerOrders` is predictable |
| Debugging time spent decoding names | Names are self-documenting |

## Common Naming Patterns

### Table Names
| Pattern | Example | When to Use |
|---------|---------|-------------|
| PascalCase singular | `Customer` | Most common, recommended |
| PascalCase plural | `Customers` | Popular in ORMs (Entity Framework, Rails) |
| snake_case | `customer_order` | Common in PostgreSQL ecosystem |

Pick one and be consistent. The standard matters less than consistency.

### Column Names
| Column Type | Pattern | Examples |
|-------------|---------|----------|
| Primary key | `Id` or `TableNameId` | `Id`, `CustomerId` |
| Foreign key | `ReferencedTableId` | `CustomerId`, `OrderId` |
| Boolean | `Is/Has/Can` prefix | `IsActive`, `HasDiscount` |
| Date/time | Descriptive suffix | `CreatedAt`, `OrderDate`, `ExpiresOn` |
| Amount/money | Descriptive noun | `TotalAmount`, `UnitPrice`, `DiscountRate` |
| Status/type | `Status` or `Type` suffix | `OrderStatus`, `AccountType` |

### Constraint & Index Names
| Object | Pattern | Example |
|--------|---------|---------|
| Primary key | `PK_TableName` | `PK_Customer` |
| Foreign key | `FK_ChildTable_ParentTable` | `FK_Order_Customer` |
| Unique | `UQ_TableName_Column` | `UQ_Customer_Email` |
| Check | `CK_TableName_Rule` | `CK_Order_TotalPositive` |
| Default | `DF_TableName_Column` | `DF_Order_OrderDate` |
| Index | `IX_TableName_Column(s)` | `IX_Order_CustomerId_OrderDate` |

## Applying with Copilot

Add the naming conventions to your custom instructions, then:

1. **New objects** — Copilot generates names following your convention automatically
2. **Existing objects** — ask Copilot to "review this schema for naming convention violations"
3. **Migrations** — Copilot generates rename scripts to bring legacy objects in line
