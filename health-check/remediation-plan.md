# Remediation Plan — RetailDb Health Check

**Report Date:** 2026-03-23  
**Database:** RetailDb @ sql-retail-lab-ug5lhmwwszwla.database.windows.net

---

## Prioritized Action Items

| Priority | Category | Issue | Action | Effort | Impact |
|----------|----------|-------|--------|--------|--------|
| 1 | Security | Auditing is disabled | Enable Azure SQL auditing to Log Analytics or Storage Account | Low | High |
| 2 | Security | Advanced Threat Protection disabled | Enable Defender for SQL on the server | Low | High |
| 3 | Backup | No long-term retention | Configure LTR policy (weekly/monthly/yearly) | Low | High |
| 4 | Backup | Only 7-day PITR retention | Increase to at least 14 days | Low | Medium |
| 5 | Security | No least-privilege roles | Create app-specific DB roles with minimal permissions | Medium | High |
| 6 | Security | AllowAzureServices firewall rule | Replace with specific IP rules or private endpoints | Medium | High |
| 7 | Backup | Local-only backup redundancy | Switch to Geo-redundant backup storage | Low | Medium |
| 8 | Integrity | No CHECK constraints | Add data validation constraints (Rating, Quantity, etc.) | Low | Medium |
| 9 | Configuration | Optimize for Ad Hoc OFF | Enable to reduce plan cache bloat | Low | Low |
| 10 | Security | No alert email configured | Set security contact email for Defender alerts | Low | Medium |

---

## Detailed Remediation Steps

### Priority 1 — Enable Auditing

```bash
# Enable auditing to a storage account
az sql server audit-policy update \
  --server sql-retail-lab-ug5lhmwwszwla \
  --resource-group rg-retail-lab \
  --state Enabled \
  --storage-account <storage-account-name> \
  --retention-days 90
```

**Why:** Without auditing, there is no record of data access, schema changes, or failed login attempts. Required for compliance (SOC2, HIPAA, PCI-DSS).

### Priority 2 — Enable Defender for SQL

```bash
# Enable Advanced Threat Protection
az sql db threat-policy update \
  --server sql-retail-lab-ug5lhmwwszwla \
  --resource-group rg-retail-lab \
  --name RetailDb \
  --state Enabled \
  --email-addresses "dba-alerts@yourcompany.com" \
  --email-account-admins Enabled
```

**Why:** Detects SQL injection attempts, brute-force attacks, anomalous database activity, and data exfiltration patterns.

### Priority 3 — Configure Long-Term Retention

```bash
# Set LTR: 4 weeks, 12 months, 3 years
az sql db ltr-policy set \
  --server sql-retail-lab-ug5lhmwwszwla \
  --resource-group rg-retail-lab \
  --name RetailDb \
  --weekly-retention P4W \
  --monthly-retention P12M \
  --yearly-retention P3Y \
  --week-of-year 1
```

**Why:** Without LTR, data cannot be restored beyond the 7-day PITR window. Critical for compliance and disaster recovery.

### Priority 4 — Increase PITR Retention

```bash
# Increase to 14 days
az sql db str-policy set \
  --server sql-retail-lab-ug5lhmwwszwla \
  --resource-group rg-retail-lab \
  --name RetailDb \
  --retention-days 14 \
  --diffbackup-hours 12
```

**Why:** 7 days is the minimum default. A 14-day window provides more recovery flexibility for accidental deletions or corruption discovered days later.

### Priority 5 — Create Least-Privilege Database Roles

```sql
-- ============================================
-- Migration: Create application database roles
-- Author: DBA Team
-- Date: 2026-03-23
-- Ticket: HEALTH-CHECK-001
-- ============================================

BEGIN TRANSACTION;
BEGIN TRY
    -- Read-only role for reporting
    CREATE ROLE [AppReader];
    GRANT SELECT ON SCHEMA::dbo TO [AppReader];

    -- Read-write role for application
    CREATE ROLE [AppWriter];
    GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [AppWriter];

    -- Admin role for migrations (no db_owner)
    CREATE ROLE [AppMigrator];
    GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [AppMigrator];
    GRANT CREATE TABLE, ALTER, REFERENCES TO [AppMigrator];

    COMMIT TRANSACTION;
    PRINT 'Roles created successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH
```

**Why:** Currently only `dbo` (owner) access exists. Applications should use the least-privilege role needed.

### Priority 6 — Restrict Firewall Access

```bash
# Remove broad Azure Services rule and add specific IPs
az sql server firewall-rule delete \
  --server sql-retail-lab-ug5lhmwwszwla \
  --resource-group rg-retail-lab \
  --name AllowAzureServices

# Add specific client IP
az sql server firewall-rule create \
  --server sql-retail-lab-ug5lhmwwszwla \
  --resource-group rg-retail-lab \
  --name MyWorkstation \
  --start-ip-address <your-ip> \
  --end-ip-address <your-ip>

# For production: consider private endpoints instead
```

**Why:** `AllowAzureServices (0.0.0.0)` permits any Azure service (including other tenants) to reach the server. Private endpoints or specific IP rules are more secure.

### Priority 7 — Switch to Geo-Redundant Backup

```bash
az sql db update \
  --server sql-retail-lab-ug5lhmwwszwla \
  --resource-group rg-retail-lab \
  --name RetailDb \
  --backup-storage-redundancy Geo
```

**Why:** Local backup redundancy only protects against storage failures within the same region. Geo-redundancy protects against regional outages.

### Priority 8 — Add CHECK Constraints

```sql
-- ============================================
-- Migration: Add CHECK constraints for data integrity
-- Author: DBA Team
-- Date: 2026-03-23
-- Ticket: HEALTH-CHECK-002
-- ============================================

BEGIN TRANSACTION;
BEGIN TRY
    ALTER TABLE dbo.Reviews
        ADD CONSTRAINT CK_Review_Rating CHECK (Rating BETWEEN 1 AND 5);

    ALTER TABLE dbo.OrderItems
        ADD CONSTRAINT CK_OrderItem_Quantity CHECK (Quantity > 0);

    ALTER TABLE dbo.OrderItems
        ADD CONSTRAINT CK_OrderItem_UnitPrice CHECK (UnitPrice >= 0);

    ALTER TABLE dbo.Products
        ADD CONSTRAINT CK_Product_UnitPrice CHECK (UnitPrice >= 0);

    ALTER TABLE dbo.Products
        ADD CONSTRAINT CK_Product_ReorderLevel CHECK (ReorderLevel >= 0);

    ALTER TABLE dbo.Inventories
        ADD CONSTRAINT CK_Inventory_QuantityOnHand CHECK (QuantityOnHand >= 0);

    ALTER TABLE dbo.Employees
        ADD CONSTRAINT CK_Employee_Salary CHECK (Salary >= 0);

    COMMIT TRANSACTION;
    PRINT 'CHECK constraints added successfully.';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    THROW;
END CATCH
```

### Priority 9 — Enable Optimize for Ad Hoc Workloads

```sql
ALTER DATABASE SCOPED CONFIGURATION SET OPTIMIZE_FOR_AD_HOC_WORKLOADS = ON;
```

**Why:** Prevents single-use query plans from consuming plan cache memory. Stores a plan stub on first execution and only caches the full plan on second execution.

### Priority 10 — Configure Security Alert Email

Configure at least one email address to receive security alerts when Defender for SQL detects anomalous activity (see Priority 2 command above, `--email-addresses` parameter).
