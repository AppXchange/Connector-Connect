namespace Connector.App.v1;
using Connector.App.v1.Files;
using Connector.App.v1.Projects;
using ESR.Hosting.CacheWriter;
using Json.Schema.Generation;

/// <summary>
/// Configuration for the Cache writer for this module. This configuration will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// The schema will be used for validation at runtime to make sure the configurations are properly formed. 
/// The schema also helps provide integrators more information for what the values are intended to be.
/// </summary>
[Title("App V1 Cache Writer Configuration")]
[Description("Configuration of the data object caches for the module.")]
public class AppV1CacheWriterConfig
{
    /// <summary>
    /// Configuration for the Projects Data Reader.
    /// </summary>
    [Title("Projects Cache Writer Configuration")]
    [Description("Configuration for the Projects data reader in the cache writer.")]
    public CacheWriterObjectConfig ProjectsConfig { get; set; } = new();
    public CacheWriterObjectConfig FilesConfig { get; set; } = new();
}