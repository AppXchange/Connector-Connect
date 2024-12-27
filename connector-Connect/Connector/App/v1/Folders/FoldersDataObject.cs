namespace Connector.App.v1.Folders;

using Json.Schema.Generation;
using System;
using System.Collections.Generic; // Added for List<>
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.CacheWriter;

/// <summary>
/// Data object that represents a folder in the Xchange system.
/// This will be converted to a JsonSchema for validation at runtime.
/// </summary>
[PrimaryKey("id", nameof(Id))]
[Description("Represents a folder object in the Xchange system.")]
public class FoldersDataObject
{
    /// <summary>
    /// The unique identifier for the folder.
    /// </summary>
    [JsonPropertyName("id")]
    [Description("Unique identifier of the folder.")]
    public required Guid Id { get; init; }

    /// <summary>
    /// The name of the folder.
    /// </summary>
    [JsonPropertyName("name")]
    [Description("Name of the folder.")]
    public required string Name { get; set; }

    /// <summary>
    /// The identifier of the parent folder.
    /// </summary>
    [JsonPropertyName("parentId")]
    [Description("Parent folder's version ID.")]
    public required string ParentId { get; set; }

    /// <summary>
    /// List of tags associated with the folder.
    /// </summary>
    [JsonPropertyName("tags")]
    [Description("List of tags linked with the folder.")]
    public List<Tag> Tags { get; set; } = new(); // Initialized to avoid null references
}

/// <summary>
/// Represents a tag associated with a folder.
/// </summary>
public class Tag
{
    /// <summary>
    /// The identifier of the tag.
    /// </summary>
    [JsonPropertyName("id")]
    [Description("The identifier for the tag.")]
    public required string Id { get; set; }

    /// <summary>
    /// The label of the tag.
    /// </summary>
    [JsonPropertyName("label")]
    [Description("The label of the tag.")]
    public required string Label { get; set; }

    /// <summary>
    /// A description of the tag.
    /// </summary>
    [JsonPropertyName("description")]
    [Description("Description of the tag.")]
    public required string Description { get; set; }
}
