# Security Hardening Skill

## Principle of Least Privilege

### Role-Based Access Patterns
```sql
-- SQL Server: Application-specific role
CREATE ROLE app_reader;
GRANT SELECT ON SCHEMA::dbo TO app_reader;

CREATE ROLE app_writer;
GRANT SELECT, INSERT, UPDATE ON SCHEMA::dbo TO app_writer;
GRANT EXECUTE ON SCHEMA::dbo TO app_writer;

-- Create application login with specific role
CREATE LOGIN app_svc WITH PASSWORD = '<strong-password>';
CREATE USER app_svc FOR LOGIN app_svc;
ALTER ROLE app_writer ADD MEMBER app_svc;

-- PostgreSQL: equivalent pattern
CREATE ROLE app_reader;
GRANT USAGE ON SCHEMA public TO app_reader;
GRANT SELECT ON ALL TABLES IN SCHEMA public TO app_reader;
ALTER DEFAULT PRIVILEGES IN SCHEMA public GRANT SELECT ON TABLES TO app_reader;

CREATE ROLE app_writer;
GRANT USAGE ON SCHEMA public TO app_writer;
GRANT SELECT, INSERT, UPDATE ON ALL TABLES IN SCHEMA public TO app_writer;
```

### Separation of Duties
| Role | Permissions | Use Case |
|------|------------|----------|
| `app_reader` | SELECT only | Reporting, read APIs |
| `app_writer` | SELECT, INSERT, UPDATE, EXECUTE | Application CRUD |
| `app_admin` | DDL on specific schemas | Migration runner |
| `dba_ops` | Server-level monitoring | Monitoring tools |
| `dba_admin` | Full admin | DBA team only |

### Anti-Patterns to Avoid
- ❌ Application accounts with `db_owner` / `SUPERUSER`
- ❌ Shared `sa` / `postgres` accounts for applications
- ❌ `GRANT ALL` on databases or schemas
- ❌ Service accounts with `sysadmin` or `CREATEDB` privileges
- ❌ Direct table access when stored procedures would suffice

## SQL Injection Prevention

### Always Use Parameterized Queries
```sql
-- ❌ NEVER: string concatenation
EXEC('SELECT * FROM users WHERE name = ''' + @name + '''');

-- ✅ ALWAYS: parameterized
EXEC sp_executesql N'SELECT * FROM users WHERE name = @name', N'@name NVARCHAR(100)', @name;
```

### Dynamic SQL Safety
```sql
-- ❌ Dangerous: user input in dynamic SQL
SET @sql = 'SELECT * FROM ' + @tableName;

-- ✅ Safe: validate against whitelist
IF @tableName NOT IN ('orders', 'customers', 'products')
    THROW 50001, 'Invalid table name', 1;

-- ✅ Safe: use QUOTENAME for identifiers
SET @sql = 'SELECT * FROM ' + QUOTENAME(@tableName);
```

## Encryption

### Data at Rest
| Method | Use Case | RDBMS |
|--------|----------|-------|
| TDE (Transparent Data Encryption) | Full database encryption | SQL Server, PostgreSQL (pgcrypto), MySQL |
| Column-level encryption | Specific sensitive columns | SQL Server (Always Encrypted) |
| Application-level encryption | Maximum control, key management | Any |

### Data in Transit
- Enable TLS/SSL for all database connections
- Use certificate-based authentication where possible
- Enforce minimum TLS 1.2

### SQL Server TDE Setup
```sql
-- Create master key
CREATE MASTER KEY ENCRYPTION BY PASSWORD = '<strong-password>';

-- Create certificate
CREATE CERTIFICATE TDECert WITH SUBJECT = 'TDE Certificate';

-- Create database encryption key
USE TargetDatabase;
CREATE DATABASE ENCRYPTION KEY
WITH ALGORITHM = AES_256
ENCRYPTION BY SERVER CERTIFICATE TDECert;

-- Enable encryption
ALTER DATABASE TargetDatabase SET ENCRYPTION ON;

-- CRITICAL: Back up the certificate immediately
BACKUP CERTIFICATE TDECert TO FILE = 'C:\Secure\TDECert.cer'
WITH PRIVATE KEY (FILE = 'C:\Secure\TDECert.pvk',
ENCRYPTION BY PASSWORD = '<backup-password>');
```

## Audit Logging

### What to Audit
| Event | Priority | Purpose |
|-------|:--------:|---------|
| Failed logins | 🔴 | Detect brute force attacks |
| Schema changes (DDL) | 🔴 | Track unauthorized modifications |
| Permission changes (GRANT/REVOKE) | 🔴 | Detect privilege escalation |
| Data changes on sensitive tables | 🟠 | Compliance, forensics |
| Successful logins | 🟡 | Access pattern analysis |
| SELECT on sensitive data | 🟡 | Data access monitoring |

### SQL Server Audit
```sql
-- Create server audit
CREATE SERVER AUDIT SecurityAudit
TO FILE (FILEPATH = 'C:\AuditLogs\', MAXSIZE = 100 MB, MAX_ROLLOVER_FILES = 10);

-- Create database audit specification
CREATE DATABASE AUDIT SPECIFICATION DBAudit
FOR SERVER AUDIT SecurityAudit
ADD (SCHEMA_OBJECT_CHANGE_GROUP),
ADD (DATABASE_PERMISSION_CHANGE_GROUP),
ADD (SELECT ON dbo.sensitive_table BY public);

ALTER SERVER AUDIT SecurityAudit WITH (STATE = ON);
ALTER DATABASE AUDIT SPECIFICATION DBAudit WITH (STATE = ON);
```

## Compliance Checklist

### Common Requirements Across Standards
- [ ] All connections encrypted (TLS 1.2+)
- [ ] No shared accounts — individual credentials per person/service
- [ ] Password policy enforced (complexity, rotation, history)
- [ ] Failed login lockout after N attempts
- [ ] Audit logging enabled for privileged operations
- [ ] Data at rest encrypted for sensitive information
- [ ] Regular access reviews (quarterly minimum)
- [ ] Backup encryption enabled
- [ ] Network isolation (private subnets, firewall rules)
- [ ] Vulnerability scanning on a schedule
