# Troubleshooting Guide

Common issues and their solutions when working with the Marketing Agents Platform.

## Development Environment Issues

### Aspire Dashboard Not Starting

**Symptoms**: Dashboard doesn't open at http://localhost:15888

**Causes & Solutions**:

1. **Port already in use**
   ```bash
   # Check if port is in use (macOS/Linux)
   lsof -i :15888
   
   # Check if port is in use (Windows)
   netstat -ano | findstr :15888
   
   # Kill the process or change port in AppHost.cs
   ```

2. **Docker not running**
   ```bash
   # Check Docker status
   docker ps
   
   # Start Docker Desktop if not running
   ```

### Cosmos DB Emulator Connection Failed

**Symptoms**: `CosmosException: Service Unavailable`

**Solutions**:

1. **Emulator not started**
   ```bash
   # Check if emulator container is running
   docker ps | grep cosmos
   
   # Restart container if needed
   docker restart <container-id>
   ```

2. **SSL Certificate Issues**
   ```bash
   # Trust the emulator certificate (one-time setup)
   # The emulator will prompt on first run
   
   # Or disable SSL validation for local dev (appsettings.Development.json)
   {
     "CosmosDb": {
       "DisableServerCertificateValidation": true
     }
   }
   ```

### Build Failures

**Symptoms**: `dotnet build` fails with package errors

**Solution**:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Clean and restore
dotnet clean
dotnet restore
dotnet build
```

### SignalR Connection Failures

**Symptoms**: Frontend can't connect to SignalR hub

**Solutions**:

1. **CORS Issues**
   ```csharp
   // Verify CORS is configured in Program.cs
   app.UseCors(policy => policy
       .WithOrigins("http://localhost:3000")
       .AllowAnyHeader()
       .AllowAnyMethod()
       .AllowCredentials());
   ```

2. **Wrong endpoint URL**
   ```typescript
   // Verify endpoint URL matches backend
   const connection = new HubConnectionBuilder()
     .withUrl('http://localhost:5001/hubs/campaign') // Check port!
     .build();
   ```

## Agent Execution Issues

### Agent Timeout

**Symptoms**: Agent execution times out after 30 seconds

**Solution**:
```csharp
// Increase timeout in agent configuration
var agent = new ChatClientAgent(chatClient, 
    instructions: "...",
    name: "AgentName")
{
    Timeout = TimeSpan.FromMinutes(5) // Increase to 5 minutes
};
```

### Rate Limiting from Azure OpenAI

**Symptoms**: `429 Too Many Requests` from Azure OpenAI

**Solutions**:

1. **Implement retry logic**
   ```csharp
   var retryPolicy = Policy
       .Handle<HttpRequestException>()
       .WaitAndRetryAsync(5, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
   ```

2. **Increase quota** in Azure Portal → Azure OpenAI → Quotas

### Agent Returns Empty Response

**Symptoms**: Agent completes but returns empty content

**Debugging Steps**:

1. **Check prompt**
   ```csharp
   // Log the full prompt being sent
   _logger.LogInformation("Sending prompt: {Prompt}", prompt);
   ```

2. **Verify model deployment**
   ```bash
   # Ensure GPT-4 model is deployed in Azure AI Foundry
   ```

3. **Check token limits**
   ```csharp
   // Verify max_tokens is sufficient
   var completionOptions = new ChatCompletionOptions
   {
       MaxTokens = 2000 // Increase if needed
   };
   ```

## Database Issues

### Cosmos DB Query Performance

**Symptoms**: Queries take > 1 second

**Solutions**:

1. **Use partition key**
   ```csharp
   // Always specify partition key in queries
   var query = container.GetItemLinqQueryable<Campaign>(
       requestOptions: new QueryRequestOptions
       {
           PartitionKey = new PartitionKey(campaignId) // Essential!
       });
   ```

2. **Create composite indexes**
   ```json
   {
     "indexingPolicy": {
       "compositeIndexes": [
         [
           { "path": "/status", "order": "ascending" },
           { "path": "/createdAt", "order": "descending" }
         ]
       ]
     }
   }
   ```

### Redis Connection Pool Exhausted

**Symptoms**: `No connection is available`

**Solution**:
```csharp
// Configure connection multiplexer with larger pool
services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect(new ConfigurationOptions
    {
        EndPoints = { redisConnectionString },
        AbortOnConnectFail = false,
        ConnectRetry = 3,
        PoolSize = 20 // Increase pool size
    }));
