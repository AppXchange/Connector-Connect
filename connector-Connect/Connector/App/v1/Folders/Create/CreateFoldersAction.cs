namespace Connector.App.v1.Folders.Create;

using Json.Schema.Generation;
using System;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Action;
using System.Collections.Generic;

/// <summary>
/// Action object that will represent an action in the Xchange system. This will contain an input object type,
/// an output object type, and an Action failure type (this will default to <see cref="StandardActionFailure"/>
/// but that can be overridden with your own preferred type). These objects will be converted to a JsonSchema, 
/// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
/// These types will be used for validation at runtime to make sure the objects being passed through the system 
/// are properly formed. The schema also helps provide integrators more information for what the values 
/// are intended to be.
/// </summary>
[Description("CreateFoldersAction Action description goes here")]
public class CreateFoldersAction : IStandardAction<CreateFoldersActionInput, CreateFoldersActionOutput>
{
    public CreateFoldersActionInput ActionInput { get; set; } = new CreateFoldersActionInput
    {
        Name = string.Empty,
        ParentId = string.Empty
    };

    public CreateFoldersActionOutput ActionOutput { get; set; } = new CreateFoldersActionOutput
    {
        Name = string.Empty,
        ParentId = string.Empty,
        CreatedBy = new User
        {
            Id = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty,
            Status = string.Empty
        },
        ModifiedBy = new User
        {
            Id = string.Empty,
            FirstName = string.Empty,
            LastName = string.Empty,
            Email = string.Empty,
            Status = string.Empty
        },
        ProjectId = string.Empty
    };

    public StandardActionFailure ActionFailure { get; set; } = new();

    public bool CreateRtap => true;
}

public class CreateFoldersActionInput
{
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("parentId")]
    public required string ParentId { get; set; }

    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = new();
    public object? UploadId { get; internal set; }

}

public class CreateFoldersActionOutput
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; } = "FOLDER";

    [JsonPropertyName("versionId")]
    public Guid VersionId { get; set; }

    [JsonPropertyName("parentId")]
    public required string ParentId { get; set; }

    [JsonPropertyName("parentType")]
    public string ParentType { get; set; } = "FOLDER";

    [JsonPropertyName("permission")]
    public string Permission { get; set; } = "FULL_ACCESS";

    [JsonPropertyName("createdBy")]
    public required User CreatedBy { get; set; }

    [JsonPropertyName("createdOn")]
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("modifiedOn")]
    public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;

    [JsonPropertyName("modifiedBy")]
    public required User ModifiedBy { get; set; }

    [JsonPropertyName("size")]
    public long Size { get; set; }

    [JsonPropertyName("projectId")]
    public required string ProjectId { get; set; }

    [JsonPropertyName("hasChildren")]
    public bool HasChildren { get; set; }

    [JsonPropertyName("path")]
    public List<PathElement> Path { get; set; } = new();

    [JsonPropertyName("tags")]
    public List<Tag> Tags { get; set; } = new();
}

public class Tag
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("label")]
    public required string Label { get; set; }

    [JsonPropertyName("description")]
    public required string Description { get; set; }
}

public class User
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("firstName")]
    public required string FirstName { get; set; }

    [JsonPropertyName("lastName")]
    public required string LastName { get; set; }

    [JsonPropertyName("email")]
    public required string Email { get; set; }

    [JsonPropertyName("status")]
    public required string Status { get; set; }
}

public class PathElement
{
    [JsonPropertyName("id")]
    public required string Id { get; set; }

    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [JsonPropertyName("versionId")]
    public Guid VersionId { get; set; }
}
