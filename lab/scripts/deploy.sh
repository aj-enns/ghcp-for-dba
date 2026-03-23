#!/usr/bin/env bash
# =============================================================================
# deploy.sh — Deploy the Retail Lab Azure SQL database
#
# Usage:
#   ./scripts/deploy.sh --resource-group <rg-name> --password <sql-password>
#                       [--location eastus] [--subscription <id>]
#
# Prerequisites:
#   - Azure CLI (az) installed and logged in
#   - Bicep CLI (included with az CLI >= 2.20)
# =============================================================================
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/.." && pwd)"
BICEP_FILE="$REPO_ROOT/infra/main.bicep"

# ── Defaults ──────────────────────────────────────────────────────────────────
RESOURCE_GROUP=""
SQL_PASSWORD=""
LOCATION="eastus"
SUBSCRIPTION=""
DEPLOYMENT_NAME="retail-lab-$(date +%Y%m%d%H%M%S)"

# ── Parse args ────────────────────────────────────────────────────────────────
while [[ $# -gt 0 ]]; do
    case "$1" in
        --resource-group|-g) RESOURCE_GROUP="$2"; shift 2 ;;
        --password|-p)       SQL_PASSWORD="$2";   shift 2 ;;
        --location|-l)       LOCATION="$2";       shift 2 ;;
        --subscription|-s)   SUBSCRIPTION="$2";   shift 2 ;;
        *) echo "Unknown argument: $1"; exit 1 ;;
    esac
done

# ── Validate required args ────────────────────────────────────────────────────
if [[ -z "$RESOURCE_GROUP" ]]; then
    echo "❌ --resource-group is required"
    exit 1
fi
if [[ -z "$SQL_PASSWORD" ]]; then
    echo "❌ --password is required"
    exit 1
fi

# ── Set subscription if provided ──────────────────────────────────────────────
if [[ -n "$SUBSCRIPTION" ]]; then
    az account set --subscription "$SUBSCRIPTION"
fi

# ── Ensure resource group exists ──────────────────────────────────────────────
echo "🔧 Ensuring resource group '$RESOURCE_GROUP' exists in '$LOCATION'..."
az group create --name "$RESOURCE_GROUP" --location "$LOCATION" --output none

# ── Deploy Bicep template ─────────────────────────────────────────────────────
echo "🚀 Deploying Bicep template..."
DEPLOY_OUTPUT=$(az deployment group create \
    --name "$DEPLOYMENT_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --template-file "$BICEP_FILE" \
    --parameters sqlAdminPassword="$SQL_PASSWORD" location="$LOCATION" \
    --output json)

# ── Extract outputs ───────────────────────────────────────────────────────────
SQL_SERVER_FQDN=$(echo "$DEPLOY_OUTPUT" | python3 -c "import sys,json; print(json.load(sys.stdin)['properties']['outputs']['sqlServerFqdn']['value'])")
DB_NAME=$(echo "$DEPLOY_OUTPUT" | python3 -c "import sys,json; print(json.load(sys.stdin)['properties']['outputs']['databaseName']['value'])")
SQL_ADMIN="retailadmin"

echo ""
echo "✅ Infrastructure deployed successfully!"
echo "   SQL Server : $SQL_SERVER_FQDN"
echo "   Database   : $DB_NAME"
echo ""

# ── Build connection string ───────────────────────────────────────────────────
CONNECTION_STRING="Server=tcp:${SQL_SERVER_FQDN},1433;Initial Catalog=${DB_NAME};Persist Security Info=False;User ID=${SQL_ADMIN};Password=${SQL_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
echo "📋 Connection string (store this securely):"
echo "   $CONNECTION_STRING"
echo ""

# ── Optionally run EF migrations + seed ──────────────────────────────────────
read -rp "Run EF migrations and seed data now? [y/N] " RUN_SETUP
if [[ "${RUN_SETUP,,}" == "y" ]]; then
    export RETAILDB_CONNECTION_STRING="$CONNECTION_STRING"
    DOTNET_PROJECT="$REPO_ROOT/src/RetailDb/RetailDb.csproj"
    echo "⚙️  Running migrations and seeding data..."
    dotnet run --project "$DOTNET_PROJECT" -- setup
fi

echo ""
echo "🎉 Lab environment ready!"
