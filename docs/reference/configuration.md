# Configuration Reference

Complete reference for all configuration options in the Marketing Agents Platform.

## Configuration Sources

Configuration is loaded from multiple sources in this priority order (highest to lowest):

1. **User Secrets** (local development only)
2. **Environment Variables**
3. **`appsettings.{Environment}.json`**
4. **`appsettings.json`**

## appsettings.json Structure

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "CosmosDb": "AccountEndpoint=https://localhost:8081/;AccountKey=...",
    "Redis": "localhost:6379"
  },
  "AzureAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "gpt-4",
    "MaxRetries": 5,
    "Timeout": "00:05:00"
  },
  "CosmosDb": {
    "DatabaseName": "marketing",
    "ContainerNames": {
      "Campaigns": "campaigns",
      "Artifacts": "artifacts",
      "AuditLogs": "audit-logs"
    },
    "DefaultThroughput": 400,
    "EnableContentResponseOnWrite": false
  },
  "Redis": {
    "InstanceName": "marketing:",
    "AbortOnConnectFail": false,
    "ConnectRetry": 3,
    "DefaultExpirationMinutes": 60
  },
  "AgentFramework": {
    "MaxConcurrentAgents": 3,
    "DefaultAgentTimeout": "00:05:00",
    "MaxRetryAttempts": 5,
    "EnableCaching": true
  },
  "SignalR": {
    "EnableDetailedErrors": false,
    "KeepAliveInterval": "00:00:15",
    "ClientTimeoutInterval": "00:00:30",
    "HandshakeTimeout": "00:00:15"
  },
  "RateLimiting": {
    "RequestsPerMinute": 100,
    "BurstSize": 20
  },
  "CORS": {
    "AllowedOrigins": ["http://localhost:3000"],
    "AllowCredentials": true
  }
}
```

## Configuration Sections

### Logging

Controls application logging behavior.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `Logging:LogLevel:Default` | string | `Information` | Default log level for all categories |
| `Logging:LogLevel:Microsoft.AspNetCore` | string | `Warning` | Log level for ASP.NET Core framework |

**Valid Log Levels**: `Trace`, `Debug`, `Information`, `Warning`, `Error`, `Critical`, `None`

**Example**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "MarketingAgents.AgentHost": "Debug"
    }
  }
}
```

### Connection Strings

Database and cache connection strings.

| Setting | Type | Required | Description |
|---------|------|----------|-------------|
| `ConnectionStrings:CosmosDb` | string | Yes | Cosmos DB connection string |
| `ConnectionStrings:Redis` | string | Yes | Redis connection string |

**Local Development (Cosmos DB Emulator)**:
```json
{
  "ConnectionStrings": {
    "CosmosDb": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
  }
}
```

**Production (Azure Cosmos DB)**:
```json
{
  "ConnectionStrings": {
    "CosmosDb": "AccountEndpoint=https://your-account.documents.azure.com:443/;AccountKey=<your-key>=="
  }
}
```

### Azure AI Configuration

Settings for Azure OpenAI and AI Foundry.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `AzureAI:Endpoint` | string | - | Azure OpenAI endpoint URL |
| `AzureAI:ApiKey` | string | - | Azure OpenAI API key |
| `AzureAI:DeploymentName` | string | `gpt-4` | Model deployment name |
| `AzureAI:MaxRetries` | int | `5` | Max retry attempts for failed requests |
| `AzureAI:Timeout` | TimeSpan | `00:05:00` | Request timeout (5 minutes) |

**Example**:
```json
{
  "AzureAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "your-api-key",
    "DeploymentName": "gpt-4-turbo",
    "MaxRetries": 5,
    "Timeout": "00:05:00"
  }
}
```

### Cosmos DB Configuration

Cosmos DB specific settings.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `CosmosDb:DatabaseName` | string | `marketing` | Database name |
| `CosmosDb:ContainerNames:Campaigns` | string | `campaigns` | Campaigns container name |
| `CosmosDb:ContainerNames:Artifacts` | string | `artifacts` | Artifacts container name |
| `CosmosDb:ContainerNames:AuditLogs` | string | `audit-logs` | Audit logs container name |
| `CosmosDb:DefaultThroughput` | int | `400` | Default RU/s for containers |
| `CosmosDb:EnableContentResponseOnWrite` | bool | `false` | Return content on write operations |

**Example**:
```json
{
  "CosmosDb": {
    "DatabaseName": "marketing-prod",
    "ContainerNames": {
      "Campaigns": "campaigns",
      "Artifacts": "artifacts",
      "AuditLogs": "audit-logs"
    },
    "DefaultThroughput": 1000,
    "EnableContentResponseOnWrite": false
  }
}
```

### Redis Configuration

Redis cache settings.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `Redis:InstanceName` | string | `marketing:` | Key prefix for all Redis keys |
| `Redis:AbortOnConnectFail` | bool | `false` | Abort if initial connection fails |
| `Redis:ConnectRetry` | int | `3` | Number of connection retry attempts |
| `Redis:DefaultExpirationMinutes` | int | `60` | Default cache expiration (minutes) |

