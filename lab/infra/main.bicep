// ============================================================
// main.bicep — Retail Lab: Azure SQL Server + Database
// ============================================================

@description('Azure region for all resources')
param location string = resourceGroup().location

@description('Unique suffix appended to resource names to avoid conflicts')
param nameSuffix string = uniqueString(resourceGroup().id)

@description('Azure AD admin display name (e.g. user principal name)')
param aadAdminLogin string

@description('Azure AD admin object ID')
param aadAdminObjectId string

@description('Azure AD tenant ID')
param aadAdminTenantId string

@description('Name of the SQL database')
param databaseName string = 'RetailDb'

@description('SKU name for the Azure SQL Database (e.g. S1, S2, GP_S_Gen5_1)')
param databaseSku string = 'S1'

@description('Allow Azure services to connect to this server')
param allowAzureServices bool = true

// ── SQL Server (Azure AD-only authentication) ─────────────────
var sqlServerName = 'sql-retail-lab-${nameSuffix}'

resource sqlServer 'Microsoft.Sql/servers@2023-05-01-preview' = {
  name: sqlServerName
  location: location
  properties: {
    version: '12.0'
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      principalType: 'User'
      login: aadAdminLogin
      sid: aadAdminObjectId
      tenantId: aadAdminTenantId
      azureADOnlyAuthentication: true
    }
  }
}

// ── Firewall: allow Azure services ──────────────────────────
resource azureServicesFirewall 'Microsoft.Sql/servers/firewallRules@2023-05-01-preview' = if (allowAzureServices) {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

// ── SQL Database ─────────────────────────────────────────────
resource sqlDatabase 'Microsoft.Sql/servers/databases@2023-05-01-preview' = {
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
