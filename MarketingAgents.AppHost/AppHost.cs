// Copyright (c) Marketing Agents. All rights reserved.

var builder = DistributedApplication.CreateBuilder(args);

// Add Redis cache for local development
var cache = builder.AddRedis("cache");

// Add Cosmos DB emulator for local development
var cosmosDb = builder.AddAzureCosmosDB("cosmosdb")
    .RunAsPreviewEmulator(
                     emulator => emulator.WithDataExplorer())
    .AddCosmosDatabase("marketingagents");

// Add API service with references to dependencies
var apiService = builder.AddProject<Projects.MarketingAgents_Api>("apiservice")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(cosmosDb)
    .WaitFor(cosmosDb)
    .WithExternalHttpEndpoints();

// Add AgentHost service with references to dependencies
builder.AddProject<Projects.MarketingAgents_AgentHost>("agenthost")
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(cosmosDb)
    .WaitFor(cosmosDb)
    .WithReference(apiService);

// Add Next.js frontend with reference to API service
builder.AddNpmApp("webapp", "../MarketingAgents.Web", "dev")
    .WithReference(apiService)
    .WaitFor(apiService)
    .WithHttpEndpoint(env: "PORT", port: 3000)
    .WithExternalHttpEndpoints()
    .PublishAsDockerFile();

await builder.Build().RunAsync();
