# High Availability & Disaster Recovery Skill

## Architecture Patterns

### Comparison Matrix
| Feature | Replication | Clustering | AlwaysOn AG | Log Shipping |
|---------|:----------:|:----------:|:-----------:|:------------:|
| Automatic failover | Depends | ✅ | ✅ | ❌ |
| Read replicas | ✅ | ❌ | ✅ | ✅ (standby) |
| Zero data loss possible | Sync only | ✅ | Sync mode | ❌ |
| Cross-region | ✅ | ❌ | ✅ | ✅ |
| Granularity | Database | Instance | Database group | Database |

## SQL Server AlwaysOn Availability Groups

### Configuration Checklist
- [ ] Windows Server Failover Cluster (WSFC) configured
- [ ] All nodes on same Active Directory domain
- [ ] Endpoint port (5022) open between nodes
- [ ] Service accounts have connect permissions
- [ ] Databases in FULL recovery model
- [ ] Full backup taken before adding to AG

### Key Settings
```sql
-- Check AG health
SELECT
    ag.name AS ag_name,
    ars.role_desc,
    ars.operational_state_desc,
    ars.synchronization_health_desc,
    ar.availability_mode_desc,
    ar.failover_mode_desc
FROM sys.dm_hadr_availability_replica_states ars
JOIN sys.availability_replicas ar ON ars.replica_id = ar.replica_id
JOIN sys.availability_groups ag ON ar.group_id = ag.group_id;

-- Check database synchronization
SELECT
    db_name(database_id) AS database_name,
    synchronization_state_desc,
    synchronization_health_desc,
    log_send_queue_size,
    redo_queue_size
FROM sys.dm_hadr_database_replica_states;
```

### Failover Decision Framework
| Scenario | Action | Data Loss Risk |
|----------|--------|:--------------:|
| Planned maintenance | Manual failover (no data loss) | None |
| Primary unresponsive, sync replica available | Automatic/manual failover | None |
| Primary unresponsive, only async replicas | Forced failover (last resort) | Possible |

## PostgreSQL Streaming Replication

### Setup Pattern
```
Primary → Synchronous Replica (same DC) → Async Replica (DR site)
```

### Key Configuration (Primary)
```
# postgresql.conf
wal_level = replica
max_wal_senders = 5
synchronous_standby_names = 'replica1'
synchronous_commit = on        # for sync replica
wal_keep_size = 1GB            # or use replication slots
```

### Monitoring Replication Lag
```sql
-- On primary: check replication status
SELECT client_addr, state, sent_lsn, write_lsn, flush_lsn, replay_lsn,
       pg_wal_lsn_diff(sent_lsn, replay_lsn) AS replay_lag_bytes
FROM pg_stat_replication;

-- On replica: check replay status
SELECT now() - pg_last_xact_replay_timestamp() AS replication_delay;
```

### Failover Tools
| Tool | Type | Use Case |
|------|------|----------|
| `pg_promote` | Built-in | Manual promotion of standby (PG 12+) |
| Patroni | External | Automated HA with consensus (etcd/ZooKeeper) |
| repmgr | External | Replication management and monitoring |
| PgBouncer | Connection router | Query routing to primary/replica |

## MySQL Replication

### Group Replication / InnoDB Cluster
- Use for multi-primary or single-primary HA
- Built-in conflict detection for multi-primary
- Requires GTID-based replication

### Monitoring
```sql
-- Check replica status
SHOW REPLICA STATUS\G

-- Key fields to monitor:
-- Seconds_Behind_Source (lag)
-- Replica_IO_Running / Replica_SQL_Running (both should be "Yes")
-- Last_Error (should be empty)
```

## Disaster Recovery Planning

### RPO/RTO Guide
| Tier | RPO | RTO | Solution |
|:----:|:---:|:---:|----------|
| 1 | 0 (zero data loss) | < 1 min | Synchronous replication + auto failover |
| 2 | < 5 min | < 15 min | Async replication + manual failover |
| 3 | < 1 hour | < 4 hours | Log shipping + standby server |
| 4 | < 24 hours | < 24 hours | Regular backups + restore |

### DR Testing Checklist
- [ ] Quarterly failover drill to DR site
- [ ] Verify application connectivity after failover
- [ ] Measure actual RTO against target
- [ ] Validate data integrity after failover
- [ ] Test failback procedure
- [ ] Document any issues and update runbook
- [ ] Verify monitoring alerts fire correctly during failover