```

## Testing Issues

### Integration Tests Fail in CI

**Symptoms**: Tests pass locally but fail in GitHub Actions

**Solutions**:

1. **Use Testcontainers**
   ```csharp
   // Ensure tests use Testcontainers for databases
   var cosmosContainer = new CosmosDbBuilder()
       .WithImage("mcr.microsoft.com/cosmosdb/linux/azure-cosmos-emulator:latest")
       .Build();
   
   await cosmosContainer.StartAsync();
   ```

2. **Increase timeouts**
   ```csharp
   [Fact(Timeout = 60000)] // 60 seconds for CI environment
   public async Task TestMethod() { }
   ```

### E2E Tests Flaky

**Symptoms**: Playwright tests pass/fail intermittently

**Solutions**:

1. **Wait for elements properly**
   ```typescript
   // Don't use fixed waits
   await page.waitForSelector('[data-testid="campaign-card"]');
   
   // Instead of
   await page.waitForTimeout(1000); // ❌ Avoid!
   ```

2. **Use retry logic**
   ```typescript
   // In playwright.config.ts
   export default defineConfig({
     retries: 2, // Retry flaky tests twice
   });
   ```

## Deployment Issues

### Container App Fails to Start

**Symptoms**: Container app shows "Unhealthy" status

**Debugging**:

1. **Check logs**
   ```bash
   az containerapp logs show \
     --name ca-marketing-api \
     --resource-group rg-marketing-agents-prod \
     --follow
   ```

2. **Verify environment variables**
   ```bash
   az containerapp show \
     --name ca-marketing-api \
     --resource-group rg-marketing-agents-prod \
     --query properties.template.containers[0].env
   ```

3. **Check health endpoint**
   ```bash
   # Verify /health returns 200 OK
   curl https://ca-marketing-api.region.azurecontainerapps.io/health
   ```

### High Memory Usage

**Symptoms**: Container app restarts frequently due to OOM

**Solutions**:

1. **Increase memory allocation**
   ```bash
   az containerapp update \
     --name ca-marketing-agenthost \
     --resource-group rg-marketing-agents-prod \
     --memory 6.0Gi # Increase from 4.0Gi
   ```

2. **Profile memory usage**
   ```csharp
   // Use dotnet-counters to profile
   dotnet-counters monitor --process-id <pid>
   ```

### Slow Cold Start

**Symptoms**: First request takes > 10 seconds

**Solutions**:

1. **Set min replicas > 0**
   ```bash
   az containerapp update \
     --name ca-marketing-api \
     --resource-group rg-marketing-agents-prod \
     --min-replicas 2 # Keep 2 instances always running
   ```

2. **Use readiness probes**
   ```bash
   # Configure readiness probe to delay traffic until ready
   az containerapp update \
     --name ca-marketing-api \
     --resource-group rg-marketing-agents-prod \
     --set-env-vars "STARTUP_DELAY=false"
   ```

## Performance Issues

### API Response Time > 2 Seconds

**Investigation Steps**:

1. **Check Application Insights**
   - Review dependency duration (Cosmos DB, Redis, Azure OpenAI)
   - Identify slow queries

2. **Enable detailed telemetry**
   ```csharp
   // Add in Program.cs
   builder.Services.AddOpenTelemetry()
       .WithTracing(tracing => tracing
           .AddAspNetCoreInstrumentation(options =>
           {
               options.RecordException = true;
           }));
   ```

3. **Optimize queries**
   - Add indexes to Cosmos DB
   - Cache frequently accessed data in Redis
   - Use projection to select only needed fields

### SignalR Message Lag

**Symptoms**: Real-time updates delayed by > 1 second

**Solutions**:

1. **Use sticky sessions**
   ```bash
   # Enable session affinity in Azure Container Apps
   az containerapp ingress sticky-sessions set \
     --name ca-marketing-api \
     --resource-group rg-marketing-agents-prod \
     --affinity sticky
   ```

2. **Scale out SignalR**
   ```bash
   # Increase replicas
   az containerapp update \
     --name ca-marketing-api \
     --resource-group rg-marketing-agents-prod \
     --min-replicas 3 \
     --max-replicas 10
   ```

## Getting Help

If issues persist:

1. **Check logs** - Always start with application logs
2. **Review documentation** - Search this documentation
3. **GitHub Issues** - Search existing issues or create new one
4. **Team Chat** - Ask in team channel

## Next Steps

- [Development Workflow](development.md) - Development best practices
- [Deployment Guide](deployment.md) - Deploy to Azure
- [Configuration Reference](../reference/configuration.md) - All configuration options
