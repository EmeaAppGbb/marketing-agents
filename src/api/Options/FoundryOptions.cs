namespace Api.Options;

/// <summary>
/// Configuration options for Azure AI Foundry integration.
/// </summary>
public sealed record FoundryOptions
{
    /// <summary>
    /// The configuration section name in appsettings.json.
    /// </summary>
    public const string SectionName = "Foundry";

    /// <summary>
    /// The Azure AI Foundry endpoint URL.
    /// </summary>
    public required string Endpoint { get; init; }

    /// <summary>
    /// The deployment model name to use for chat completions.
    /// </summary>
    public required string ModelDeploymentName { get; init; }

    /// <summary>
    /// The Azure credential scope for authentication.
    /// Default: https://cognitiveservices.azure.com/.default
    /// </summary>
    public string CredentialScope { get; init; } = "https://ai.azure.com/.default";
}
