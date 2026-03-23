# =============================================================================
# deploy.ps1 — Deploy the Retail Lab Azure SQL database (PowerShell version)
#
# Usage:
#   .\scripts\deploy.ps1 -ResourceGroup <rg-name>
#                        [-Location eastus] [-Subscription <id>]
#
# Prerequisites:
#   - Azure CLI (az) installed and logged in
#   - Bicep CLI (included with az CLI >= 2.20)
#   - .NET 8 SDK
# =============================================================================
[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$ResourceGroup,
    [string]$Location     = "eastus",
    [string]$Subscription = ""
)

$ErrorActionPreference = "Stop"

$ScriptDir   = Split-Path -Parent $MyInvocation.MyCommand.Path
$RepoRoot    = Split-Path -Parent $ScriptDir
$BicepFile   = Join-Path $RepoRoot "infra\main.bicep"
$ProjectFile = Join-Path $RepoRoot "src\RetailDb\RetailDb.csproj"
$DeployName  = "retail-lab-$(Get-Date -Format 'yyyyMMddHHmmss')"

# ── Set subscription ──────────────────────────────────────────────────────────
if ($Subscription) {
    Write-Host "Setting subscription to $Subscription..."
    az account set --subscription $Subscription
}

# ── Get current Azure AD user info ────────────────────────────────────────────
Write-Host "🔑 Retrieving Azure AD identity..."
$AadUser = az ad signed-in-user show --output json | ConvertFrom-Json
$AadAdminLogin    = $AadUser.userPrincipalName
$AadAdminObjectId = $AadUser.id
$AadTenantId      = (az account show --output json | ConvertFrom-Json).tenantId

Write-Host "   Admin : $AadAdminLogin"
Write-Host "   Tenant: $AadTenantId"

# ── Ensure resource group ─────────────────────────────────────────────────────
Write-Host "🔧 Ensuring resource group '$ResourceGroup' exists in '$Location'..."
az group create --name $ResourceGroup --location $Location --output none

# ── Deploy Bicep ──────────────────────────────────────────────────────────────
Write-Host "🚀 Deploying Bicep template..."
$PrevErrorPref = $ErrorActionPreference
$ErrorActionPreference = "Continue"
$DeployOutput = az deployment group create `
    --name $DeployName `
    --resource-group $ResourceGroup `
    --template-file $BicepFile `
    --parameters location=$Location `
                 aadAdminLogin=$AadAdminLogin `
                 aadAdminObjectId=$AadAdminObjectId `
                 aadAdminTenantId=$AadTenantId `
    --output json
$DeployExitCode = $LASTEXITCODE
$ErrorActionPreference = $PrevErrorPref

if ($DeployExitCode -ne 0) {
    Write-Error "❌ Bicep deployment failed:`n$DeployOutput"
    exit 1
}

$DeployJson    = $DeployOutput | ConvertFrom-Json
$SqlServerFqdn = $DeployJson.properties.outputs.sqlServerFqdn.value
$DbName        = $DeployJson.properties.outputs.databaseName.value

if (-not $SqlServerFqdn -or -not $DbName) {
    Write-Error "❌ Deployment outputs are missing. Check the Azure deployment log."
    exit 1
}

Write-Host ""
Write-Host "✅ Infrastructure deployed successfully!"
Write-Host "   SQL Server : $SqlServerFqdn"
Write-Host "   Database   : $DbName"
Write-Host ""

# ── Connection string (Azure AD) ─────────────────────────────────────────────
$ConnectionString = "Server=tcp:${SqlServerFqdn},1433;Initial Catalog=${DbName};Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;"
Write-Host "📋 Connection string (store this securely):"
Write-Host "   $ConnectionString"
Write-Host ""

# ── Run EF migrations + seed ──────────────────────────────────────────────────
$RunSetup = Read-Host "Run EF migrations and seed data now? [y/N]"
if ($RunSetup -ieq "y") {
    $env:RETAILDB_CONNECTION_STRING = $ConnectionString
    Write-Host "⚙️  Running migrations and seeding data..."
    dotnet run --project $ProjectFile -- setup
}

Write-Host ""
Write-Host "🎉 Lab environment ready!"
