namespace Connector.App.v1.Files;

using Json.Schema.Generation;
using System;
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.CacheWriter;

/// <summary>
/// Represents a file or folder in the Xchange system.
/// </summary>
[PrimaryKey("id", nameof(Id))]
[Description("Represents a file or folder in the project.")]
public class FilesDataObject
{
    [JsonPropertyName("id")]
    [Description("The unique identifier of the file or folder.")]
    public required string Id { get; set; }

    [JsonPropertyName("vid")]
    [Description("The version ID of the file or folder.")]
    public required string Vid { get; set; }

    [JsonPropertyName("nm")]
    [Description("The name of the file or folder.")]
    public required string Name { get; set; }

    [JsonPropertyName("pid")]
    [Description("The parent ID of the file or folder.")]
    public required string ParentId { get; set; }

    // [JsonPropertyName("ptp")]
    // [Description("The parent type.")]
    // public string ParentType { get; set; }

    [JsonPropertyName("tp")]
    [Description("The type of object (FILE or FOLDER).")]
    public required string Type { get; set; }

    [JsonPropertyName("ct")]
    [Description("The creation timestamp of the file or folder.")]
    public DateTime CreatedTime { get; set; }

    [JsonPropertyName("mt")]
    [Description("The modification timestamp of the file or folder.")]
    public DateTime ModifiedTime { get; set; }

    [JsonPropertyName("cid")]
    [Description("The creator ID.")]
    public required string CreatorId { get; set; }

    [JsonPropertyName("mid")]
    [Description("The last modifier ID.")]
    public required string ModifierId { get; set; }

    [JsonPropertyName("sz")]
    [Description("The size of the file in bytes.")]
    public long Size { get; set; }

    [JsonPropertyName("del")]
    [Description("Indicates whether the file or folder is marked as deleted.")]
    public bool IsDeleted { get; set; }

    [JsonPropertyName("tn")]
    [Description("Thumbnail URL of the file.")]
    public required string Thumbnail { get; set; }

    [JsonPropertyName("md5")]
    [Description("MD5 hash of the file.")]
    public required string Md5 { get; set; }

    [JsonPropertyName("chid")]
    [Description("Change ID.")]
    public required string ChangeId { get; set; }

    [JsonPropertyName("cht")]
    [Description("Change timestamp.")]
    public DateTime ChangeTime { get; set; }

    [JsonPropertyName("rv")]
    [Description("Revision number.")]
    public int Revision { get; set; }

    [JsonPropertyName("uploadId")]
    [Description("The unique identifier for the upload session.")]
    public required string UploadId { get; set; }

    [JsonPropertyName("parentType")]
    [Description("Parent type identifier supported values are FOLDER|TODO|COMMENT|OBJECTLINK.")]
    public required string ParentType { get; set; }

    [JsonPropertyName("syncSessionId")]
    [Description("The sync session identifier.")]
    public required string SyncSessionId { get; set; }

    [JsonPropertyName("batchId")]
    [Description("The batch identifier for file processing.")]
    public required string BatchId { get; set; }

    [JsonPropertyName("format")]
    [Description("The format of the file.")]
    public required string Format { get; set; }

    [JsonPropertyName("parts")]
    [Description("The parts of the multipart upload.")]
    public required MultipartUploadPart[] Parts { get; set; }
}

public class MultipartUploadPart
{
    [JsonPropertyName("etag")]
    [Description("The ETag of the part.")]
    public required string Etag { get; set; }

    [JsonPropertyName("part_number")]
    [Description("The part number of the part.")]
    public int PartNumber { get; set; }
}
