---
mode: agent
description: Generates capacity planning reports with growth projections and scaling recommendations
---

# Capacity Planning

Analyze current database resource usage and project future capacity needs.

## Step 1 — Collect Current Usage Metrics

Gather current resource utilization data:

- **Storage** — database sizes, growth trends (last 6-12 months), file group utilization
- **Compute** — CPU utilization patterns (peak, average, off-hours)
- **Memory** — buffer pool usage, page life expectancy trends, memory grants
- **I/O** — read/write throughput, latency percentiles (p50, p95, p99)
- **Connections** — peak concurrent connections, connection pool utilization

Document current state in `capacity-planning/current-usage.md`.

## Step 2 — Analyze Growth Patterns

Project future growth based on historical trends:

| Resource | Current | Monthly Growth | 6-Month Projection | 12-Month Projection |
|----------|---------|---------------|--------------------|--------------------|
| Database Size | 500 GB | +15 GB/month | 590 GB | 680 GB |
| Peak CPU | 65% | +2%/month | 77% | 89% |

Consider factors that affect projections:
- Business growth plans (new features, customer acquisition)
- Seasonal patterns (holiday spikes, fiscal year-end)
- Planned data retention changes
- Expected schema changes

Save analysis to `capacity-planning/growth-analysis.md`.

## Step 3 — Identify Capacity Risks

Flag resources approaching critical thresholds:

| Risk Level | Resource | Current | Threshold | Time to Threshold |
|-----------|----------|---------|-----------|-------------------|
| 🔴 Critical | Disk Space | 85% | 90% | ~2 months |
| 🟠 High | CPU | 70% peak | 80% | ~5 months |

Save to `capacity-planning/risk-assessment.md`.

## Step 4 — Generate Scaling Recommendations

Provide scaling options in `capacity-planning/scaling-plan.md`:

- **Vertical scaling** — upgrade hardware specs, cost implications
- **Horizontal scaling** — read replicas, sharding strategies, connection routing
- **Optimization** — archiving old data, partitioning, query tuning to reduce resource demand
- **Cloud migration** — if applicable, compare on-prem vs cloud costs at projected scale
- **Timeline** — recommended action dates based on growth projections
