// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.Api.Data;
using MarketingAgents.Api.Models;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// Add Cosmos DB client via Aspire integration
builder.AddAzureCosmosClient("cosmosdb");

// Register database initialization service
const string DatabaseName = "marketingagents";
builder.Services.AddSingleton(sp =>
{
    var cosmosClient = sp.GetRequiredService<Microsoft.Azure.Cosmos.CosmosClient>();
    var logger = sp.GetRequiredService<ILogger<CosmosDbInitializationService>>();
    return new CosmosDbInitializationService(cosmosClient, DatabaseName, logger);
});

// Register repositories
builder.Services.AddScoped<ICampaignRepository>(sp =>
{
    var cosmosClient = sp.GetRequiredService<Microsoft.Azure.Cosmos.CosmosClient>();
    var versionRepo = sp.GetRequiredService<IVersionRepository>();
    var auditRepo = sp.GetRequiredService<IAuditReportRepository>();
    return new CampaignRepository(cosmosClient, DatabaseName, versionRepo, auditRepo);
});

builder.Services.AddScoped<IArtifactRepository>(sp =>
{
    var cosmosClient = sp.GetRequiredService<Microsoft.Azure.Cosmos.CosmosClient>();
    return new ArtifactRepository(cosmosClient, DatabaseName);
});

builder.Services.AddScoped<IVersionRepository>(sp =>
{
    var cosmosClient = sp.GetRequiredService<Microsoft.Azure.Cosmos.CosmosClient>();
    return new VersionRepository(cosmosClient, DatabaseName);
});

builder.Services.AddScoped<IAuditReportRepository>(sp =>
{
    var cosmosClient = sp.GetRequiredService<Microsoft.Azure.Cosmos.CosmosClient>();
    return new AuditReportRepository(cosmosClient, DatabaseName);
});

builder.Services.AddScoped<IIterationLogRepository>(sp =>
{
    var cosmosClient = sp.GetRequiredService<Microsoft.Azure.Cosmos.CosmosClient>();
    return new IterationLogRepository(cosmosClient, DatabaseName);
});

builder.Services.AddScoped<IOrchestrationRunRepository>(sp =>
{
    var cosmosClient = sp.GetRequiredService<Microsoft.Azure.Cosmos.CosmosClient>();
    return new OrchestrationRunRepository(cosmosClient, DatabaseName);
});

// Add services to the container
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize Cosmos DB database and containers in development
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbInitService = scope.ServiceProvider.GetRequiredService<CosmosDbInitializationService>();
    await dbInitService.InitializeAsync();
}

// Configure the HTTP request pipeline
app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Sample endpoint - will be replaced with actual campaign endpoints
var summaries = new[]
{
    "Freezing",
    "Bracing",
    "Chilly",
    "Cool",
    "Mild",
    "Warm",
    "Balmy",
    "Hot",
    "Sweltering",
    "Scorching",
};

app.MapGet(
    "/weatherforecast",
    () =>
{
    var forecast = Enumerable.Range(1, 5).Select(
        index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

await app.RunAsync();

// Make Program class accessible to tests
public partial class Program
{
}
