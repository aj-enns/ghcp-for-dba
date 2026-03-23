// ============================================================
// main.bicep — Retail Lab: Azure SQL Server + Database
// ============================================================

@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Unique suffix appended to resource names to avoid conflicts')
param nameSuffix string = uniqueString(resourceGroup().id)

@description('SQL Server administrator login name')
param sqlAdminLogin string = 'retailadmin'

@description('SQL Server administrator password')
@secure()
param sqlAdminPassword string

@description('Name of the SQL database')
param databaseName string = 'RetailDb'

@description('SKU name for the Azure SQL Database (e.g. S1, S2, GP_S_Gen5_1)')
param databaseSku string = 'S1'

@description('Allow Azure services to connect to this server')
param allowAzureServices bool = true

// ── SQL Server ────────────────────────────────────────────────
var sqlServerName = 'sql-retail-lab-${nameSuffix}'

resource sqlServer 'Microsoft.Sql/servers@2021-11-01' = {
  name: sqlServerName
  location: location
  properties: {
    administratorLogin: sqlAdminLogin
    administratorLoginPassword: sqlAdminPassword
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
  }
}

// ── Firewall: allow Azure services ──────────────────────────
resource azureServicesFirewall 'Microsoft.Sql/servers/firewallRules@2021-11-01' = if (allowAzureServices) {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// ── SQL Database ─────────────────────────────────────────────
resource sqlDatabase 'Microsoft.Sql/servers/databases@2021-11-01' = {
  parent: sqlServer
  name: databaseName
  location: location
  sku: {
    name: databaseSku
    tier: 'Standard'
  }
  properties: {
    collation: 'SQL_Latin1_General_CP1_CI_AS'
    maxSizeBytes: 10737418240 // 10 GB
    zoneRedundant: false
    readScale: 'Disabled'
    requestedBackupStorageRedundancy: 'Local'
  }
}

// ── Outputs ──────────────────────────────────────────────────
output sqlServerName string = sqlServer.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
output databaseName string = sqlDatabase.name
output connectionStringTemplate string = 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Initial Catalog=${databaseName};Persist Security Info=False;User ID=${sqlAdminLogin};Password=<password>;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
