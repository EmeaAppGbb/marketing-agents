# Deployment Guide

This guide describes how to deploy the Marketing Agents Platform to Azure using Azure Container Apps.

## Prerequisites

Before deploying, ensure you have:

- [Azure CLI](https://docs.microsoft.com/cli/azure/install-azure-cli) installed
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) installed
- [Azure Developer CLI (azd)](https://learn.microsoft.com/azure/developer/azure-developer-cli/install-azd) installed (recommended)
- Azure subscription with appropriate permissions

## Deployment Options

### Option 1: Azure Developer CLI (Recommended)

The simplest way to deploy is using `azd`:

```bash
# Initialize azd (first time only)
azd init

# Provision infrastructure and deploy
azd up
```

This command will:
1. Provision Azure resources (Container Apps, Cosmos DB, Redis, etc.)
2. Build container images
3. Push images to Azure Container Registry
4. Deploy services to Azure Container Apps
5. Configure service discovery and networking

### Option 2: Manual Deployment with Aspire

Use .NET Aspire's publish command to generate deployment manifests:

```bash
# Generate deployment manifests
aspire publish --output ./deploy

# Review generated files in ./deploy directory
ls ./deploy
```

Then deploy using Azure CLI or Azure Portal.

## Step-by-Step Deployment (Azure Developer CLI)

### Step 1: Login to Azure

```bash
# Login to Azure
az login

# Select subscription (if you have multiple)
az account set --subscription "Your Subscription Name"

# Login to azd
azd auth login
```

### Step 2: Initialize Project

```bash
# Navigate to repository root
cd marketing-agents

# Initialize azd (creates azure.yaml if not exists)
azd init

# Select subscription and location when prompted
# Recommended: East US 2, West Europe, or your nearest region
```

### Step 3: Configure Environment

Create or update `.azure/<environment-name>/.env` with configuration:

```bash
# Create environment
azd env new production

# Set environment variables
azd env set AZURE_LOCATION eastus2
azd env set AZURE_OPENAI_ENDPOINT https://your-openai.openai.azure.com/
```

### Step 4: Provision and Deploy

```bash
# Provision infrastructure and deploy all services
azd up

# This will:
# - Create resource group
# - Provision Azure Container Apps Environment
# - Provision Cosmos DB
# - Provision Redis Cache
# - Provision Azure Container Registry
# - Build container images
# - Push images to ACR
# - Deploy services
```

Expected output:
```
Provisioning Azure resources can take some time.

Subscription: <your-subscription> (xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx)
Location: East US 2

  You can view detailed progress in the Azure Portal:
  https://portal.azure.com/#view/HubsExtension/DeploymentDetailsBlade/...

  (✓) Done: Resource group: rg-marketing-agents-prod
  (✓) Done: Container Apps Environment: cae-marketing-agents-prod
  (✓) Done: Cosmos DB Account: cosmos-marketing-agents-prod
  (✓) Done: Redis Cache: redis-marketing-agents-prod
  (✓) Done: Container Registry: crmarketingagents
  (✓) Done: Container App: ca-marketing-api
  (✓) Done: Container App: ca-marketing-agenthost

SUCCESS: Your application was provisioned and deployed to Azure in X minutes Y seconds.
```

### Step 5: Verify Deployment

```bash
# Get endpoint URLs
azd show

# Expected output:
# API Endpoint: https://ca-marketing-api.region.azurecontainerapps.io
# AgentHost Endpoint: https://ca-marketing-agenthost.region.azurecontainerapps.io
```

Test the API:
```bash
curl https://ca-marketing-api.region.azurecontainerapps.io/health
```

## Manual Deployment Steps

If not using `azd`, follow these manual steps:

### Step 1: Create Resource Group

```bash
az group create \
  --name rg-marketing-agents-prod \
  --location eastus2
```

### Step 2: Create Container Apps Environment

```bash
az containerapp env create \
  --name cae-marketing-agents-prod \
  --resource-group rg-marketing-agents-prod \
  --location eastus2
```

### Step 3: Create Cosmos DB

```bash
az cosmosdb create \
  --name cosmos-marketing-agents-prod \
  --resource-group rg-marketing-agents-prod \
  --locations regionName=eastus2 failoverPriority=0 \
  --default-consistency-level Session

# Create database
az cosmosdb sql database create \
  --account-name cosmos-marketing-agents-prod \
  --resource-group rg-marketing-agents-prod \
  --name marketing

# Create containers
az cosmosdb sql container create \
  --account-name cosmos-marketing-agents-prod \
  --database-name marketing \
  --name campaigns \
  --partition-key-path /id \
  --throughput 400
```

### Step 4: Create Redis Cache

```bash
az redis create \
  --name redis-marketing-agents-prod \
  --resource-group rg-marketing-agents-prod \
  --location eastus2 \
  --sku Basic \
  --vm-size c0
```

### Step 5: Create Container Registry

```bash
az acr create \
  --name crmarketingagents \
  --resource-group rg-marketing-agents-prod \
  --location eastus2 \
  --sku Basic \
  --admin-enabled true
```

### Step 6: Build and Push Container Images

```bash
# Login to ACR
az acr login --name crmarketingagents

# Build and push API image
docker build -t crmarketingagents.azurecr.io/marketing-api:latest \
  -f MarketingAgents.Api/Dockerfile .
docker push crmarketingagents.azurecr.io/marketing-api:latest

# Build and push AgentHost image
docker build -t crmarketingagents.azurecr.io/marketing-agenthost:latest \
  -f MarketingAgents.AgentHost/Dockerfile .
docker push crmarketingagents.azurecr.io/marketing-agenthost:latest
```

### Step 7: Deploy Container Apps

```bash
# Get ACR credentials
ACR_USERNAME=$(az acr credential show --name crmarketingagents --query username -o tsv)
ACR_PASSWORD=$(az acr credential show --name crmarketingagents --query passwords[0].value -o tsv)

# Deploy API service
az containerapp create \
  --name ca-marketing-api \
  --resource-group rg-marketing-agents-prod \
  --environment cae-marketing-agents-prod \
  --image crmarketingagents.azurecr.io/marketing-api:latest \
  --registry-server crmarketingagents.azurecr.io \
  --registry-username $ACR_USERNAME \
  --registry-password $ACR_PASSWORD \
  --target-port 8080 \
  --ingress external \
  --min-replicas 1 \
  --max-replicas 5 \
  --cpu 1.0 \
  --memory 2.0Gi \
  --env-vars \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "ConnectionStrings__CosmosDb=<cosmos-connection-string>" \
    "ConnectionStrings__Redis=<redis-connection-string>"

# Deploy AgentHost service
az containerapp create \
  --name ca-marketing-agenthost \
  --resource-group rg-marketing-agents-prod \
  --environment cae-marketing-agents-prod \
  --image crmarketingagents.azurecr.io/marketing-agenthost:latest \
  --registry-server crmarketingagents.azurecr.io \
  --registry-username $ACR_USERNAME \
  --registry-password $ACR_PASSWORD \
  --target-port 8080 \
  --ingress internal \
  --min-replicas 1 \
  --max-replicas 10 \
  --cpu 2.0 \
  --memory 4.0Gi \
  --env-vars \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "ConnectionStrings__CosmosDb=<cosmos-connection-string>" \
    "AzureAI__Endpoint=<azure-openai-endpoint>" \
    "AzureAI__ApiKey=<azure-openai-key>"
```

## Configuration

### Environment Variables

Set environment variables for production:

```bash
# Using azd
azd env set AZURE_OPENAI_ENDPOINT https://your-openai.openai.azure.com/
azd env set AZURE_OPENAI_API_KEY your-api-key

# Using Azure CLI
az containerapp update \
  --name ca-marketing-api \
  --resource-group rg-marketing-agents-prod \
  --set-env-vars \
    "AzureAI__Endpoint=https://your-openai.openai.azure.com/" \
    "AzureAI__ApiKey=your-api-key"
```

### Secrets Management

Use Azure Key Vault for production secrets:

```bash
# Create Key Vault
az keyvault create \
  --name kv-marketing-agents \
  --resource-group rg-marketing-agents-prod \
  --location eastus2

# Store secrets
az keyvault secret set \
  --vault-name kv-marketing-agents \
  --name CosmosDbConnectionString \
  --value "<connection-string>"

# Grant Container App access to Key Vault
# (Enable managed identity and assign access policy)
```

### Scaling Configuration

Configure auto-scaling rules:

```bash
az containerapp update \
  --name ca-marketing-api \
  --resource-group rg-marketing-agents-prod \
  --min-replicas 2 \
  --max-replicas 10 \
  --scale-rule-name http-rule \
  --scale-rule-type http \
  --scale-rule-http-concurrency 100
```

## Continuous Deployment

### GitHub Actions

The repository includes a GitHub Actions workflow for CI/CD:

```yaml
# .github/workflows/deploy.yml
name: Deploy to Azure

on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v4
      
      - name: Azure Login
        uses: azure/login@v1
        with:
          creds: ${{ secrets.AZURE_CREDENTIALS }}
      
      - name: Deploy with azd
        run: azd up --no-prompt
        env:
          AZURE_ENV_NAME: production
          AZURE_LOCATION: eastus2
```

### Setup GitHub Secrets

```bash
# Create service principal
az ad sp create-for-rbac \
  --name "github-marketing-agents" \
  --role contributor \
  --scopes /subscriptions/<subscription-id>/resourceGroups/rg-marketing-agents-prod \
  --sdk-auth

# Copy the output JSON and store as GitHub secret: AZURE_CREDENTIALS
```

## Monitoring

### Application Insights

```bash
# Create Application Insights
az monitor app-insights component create \
  --app marketing-agents-insights \
  --location eastus2 \
  --resource-group rg-marketing-agents-prod

# Get instrumentation key
az monitor app-insights component show \
  --app marketing-agents-insights \
  --resource-group rg-marketing-agents-prod \
  --query instrumentationKey -o tsv

# Configure in Container Apps
az containerapp update \
  --name ca-marketing-api \
  --resource-group rg-marketing-agents-prod \
  --set-env-vars \
    "APPLICATIONINSIGHTS_CONNECTION_STRING=<connection-string>"
```

### View Logs

```bash
# Stream logs from Container App
az containerapp logs show \
  --name ca-marketing-api \
  --resource-group rg-marketing-agents-prod \
  --follow

# Query logs with Log Analytics
az monitor log-analytics query \
  --workspace <workspace-id> \
  --analytics-query "ContainerAppConsoleLogs_CL | where ContainerAppName_s == 'ca-marketing-api' | order by TimeGenerated desc"
```

## Rollback

If deployment fails, rollback to previous version:

```bash
# List revisions
az containerapp revision list \
  --name ca-marketing-api \
  --resource-group rg-marketing-agents-prod \
  --query "[].{Name:name, Active:properties.active, Created:properties.createdTime}" -o table

# Activate previous revision
az containerapp revision activate \
  --name ca-marketing-api \
  --resource-group rg-marketing-agents-prod \
  --revision <previous-revision-name>
```

## Cleanup

Remove all Azure resources:

```bash
# Using azd
azd down --purge

# Or manually delete resource group
az group delete --name rg-marketing-agents-prod --yes
```

## Next Steps

- [Development Workflow](development.md) - Contribute to the project
- [Troubleshooting](troubleshooting.md) - Common deployment issues
- [Configuration Reference](../reference/configuration.md) - All configuration options
