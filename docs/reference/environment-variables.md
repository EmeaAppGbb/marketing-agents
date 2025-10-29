# Environment Variables Reference

Complete reference for all environment variables used in the Marketing Agents Platform.

## Standard .NET Environment Variables

| Variable | Values | Description |
|----------|--------|-------------|
| `ASPNETCORE_ENVIRONMENT` | `Development`, `Staging`, `Production` | Application environment |
| `DOTNET_ENVIRONMENT` | `Development`, `Staging`, `Production` | .NET runtime environment |
| `ASPNETCORE_URLS` | URL(s) | Endpoints to listen on (e.g., `http://+:8080`) |

**Example**:
```bash
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS=http://+:8080;https://+:8081
```

## Connection Strings

| Variable | Format | Description |
|----------|--------|-------------|
| `ConnectionStrings__CosmosDb` | `AccountEndpoint=...;AccountKey=...` | Cosmos DB connection string |
| `ConnectionStrings__Redis` | `hostname:port` or connection string | Redis connection string |

**Example**:
```bash
# Cosmos DB
export ConnectionStrings__CosmosDb="AccountEndpoint=https://your-account.documents.azure.com:443/;AccountKey=<key>"

# Redis
export ConnectionStrings__Redis="your-cache.redis.cache.windows.net:6380,password=<key>,ssl=True"
```

## Azure AI Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `AzureAI__Endpoint` | URL | Azure OpenAI endpoint |
| `AzureAI__ApiKey` | string | Azure OpenAI API key |
| `AzureAI__DeploymentName` | string | Model deployment name (default: `gpt-4`) |
| `AzureAI__MaxRetries` | integer | Max retry attempts (default: `5`) |
| `AzureAI__Timeout` | TimeSpan | Request timeout (default: `00:05:00`) |

**Example**:
```bash
export AzureAI__Endpoint="https://your-resource.openai.azure.com/"
export AzureAI__ApiKey="your-api-key"
export AzureAI__DeploymentName="gpt-4-turbo"
export AzureAI__MaxRetries="3"
export AzureAI__Timeout="00:10:00"
```

## Cosmos DB Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `CosmosDb__DatabaseName` | string | Database name (default: `marketing`) |
| `CosmosDb__ContainerNames__Campaigns` | string | Campaigns container name |
| `CosmosDb__ContainerNames__Artifacts` | string | Artifacts container name |
| `CosmosDb__ContainerNames__AuditLogs` | string | Audit logs container name |
| `CosmosDb__DefaultThroughput` | integer | Default RU/s (default: `400`) |
| `CosmosDb__EnableContentResponseOnWrite` | boolean | Return content on writes (default: `false`) |

**Example**:
```bash
export CosmosDb__DatabaseName="marketing-prod"
export CosmosDb__DefaultThroughput="1000"
export CosmosDb__EnableContentResponseOnWrite="false"
```

## Redis Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `Redis__InstanceName` | string | Key prefix (default: `marketing:`) |
| `Redis__AbortOnConnectFail` | boolean | Abort if connection fails (default: `false`) |
| `Redis__ConnectRetry` | integer | Connection retry attempts (default: `3`) |
| `Redis__DefaultExpirationMinutes` | integer | Default cache expiration (default: `60`) |

**Example**:
```bash
export Redis__InstanceName="marketing-prod:"
export Redis__ConnectRetry="5"
export Redis__DefaultExpirationMinutes="120"
```

## Agent Framework Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `AgentFramework__MaxConcurrentAgents` | integer | Max parallel agents (default: `3`) |
| `AgentFramework__DefaultAgentTimeout` | TimeSpan | Agent timeout (default: `00:05:00`) |
| `AgentFramework__MaxRetryAttempts` | integer | Max retries (default: `5`) |
| `AgentFramework__EnableCaching` | boolean | Enable caching (default: `true`) |

**Example**:
```bash
export AgentFramework__MaxConcurrentAgents="10"
export AgentFramework__DefaultAgentTimeout="00:10:00"
export AgentFramework__MaxRetryAttempts="3"
export AgentFramework__EnableCaching="true"
```

## SignalR Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `SignalR__EnableDetailedErrors` | boolean | Include detailed errors (default: `false`) |
| `SignalR__KeepAliveInterval` | TimeSpan | Ping interval (default: `00:00:15`) |
| `SignalR__ClientTimeoutInterval` | TimeSpan | Client timeout (default: `00:00:30`) |
| `SignalR__HandshakeTimeout` | TimeSpan | Handshake timeout (default: `00:00:15`) |

**Example**:
```bash
export SignalR__EnableDetailedErrors="true"
export SignalR__KeepAliveInterval="00:00:10"
export SignalR__ClientTimeoutInterval="00:01:00"
```

## Rate Limiting Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `RateLimiting__RequestsPerMinute` | integer | Max requests/minute (default: `100`) |
| `RateLimiting__BurstSize` | integer | Burst allowance (default: `20`) |

**Example**:
```bash
export RateLimiting__RequestsPerMinute="200"
export RateLimiting__BurstSize="50"
```

## CORS Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `CORS__AllowedOrigins__0` | URL | First allowed origin |
| `CORS__AllowedOrigins__1` | URL | Second allowed origin |
| `CORS__AllowCredentials` | boolean | Allow credentials (default: `true`) |

**Example**:
```bash
export CORS__AllowedOrigins__0="https://marketing-agents.com"
export CORS__AllowedOrigins__1="https://staging.marketing-agents.com"
export CORS__AllowCredentials="true"
```

