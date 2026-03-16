# Tip: Safe Database Deployments

A prompting pattern that makes every schema change safer with pre-checks, transactions, and rollback scripts.

## The Pattern

Before deploying any DDL or data change, ask Copilot:

> "Wrap this migration in a safe deployment pattern: add pre-checks to verify current state, wrap in a transaction with error handling, add post-checks to verify the change, and generate a rollback script."

## Why It Works

Most database incidents during deployments happen because of:
- Running a migration against the wrong database or state
- No way to reverse a change when something goes wrong
- No verification that the change actually worked

This pattern addresses all three.

## The Safe Deployment Template

```sql
-- ============================================
-- Migration: Add EmailVerified column to Customer
-- Author: [name]
-- Date: [date]
-- Ticket: [reference]
-- ============================================

-- PRE-CHECK: Verify current state
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
               WHERE TABLE_NAME = 'Customer' AND COLUMN_NAME = 'EmailVerified')
BEGIN
    PRINT 'Pre-check passed: Column does not exist yet.';

    BEGIN TRANSACTION;
    BEGIN TRY
        ALTER TABLE dbo.Customer
        ADD EmailVerified BIT NOT NULL
        CONSTRAINT DF_Customer_EmailVerified DEFAULT (0);

        -- POST-CHECK: Verify the change
        IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
                   WHERE TABLE_NAME = 'Customer' AND COLUMN_NAME = 'EmailVerified')
        BEGIN
            COMMIT TRANSACTION;
            PRINT 'Migration completed successfully.';
        END
        ELSE
        BEGIN
            ROLLBACK TRANSACTION;
            PRINT 'Post-check failed: Column was not created.';
        END
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
ELSE
BEGIN
    PRINT 'Skipped: Column already exists (migration already applied).';
END
```

## Rollback Script

```sql
-- ROLLBACK: Remove EmailVerified column
-- Only run if the forward migration needs to be reversed
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
           WHERE TABLE_NAME = 'Customer' AND COLUMN_NAME = 'EmailVerified')
BEGIN
    ALTER TABLE dbo.Customer
    DROP CONSTRAINT IF EXISTS DF_Customer_EmailVerified;

    ALTER TABLE dbo.Customer
    DROP COLUMN EmailVerified;

    PRINT 'Rollback completed: EmailVerified column removed.';
END
ELSE
BEGIN
    PRINT 'Skipped: Column does not exist (nothing to rollback).';
END
```

## Key Principles

| Principle | What It Means |
|-----------|--------------|
| **Idempotent** | Safe to run multiple times — checks if change already applied |
| **Transactional** | All-or-nothing — partial changes are rolled back on error |
| **Verified** | Pre-checks and post-checks prove the migration worked |
| **Reversible** | Rollback script paired with every forward migration |
| **Documented** | Header block with who, when, why, and ticket reference |

## Variations

| Prompt variation | When to use |
|-----------------|-------------|
| "...and add a backup of affected data before the change" | When modifying or deleting data |
| "...and estimate the lock duration" | When changing large tables |
| "...make it online-compatible" | When zero-downtime is required |
| "...and generate a test script I can run in dev first" | When the change is high-risk |
