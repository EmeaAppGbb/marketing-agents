# Configuration

This guide explains all configuration options for the Marketing Agents Platform.

## Configuration Sources

The application uses .NET's hierarchical configuration system. Settings are loaded in this order (later sources override earlier ones):

1. `appsettings.json` (base settings)
2. `appsettings.{Environment}.json` (environment-specific)
3. User Secrets (development only)
4. Environment Variables
5. Command-line arguments

## Environment Settings

### Development

**File**: `appsettings.Development.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

### Production

**File**: `appsettings.Production.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*.azurecontainerapps.io"
}
```

## Connection Strings

### Cosmos DB

**User Secrets** (Local Development):
```bash
dotnet user-secrets set "ConnectionStrings:CosmosDb" "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
```

**Environment Variable** (Production):
```bash
export ConnectionStrings__CosmosDb="AccountEndpoint=https://your-account.documents.azure.com:443/;AccountKey=YOUR_KEY"
```

**appsettings.json**:
```json
{
  "ConnectionStrings": {
    "CosmosDb": "AccountEndpoint=https://localhost:8081/;AccountKey=C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw=="
  }
}
```

### Redis Cache

**Aspire-Managed** (Development):
Automatically configured via service discovery.

**Environment Variable** (Production):
```bash
export ConnectionStrings__Redis="your-redis-host:6379,password=YOUR_PASSWORD"
```

## Azure AI Configuration

### Azure OpenAI

**User Secrets**:
```bash
dotnet user-secrets set "AzureAI:Endpoint" "https://your-resource.openai.azure.com/"
dotnet user-secrets set "AzureAI:ApiKey" "your-api-key"
dotnet user-secrets set "AzureAI:DeploymentName" "gpt-4"
```

**appsettings.json**:
```json
{
  "AzureAI": {
    "Endpoint": "https://your-resource.openai.azure.com/",
    "ApiKey": "",  // Never commit API keys
    "DeploymentName": "gpt-4",
    "MaxTokens": 2000,
    "Temperature": 0.7
  }
}
```

## OpenTelemetry Configuration

### OTLP Exporter

**appsettings.json**:
```json
{
  "OpenTelemetry": {
    "Endpoint": "http://localhost:4317",
    "Protocol": "grpc"
  }
}
```

**Environment Variables**:
```bash
export OTEL_EXPORTER_OTLP_ENDPOINT="http://localhost:4317"
export OTEL_EXPORTER_OTLP_PROTOCOL="grpc"
```

### Service Name

Automatically set via Aspire. Override if needed:

```bash
export OTEL_SERVICE_NAME="marketing-agents-api"
```

## SignalR Configuration

### Hub Settings

**appsettings.json**:
```json
{
  "SignalR": {
    "EnableDetailedErrors": true,  // Development only
    "KeepAliveInterval": "00:00:15",
    "ClientTimeoutInterval": "00:00:30",
    "MaximumReceiveMessageSize": 32768
  }
}
```

### Azure SignalR Service (Production)

**Environment Variable**:
```bash
export Azure__SignalR__ConnectionString="Endpoint=https://your-signalr.service.signalr.net;AccessKey=YOUR_KEY"
```

## CORS Configuration

**appsettings.json**:
```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",  // Next.js dev server
      "https://your-app.azurewebsites.net"
    ],
    "AllowedMethods": ["GET", "POST", "PUT", "DELETE"],
    "AllowCredentials": true
  }
}
```

## Health Checks Configuration

**appsettings.json**:
```json
{
  "HealthChecks": {
    "Timeout": "00:00:05",
    "Checks": {
      "CosmosDb": {
        "Enabled": true,
        "Timeout": "00:00:03"
      },
      "Redis": {
        "Enabled": true,
        "Timeout": "00:00:02"
      }
    }
  }
}
```

## Agent Configuration

### Agent Settings

**appsettings.json**:
```json
{
  "Agents": {
    "Copywriting": {
      "Model": "gpt-4",
      "MaxTokens": 2000,
      "Temperature": 0.7,
      "MaxRetries": 5
    },
    "ShortCopy": {
      "Model": "gpt-3.5-turbo",
      "MaxTokens": 500,
      "Temperature": 0.8,
      "MaxRetries": 3
    },
    "Audit": {
      "Model": "gpt-4",
      "MaxTokens": 1000,
      "Temperature": 0.3,
      "StrictMode": true
    }
  }
}
```

## Logging Configuration

### Structured Logging

**appsettings.json**:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.AspNetCore.SignalR": "Information",
      "Microsoft.AspNetCore.Http.Connections": "Information"
    },
    "Console": {
      "IncludeScopes": true,
      "TimestampFormat": "yyyy-MM-dd HH:mm:ss "
    }
  }
}
```