## Logging Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `Logging__LogLevel__Default` | `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical` | Default log level |
| `Logging__LogLevel__Microsoft.AspNetCore` | Log level | ASP.NET Core log level |
| `Logging__Console__FormatterName` | `simple`, `json`, `systemd` | Console log format |

**Example**:
```bash
export Logging__LogLevel__Default="Information"
export Logging__LogLevel__Microsoft.AspNetCore="Warning"
export Logging__Console__FormatterName="json"
```

## Application Insights

| Variable | Format | Description |
|----------|--------|-------------|
| `APPLICATIONINSIGHTS_CONNECTION_STRING` | Connection string | Application Insights connection string |
| `ApplicationInsights__InstrumentationKey` | GUID | Instrumentation key (deprecated, use connection string) |

**Example**:
```bash
export APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=...;IngestionEndpoint=..."
```

## Azure-Specific Variables

### Managed Identity

| Variable | Format | Description |
|----------|--------|-------------|
| `AZURE_CLIENT_ID` | GUID | Managed identity client ID |
| `AZURE_TENANT_ID` | GUID | Azure AD tenant ID |
| `AZURE_CLIENT_SECRET` | string | Service principal secret (avoid in production) |

### Azure Developer CLI

| Variable | Format | Description |
|----------|--------|-------------|
| `AZURE_ENV_NAME` | string | azd environment name |
| `AZURE_LOCATION` | string | Azure region (e.g., `eastus2`) |
| `AZURE_SUBSCRIPTION_ID` | GUID | Azure subscription ID |

**Example**:
```bash
export AZURE_ENV_NAME="production"
export AZURE_LOCATION="eastus2"
export AZURE_SUBSCRIPTION_ID="xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
```

## Docker & Container Configuration

| Variable | Format | Description |
|----------|--------|-------------|
| `DOTNET_RUNNING_IN_CONTAINER` | `true`, `false` | Indicates running in container |
| `DOTNET_SYSTEM_GLOBALIZATION_INVARIANT` | `true`, `false` | Use invariant globalization |

**Example**:
```bash
export DOTNET_RUNNING_IN_CONTAINER="true"
export DOTNET_SYSTEM_GLOBALIZATION_INVARIANT="false"
```

## Setting Environment Variables

### Bash/Zsh (Linux/macOS)

**Temporary (current session)**:
```bash
export ASPNETCORE_ENVIRONMENT=Production
export AzureAI__ApiKey="your-api-key"
```

**Permanent (add to `~/.bashrc` or `~/.zshrc`)**:
```bash
echo 'export ASPNETCORE_ENVIRONMENT=Production' >> ~/.zshrc
source ~/.zshrc
```

### PowerShell (Windows)

**Temporary (current session)**:
```powershell
$env:ASPNETCORE_ENVIRONMENT="Production"
$env:AzureAI__ApiKey="your-api-key"
```

**Permanent (user profile)**:
```powershell
[System.Environment]::SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production", "User")
```

### Docker

**Dockerfile**:
```dockerfile
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:8080
```

**docker-compose.yml**:
```yaml
services:
  api:
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - AzureAI__ApiKey=${AZURE_AI_API_KEY}
```

### Azure Container Apps

```bash
az containerapp update \
  --name ca-marketing-api \
  --resource-group rg-marketing-agents-prod \
  --set-env-vars \
    "ASPNETCORE_ENVIRONMENT=Production" \
    "AzureAI__Endpoint=https://your-resource.openai.azure.com/"
```

### GitHub Actions

```yaml
env:
  ASPNETCORE_ENVIRONMENT: Production
  AZURE_LOCATION: eastus2

jobs:
  deploy:
    steps:
      - name: Deploy
        env:
          AZURE_AI_API_KEY: ${{ secrets.AZURE_AI_API_KEY }}
        run: azd up --no-prompt
```

## Security Best Practices

1. **Never commit secrets to version control**
   ```bash
   # Use .gitignore to exclude
   .env
   .env.local
   appsettings.*.json
   ```

2. **Use Azure Key Vault for production secrets**
   ```bash
   # Reference secrets from Key Vault
   export ConnectionStrings__CosmosDb="@Microsoft.KeyVault(SecretUri=https://vault.vault.azure.net/secrets/CosmosDbConnectionString/)"
   ```

3. **Use managed identities in Azure**
   ```bash
   # No need to store credentials, use managed identity
   az containerapp identity assign \
     --name ca-marketing-api \
     --resource-group rg-marketing-agents-prod \
     --system-assigned
   ```

4. **Rotate secrets regularly**
   ```bash
   # Update Key Vault secret
   az keyvault secret set \
     --vault-name kv-marketing-agents \
     --name AzureAIApiKey \
     --value "new-api-key"
   ```

## Troubleshooting

### Environment Variable Not Applied

**Issue**: Changes to environment variables not taking effect

**Solutions**:

1. **Restart the application** after setting variables
2. **Verify variable name** (use `__` for nested keys, not `:`)
3. **Check configuration priority** (User Secrets > Env Vars > appsettings)

### Invalid Format

**Issue**: `FormatException` when parsing environment variable

**Solutions**:

1. **Boolean values**: Use `true` or `false` (case-insensitive)
2. **TimeSpan values**: Use format `hh:mm:ss` (e.g., `00:05:00`)
3. **Integer values**: Use plain numbers (e.g., `100`)
4. **Arrays**: Use indexed keys (e.g., `CORS__AllowedOrigins__0`)

## Next Steps

- [Configuration Reference](configuration.md) - Detailed configuration options
- [Deployment Guide](../guides/deployment.md) - Deploy to Azure
- [Development Setup](../development/setup.md) - Local development
