# Provisions a Free Azure Static Web App for SquadMasterWeb (Blazor WASM).
# Tenant: digitalox.ca (542c6e72-9e01-4900-ac68-1b8a6b64e00e)
#
# Prerequisites:
#   az login --tenant 542c6e72-9e01-4900-ac68-1b8a6b64e00e
#   az account set --subscription <subscription-id>
#
# Usage:
#   .\scripts\provision-azure.ps1 -SubscriptionId <guid>
#
# Free SKU is available in a subset of regions. Default is eastus2.

[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string] $SubscriptionId,

    [string] $Location = "eastus2",
    [string] $ResourceGroup = "rg-squadmaster",
    [string] $StaticWebAppName = "squadmaster-web",
    [string] $TenantId = "542c6e72-9e01-4900-ac68-1b8a6b64e00e"
)

$ErrorActionPreference = "Stop"

Write-Host "Setting subscription $SubscriptionId (tenant $TenantId)..."
az account set --subscription $SubscriptionId
if ($LASTEXITCODE -ne 0) { throw "az account set failed. Login with: az login --tenant $TenantId" }

Write-Host "Ensuring resource group $ResourceGroup in $Location..."
az group create -n $ResourceGroup -l $Location | Out-Null

Write-Host "Creating Static Web App $StaticWebAppName (Free)..."
az staticwebapp create `
    -g $ResourceGroup `
    -n $StaticWebAppName `
    -l $Location `
    --sku Free | Out-Null

$swaHostname = az staticwebapp show -g $ResourceGroup -n $StaticWebAppName --query defaultHostname -o tsv
$swaToken = az staticwebapp secrets list -g $ResourceGroup -n $StaticWebAppName --query properties.apiKey -o tsv

Write-Host ""
Write-Host "=== GitHub configuration ==="
Write-Host "Repo: DigitalOxCanada/SquadMasterWeb"
Write-Host ""
Write-Host "Repo secret AZURE_STATIC_WEB_APPS_API_TOKEN:"
Write-Host $swaToken
Write-Host ""
Write-Host "Static Web App URL: https://$swaHostname"
Write-Host "Done."
