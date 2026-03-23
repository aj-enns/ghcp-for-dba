#!/usr/bin/env bash
# =============================================================================
# deploy.sh — Deploy the Retail Lab Azure SQL database
#
# Usage:
#   ./scripts/deploy.sh --resource-group <rg-name>
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
LOCATION="eastus"
SUBSCRIPTION=""
DEPLOYMENT_NAME="retail-lab-$(date +%Y%m%d%H%M%S)"

# ── Parse args ────────────────────────────────────────────────────────────────
while [[ $# -gt 0 ]]; do
    case "$1" in
        --resource-group|-g) RESOURCE_GROUP="$2"; shift 2 ;;
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

# ── Set subscription if provided ──────────────────────────────────────────────
if [[ -n "$SUBSCRIPTION" ]]; then
    az account set --subscription "$SUBSCRIPTION"
fi

# ── Get current Azure AD user info ────────────────────────────────────────────
echo "🔑 Retrieving Azure AD identity..."
AAD_ADMIN_LOGIN=$(az ad signed-in-user show --query userPrincipalName --output tsv)
AAD_ADMIN_OBJECT_ID=$(az ad signed-in-user show --query id --output tsv)
AAD_TENANT_ID=$(az account show --query tenantId --output tsv)
echo "   Admin : $AAD_ADMIN_LOGIN"
echo "   Tenant: $AAD_TENANT_ID"

# ── Ensure resource group exists ──────────────────────────────────────────────
echo "🔧 Ensuring resource group '$RESOURCE_GROUP' exists in '$LOCATION'..."
az group create --name "$RESOURCE_GROUP" --location "$LOCATION" --output none

# ── Deploy Bicep template ─────────────────────────────────────────────────────
echo "🚀 Deploying Bicep template..."
DEPLOY_OUTPUT=$(az deployment group create \
    --name "$DEPLOYMENT_NAME" \
    --resource-group "$RESOURCE_GROUP" \
    --template-file "$BICEP_FILE" \
    --parameters location="$LOCATION" \
                 aadAdminLogin="$AAD_ADMIN_LOGIN" \
                 aadAdminObjectId="$AAD_ADMIN_OBJECT_ID" \
                 aadAdminTenantId="$AAD_TENANT_ID" \
    --output json)

if [[ $? -ne 0 ]]; then
    echo "❌ Bicep deployment failed."
    exit 1
fi

# ── Extract outputs ───────────────────────────────────────────────────────────
SQL_SERVER_FQDN=$(echo "$DEPLOY_OUTPUT" | python3 -c "import sys,json; print(json.load(sys.stdin)['properties']['outputs']['sqlServerFqdn']['value'])")
DB_NAME=$(echo "$DEPLOY_OUTPUT" | python3 -c "import sys,json; print(json.load(sys.stdin)['properties']['outputs']['databaseName']['value'])")

if [[ -z "$SQL_SERVER_FQDN" || -z "$DB_NAME" ]]; then
    echo "❌ Deployment outputs are missing. Check the Azure deployment log."
    exit 1
fi

echo ""
echo "✅ Infrastructure deployed successfully!"
echo "   SQL Server : $SQL_SERVER_FQDN"
echo "   Database   : $DB_NAME"
echo ""

# ── Build connection string (Azure AD) ────────────────────────────────────────
CONNECTION_STRING="Server=tcp:${SQL_SERVER_FQDN},1433;Initial Catalog=${DB_NAME};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
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
