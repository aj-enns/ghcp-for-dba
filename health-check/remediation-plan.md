# Remediation Plan — RetailDb

**Report Date:** 2026-03-23  
**Database:** RetailDb (Standard S1, 20 DTUs)

---

## Prioritized Action Items

| Priority | Category | Issue | Action | Effort | Impact |
|----------|----------|-------|--------|--------|--------|
| 1 | Configuration | MAXDOP=8 too high for S1 | Reduce MAXDOP to 2 for S1 tier (limited CPU resources) | Low | Medium |
| 2 | Performance | OPTIMIZE_FOR_AD_HOC_WORKLOADS is OFF | Enable to reduce plan cache bloat from single-use ad hoc plans | Low | Medium |
| 3 | Security | Single db_owner user, no role separation | Create application-specific users with least-privilege roles | Medium | High |
| 4 | Security | Auditing status unknown | Enable Azure SQL Auditing via Azure Portal or Bicep | Low | High |
| 5 | Operations | No index maintenance scheduled | Implement index/statistics maintenance using Azure Automation or Elastic Jobs | Medium | Medium |
| 6 | Security | No stored procedures for data access | Consider wrapping common operations in stored procedures with EXECUTE permissions | High | Medium |

---

## Detailed Remediation Steps

### 1. Reduce MAXDOP (Priority: HIGH)

**Issue:** MAXDOP is set to 8, but the S1 tier (20 DTUs) has very limited CPU. High MAXDOP on a low-DTU tier can cause unnecessary parallelism overhead.

```sql
-- ============================================
-- Remediation: Reduce MAXDOP for S1 tier
-- Author: Health Check
-- Date: 2026-03-23
-- ============================================
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 2;
```

**Rollback:**
```sql
ALTER DATABASE SCOPED CONFIGURATION SET MAXDOP = 8;
```

---

### 2. Enable OPTIMIZE_FOR_AD_HOC_WORKLOADS (Priority: HIGH)

**Issue:** 487 ad hoc plans consuming 101 MB in plan cache. Many single-use plans waste memory.

```sql
-- ============================================
-- Remediation: Enable ad hoc workload optimization
-- Author: Health Check
-- Date: 2026-03-23
-- ============================================
ALTER DATABASE SCOPED CONFIGURATION SET OPTIMIZE_FOR_AD_HOC_WORKLOADS = ON;
```

**Rollback:**
```sql
ALTER DATABASE SCOPED CONFIGURATION SET OPTIMIZE_FOR_AD_HOC_WORKLOADS = OFF;
```

---

### 3. Create Application Users with Least-Privilege Roles (Priority: MEDIUM)

**Issue:** Only `dbo` (db_owner) exists. If an application connects as dbo, it has unrestricted access.

```sql
-- ============================================
-- Remediation: Create application role with least privilege
-- Author: Health Check
-- Date: 2026-03-23
-- ============================================

-- Create a custom role for application read/write access
CREATE ROLE [app_readwrite];

-- Grant appropriate permissions
GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO [app_readwrite];

-- Create a read-only role for reporting
CREATE ROLE [app_readonly];
GRANT SELECT ON SCHEMA::dbo TO [app_readonly];

-- Then create Azure AD users and assign roles:
-- CREATE USER [app-service-identity] FROM EXTERNAL PROVIDER;
-- ALTER ROLE [app_readwrite] ADD MEMBER [app-service-identity];
```

**Rollback:**
```sql
DROP ROLE IF EXISTS [app_readwrite];
DROP ROLE IF EXISTS [app_readonly];
```

---

### 4. Enable Azure SQL Auditing (Priority: MEDIUM)

**Issue:** Database auditing status could not be verified via T-SQL. Azure SQL Auditing should be enabled for compliance and security monitoring.

**Action:** Enable via Azure Portal:
1. Navigate to **SQL Server** > **Auditing**
2. Set **Enable Azure SQL Auditing** to ON
3. Choose storage destination (Storage Account, Log Analytics, or Event Hub)

Or via Azure CLI:
```bash
az sql server audit-policy update \
  --resource-group rg-retail-lab \
  --server sql-retail-lab-ug5lhmwwszwla \
  --state Enabled \
  --storage-account <storage-account-name>
```

Or add to the Bicep infrastructure template in `lab/infra/main.bicep`.

---

### 5. Implement Index Maintenance (Priority: LOW)

**Issue:** No scheduled index maintenance. As the database grows, fragmentation will increase.

**Action:** Use [Ola Hallengren's maintenance solution](https://ola.hallengren.com/) adapted for Azure SQL, or implement via Azure Automation Runbooks:

```sql
-- Example: Rebuild/reorganize indexes based on fragmentation
-- Run periodically via Azure Automation or Elastic Jobs

DECLARE @TableName NVARCHAR(256);
DECLARE @IndexName NVARCHAR(256);
DECLARE @Frag FLOAT;

DECLARE idx_cursor CURSOR FOR
SELECT
    OBJECT_NAME(ips.object_id),
    i.name,
    ips.avg_fragmentation_in_percent
FROM sys.dm_db_index_physical_stats(DB_ID(), NULL, NULL, NULL, 'LIMITED') ips
JOIN sys.indexes i ON ips.object_id = i.object_id AND ips.index_id = i.index_id
WHERE ips.page_count > 1000
  AND ips.avg_fragmentation_in_percent > 10;

OPEN idx_cursor;
FETCH NEXT FROM idx_cursor INTO @TableName, @IndexName, @Frag;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF @Frag > 30
        EXEC('ALTER INDEX [' + @IndexName + '] ON [' + @TableName + '] REBUILD');
    ELSE IF @Frag > 10
        EXEC('ALTER INDEX [' + @IndexName + '] ON [' + @TableName + '] REORGANIZE');
    
    FETCH NEXT FROM idx_cursor INTO @TableName, @IndexName, @Frag;
END;

CLOSE idx_cursor;
DEALLOCATE idx_cursor;
```

---

### 6. Consider Stored Procedures for Common Operations (Priority: LOW)

**Issue:** No stored procedures exist. The application uses ad hoc queries, which limits ability to enforce fine-grained EXECUTE permissions and increases plan cache pressure.

**Action:** Identify the most common data access patterns and wrap them in stored procedures. This enables:
- Fine-grained permission control (EXECUTE only, no direct table access)
- Better plan reuse
- Reduced SQL injection risk
- Easier performance monitoring via Query Store

This is a longer-term architectural improvement and should be planned as part of application development sprints.