### Application Insights (Production)

**Environment Variable**:
```bash
export APPLICATIONINSIGHTS_CONNECTION_STRING="InstrumentationKey=YOUR_KEY;IngestionEndpoint=https://YOUR_REGION.in.applicationinsights.azure.com/"
```

## Rate Limiting

**appsettings.json**:
```json
{
  "RateLimiting": {
    "PermitLimit": 100,
    "Window": "00:01:00",
    "QueueLimit": 10
  }
}
```

## Feature Flags

**appsettings.json**:
```json
{
  "FeatureManagement": {
    "CampaignIteration": true,
    "VisualConceptAgent": false,
    "ComplianceStrictMode": true
  }
}
```

## Environment-Specific Settings

### Local Development

```bash
# Set environment
export ASPNETCORE_ENVIRONMENT=Development
export DOTNET_ENVIRONMENT=Development

# Enable detailed errors
export ASPNETCORE_DETAILEDERRORS=true

# Use local emulators
export USE_COSMOS_EMULATOR=true
export USE_REDIS_CONTAINER=true
```

### Staging

```bash
export ASPNETCORE_ENVIRONMENT=Staging
export DOTNET_ENVIRONMENT=Staging
```

### Production

```bash
export ASPNETCORE_ENVIRONMENT=Production
export DOTNET_ENVIRONMENT=Production

# Disable detailed errors
export ASPNETCORE_DETAILEDERRORS=false
```

## Secrets Management

### User Secrets (Development)

```bash
# Initialize user secrets for Api project
cd MarketingAgents.Api
dotnet user-secrets init

# Set secrets
dotnet user-secrets set "ConnectionStrings:CosmosDb" "YOUR_CONNECTION_STRING"
dotnet user-secrets set "AzureAI:ApiKey" "YOUR_API_KEY"

# List all secrets
dotnet user-secrets list

# Remove a secret
dotnet user-secrets remove "AzureAI:ApiKey"

# Clear all secrets
dotnet user-secrets clear
```

### Azure Key Vault (Production)

**appsettings.json**:
```json
{
  "KeyVault": {
    "VaultUri": "https://your-keyvault.vault.azure.net/"
  }
}
```

**Program.cs Integration**:
```csharp
if (builder.Environment.IsProduction())
{
    var keyVaultUri = builder.Configuration["KeyVault:VaultUri"];
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUri),
        new DefaultAzureCredential());
}
```

## Configuration Validation

### Validation on Startup

**Program.cs**:
```csharp
builder.Services.AddOptions<AzureAIOptions>()
    .Bind(builder.Configuration.GetSection("AzureAI"))
    .ValidateDataAnnotations()
    .ValidateOnStart();
```

**Options Class**:
```csharp
public class AzureAIOptions
{
    [Required]
    public string Endpoint { get; set; } = string.Empty;
    
    [Required]
    public string ApiKey { get; set; } = string.Empty;
    
    [Required]
    public string DeploymentName { get; set; } = string.Empty;
}
```

## Troubleshooting

### Configuration Not Loading

Check the environment:
```bash
dotnet run --environment Development
```

View effective configuration:
```csharp
// In a controller or service
public MyService(IConfiguration configuration)
{
    var value = configuration["SomeKey"];
    var section = configuration.GetSection("SomeSection");
}
```

### User Secrets Not Found

Ensure `UserSecretsId` is in `.csproj`:
```xml
<PropertyGroup>
  <UserSecretsId>aspnet-MarketingAgents-12345</UserSecretsId>
</PropertyGroup>
```

### Environment Variables Not Applied

Use double underscores (`__`) to separate nested keys:
```bash
# For appsettings: { "ConnectionStrings": { "CosmosDb": "..." } }
export ConnectionStrings__CosmosDb="..."
```

## Reference

- [.NET Configuration Documentation](https://learn.microsoft.com/aspnet/core/fundamentals/configuration/)
- [User Secrets in Development](https://learn.microsoft.com/aspnet/core/security/app-secrets)
- [Azure Key Vault Configuration](https://learn.microsoft.com/aspnet/core/security/key-vault-configuration)
