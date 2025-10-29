// Copyright (c) Marketing Agents. All rights reserved.

using MarketingAgents.AgentHost.Models.Configuration;
using MarketingAgents.AgentHost.Orchestration;
using MarketingAgents.AgentHost.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults (OpenTelemetry, health checks, service discovery, resilience)
builder.AddServiceDefaults();

// Configure options from appsettings.json and user secrets
builder.Services.Configure<AgentConfiguration>(
    builder.Configuration.GetSection("Agent"));

builder.Services.Configure<OpenAIConfiguration>(
    builder.Configuration.GetSection("OpenAI"));

builder.Services.Configure<OrchestrationConfiguration>(
    builder.Configuration.GetSection("Orchestration"));

// Add Azure AI Foundry chat client via Aspire integration
// Connection string format: Endpoint=https://{endpoint}/;DeploymentId={deploymentName}
// Aspire automatically configures from connection string "chat"
builder.AddAzureChatCompletionsClient("chat");

// Register IChatClient for agent framework
// This is automatically configured by AddAzureChatCompletionsClient
// and available for dependency injection in agent providers

// Register core agent services
builder.Services.AddSingleton<RetryPolicyService>();
builder.Services.AddScoped<IAgentOrchestrator, AgentOrchestrator>();

var app = builder.Build();

// Map health check endpoints
app.MapDefaultEndpoints();

app.MapGet("/", () => "MarketingAgents AgentHost Service")
    .WithName("GetRoot");

await app.RunAsync();
