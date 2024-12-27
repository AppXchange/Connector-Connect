using System.Text.Json.Serialization;
using System.Collections.Generic; // Added for List<>

namespace Connector;

/// <summary>
/// Contains all configuration values necessary for execution of the connector, configurable by a connector implementation.
/// </summary>
public class ConnectorRegistrationConfig
{
    /// <summary>
    /// The base URL for the Trimble Connect API.
    /// </summary>
    [JsonPropertyName("baseUrl")]
    public string BaseUrl { get; set; } = "https://connect.trimble.com";

    /// <summary>
    /// The API key or authentication token used to authenticate requests to Trimble Connect.
    /// </summary>
    [JsonPropertyName("apiKey")]
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// The region to use for Trimble Connect API calls (e.g., "northAmerica" or "europe").
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = "northAmerica";

    /// <summary>
    /// The retry count for failed API calls.
    /// </summary>
    [JsonPropertyName("retryCount")]
    public int RetryCount { get; set; } = 3;

    /// <summary>
    /// Timeout in seconds for API calls to Trimble Connect.
    /// </summary>
    [JsonPropertyName("timeoutSeconds")]
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Optional: Enable detailed logging for debugging purposes.
    /// </summary>
    [JsonPropertyName("enableDebugLogging")]
    public bool EnableDebugLogging { get; set; } = false;

    /// <summary>
    /// Optional: A list of project IDs to filter for specific integrations.
    /// </summary>
    [JsonPropertyName("projectFilters")]
    public List<string> ProjectFilters { get; set; } = new();
}