**Example**:
```json
{
  "Redis": {
    "InstanceName": "marketing-prod:",
    "AbortOnConnectFail": false,
    "ConnectRetry": 5,
    "DefaultExpirationMinutes": 120
  }
}
```

### Agent Framework Configuration

AI agent execution settings.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `AgentFramework:MaxConcurrentAgents` | int | `3` | Max agents running in parallel |
| `AgentFramework:DefaultAgentTimeout` | TimeSpan | `00:05:00` | Default agent timeout (5 minutes) |
| `AgentFramework:MaxRetryAttempts` | int | `5` | Max retry attempts for failed agent runs |
| `AgentFramework:EnableCaching` | bool | `true` | Enable agent response caching |

**Example**:
```json
{
  "AgentFramework": {
    "MaxConcurrentAgents": 5,
    "DefaultAgentTimeout": "00:10:00",
    "MaxRetryAttempts": 3,
    "EnableCaching": true
  }
}
```

### SignalR Configuration

Real-time communication settings.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `SignalR:EnableDetailedErrors` | bool | `false` | Include detailed error messages |
| `SignalR:KeepAliveInterval` | TimeSpan | `00:00:15` | Keep-alive ping interval (15 seconds) |
| `SignalR:ClientTimeoutInterval` | TimeSpan | `00:00:30` | Client timeout (30 seconds) |
| `SignalR:HandshakeTimeout` | TimeSpan | `00:00:15` | Handshake timeout (15 seconds) |

**Example**:
```json
{
  "SignalR": {
    "EnableDetailedErrors": true,
    "KeepAliveInterval": "00:00:10",
    "ClientTimeoutInterval": "00:01:00",
    "HandshakeTimeout": "00:00:30"
  }
}
```

### Rate Limiting Configuration

API rate limiting settings.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `RateLimiting:RequestsPerMinute` | int | `100` | Max requests per minute per user |
| `RateLimiting:BurstSize` | int | `20` | Burst allowance |

**Example**:
```json
{
  "RateLimiting": {
    "RequestsPerMinute": 200,
    "BurstSize": 50
  }
}
```

### CORS Configuration

Cross-Origin Resource Sharing settings.

| Setting | Type | Default | Description |
|---------|------|---------|-------------|
| `CORS:AllowedOrigins` | array | `[]` | Allowed origin URLs |
| `CORS:AllowCredentials` | bool | `true` | Allow credentials in requests |

**Example**:
```json
{
  "CORS": {
    "AllowedOrigins": [
      "https://marketing-agents.com",
      "https://staging.marketing-agents.com"
    ],
    "AllowCredentials": true
  }
}
```

## Environment-Specific Configuration

### Development (`appsettings.Development.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "Microsoft.AspNetCore": "Information"
    }
  },
  "ConnectionStrings": {
    "CosmosDb": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==",
    "Redis": "localhost:6379"
  },
  "SignalR": {
    "EnableDetailedErrors": true
  },
  "CORS": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:3001"]
  }
}
```

### Production (`appsettings.Production.json`)

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "SignalR": {
    "EnableDetailedErrors": false
  },
  "AgentFramework": {
    "MaxConcurrentAgents": 10
  },
  "RateLimiting": {
    "RequestsPerMinute": 200,
    "BurstSize": 50
  }
}
```

## User Secrets (Local Development)

Store sensitive configuration in User Secrets:

```bash
# Initialize user secrets
cd MarketingAgents.Api
dotnet user-secrets init

# Set secrets
dotnet user-secrets set "ConnectionStrings:CosmosDb" "AccountEndpoint=..."
dotnet user-secrets set "AzureAI:ApiKey" "your-api-key"
dotnet user-secrets set "AzureAI:Endpoint" "https://your-resource.openai.azure.com/"
```

User secrets location:
- **macOS/Linux**: `~/.microsoft/usersecrets/<user-secrets-id>/secrets.json`
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\<user-secrets-id>\secrets.json`

## Environment Variables

Override configuration using environment variables:

```bash
# Bash/Zsh
export ConnectionStrings__CosmosDb="AccountEndpoint=..."
export AzureAI__ApiKey="your-api-key"

# PowerShell
$env:ConnectionStrings__CosmosDb="AccountEndpoint=..."
$env:AzureAI__ApiKey="your-api-key"
```

**Note**: Use double underscores `__` to represent nested configuration keys.

## Azure Key Vault Integration

For production, use Azure Key Vault:

```csharp
// Program.cs
if (builder.Environment.IsProduction())
{
    builder.Configuration.AddAzureKeyVault(
        new Uri($"https://{keyVaultName}.vault.azure.net/"),
        new DefaultAzureCredential());
}
```

## Next Steps

- [Environment Variables Reference](environment-variables.md) - All environment variables
- [Deployment Guide](../guides/deployment.md) - Deploy to Azure
- [Development Setup](../development/setup.md) - Local development
