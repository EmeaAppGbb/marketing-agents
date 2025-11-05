using System.ClientModel.Primitives;
using Api.Options;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Options;
using OpenAI;
using OpenAI.Chat;

var builder = WebApplication.CreateBuilder(args);

// Configure options
builder.Services.AddOptions<FoundryOptions>()
    .BindConfiguration(FoundryOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
     app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", async (IOptions<FoundryOptions> foundryOptions) =>
{
    var options = foundryOptions.Value;
    var clientOptions = new OpenAIClientOptions() { Endpoint = new Uri(options.Endpoint) };

    BearerTokenPolicy tokenPolicy = new(
    new AzureDeveloperCliCredential(),
    options.CredentialScope);

#pragma warning disable OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
    ChatClient client = new(
        authenticationPolicy: tokenPolicy,
        model: options.ModelDeploymentName,
        options: clientOptions);
#pragma warning restore OPENAI001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.





    AIAgent agent = client.CreateAIAgent(
    instructions: "You are good at telling jokes.",
    name: "Joker");

    var response = await agent.RunAsync("Tell me a joke about a pirate.");
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            response.Text
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
