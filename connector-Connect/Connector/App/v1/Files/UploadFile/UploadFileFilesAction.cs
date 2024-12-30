namespace Connector.App.v1.Files.UploadFile;

using System;
using System.Text.Json.Serialization;
using System.ComponentModel; // Added for DescriptionAttribute
using Xchange.Connector.SDK.Action;
using System.Collections.Generic;


/// <summary>
/// Action object for uploading files in the Xchange system. This action represents the file upload process,
/// including support for multipart uploads and filesets.
/// </summary>
[Description("Handles file uploads with support for multipart and filesets.")]
public class UploadFileFilesAction : IStandardAction<UploadFileFilesActionInput, UploadFileFilesActionOutput>
{
    public UploadFileFilesActionInput ActionInput { get; set; } = new();
    public UploadFileFilesActionOutput ActionOutput { get; set; } = new();
    public StandardActionFailure ActionFailure { get; set; } = new();

    /// <summary>
    /// Indicates whether RTAP should be created.
    /// </summary>
    public bool CreateRtap => true;
}

/// <summary>
/// Input object for UploadFileFilesAction.
/// </summary>
public class UploadFileFilesActionInput
{
    /// <summary>
    /// The name of the file to be uploaded.
    /// </summary>
    [JsonPropertyName("name")]
    [Description("The name of the file to be uploaded.")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The size of the file in bytes.
    /// </summary>
    [JsonPropertyName("size")]
    [Description("The size of the file in bytes.")]
    public long Size { get; set; }

    /// <summary>
    /// The type of file to be uploaded.
    /// </summary>
    [JsonPropertyName("type")]
    [Description("The type of file to be uploaded.")]
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The format of the file (e.g., PDF, TRB, THUMB).
    /// </summary>
    [JsonPropertyName("format")]
    [Description("The format of the file, such as PDF, TRB, or THUMB.")]
    public string Format { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the file is part of a multipart upload.
    /// </summary>
    [JsonPropertyName("multipart")]
    [Description("Indicates if the file is part of a multipart upload.")]
    public bool Multipart { get; set; }

    /// <summary>
    /// Indicates if the file is part of a fileset.
    /// </summary>
    [JsonPropertyName("fileset")]
    [Description("Indicates if the file is part of a fileset.")]
    public bool Fileset { get; set; }

    /// <summary>
    /// The parent identifier to which the file has to be uploaded.
    /// </summary>
    [JsonPropertyName("parentId")]
    [Description("The parent identifier where the file will be uploaded.")]
    public string ParentId { get; set; } = string.Empty;

    /// <summary>
    /// The parent type identifier.
    /// Supported values are FOLDER, TODO, COMMENT, OBJECTLINK.
    /// </summary>
    [JsonPropertyName("parentType")]
    [Description("The type of the parent to which the file belongs.")]
    public string ParentType { get; set; } = "FOLDER";

    /// <summary>
    /// The URL to upload the file.
    /// </summary>
    [JsonPropertyName("url")]
    [Description("The pre-signed URL for uploading the file.")]
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// The path template for the file.
    /// </summary>
    [JsonPropertyName("path_template")]
    [Description("The path template for the uploaded file.")]
    public string PathTemplate { get; set; } = string.Empty;

    /// <summary>
    /// The file content.
    /// </summary>
    [JsonPropertyName("fileContent")]
    [Description("The binary content of the file.")]
    public byte[] FileContent { get; set; } = Array.Empty<byte>();

    /// <summary>
    /// The Session ID.
    /// </summary>
    [JsonPropertyName("syncSessionId")]
    [Description("The synchronization session ID.")]
    public string SyncSessionId { get; set; } = string.Empty;

    /// <summary>
    /// The Baatch ID.
    /// </summary>
    [JsonPropertyName("batchId")]
    [Description("The batch ID for multi-file uploads.")]
    public string BatchId { get; set; } = string.Empty;
}

/// <summary>
/// Output object for UploadFileFilesAction.
/// </summary>
public class UploadFileFilesActionOutput
{
    /// <summary>
    /// The unique identifier for the uploaded file.
    /// </summary>
    [JsonPropertyName("fileId")]
    [Description("The unique identifier for the uploaded file.")]
    public string FileId { get; set; } = string.Empty;

    /// <summary>
    /// The upload status.
    /// </summary>
    [JsonPropertyName("status")]
    [Description("The status of the file upload.")]
    public string Status { get; set; } = "UPLOADABLE";

    /// <summary>
    /// The version identifier for the uploaded file.
    /// </summary>
    [JsonPropertyName("versionId")]
    [Description("The version identifier for the uploaded file.")]
    public string VersionId { get; set; } = string.Empty;

    /// <summary>
    /// The revision number of the uploaded file.
    /// </summary>
    [JsonPropertyName("revision")]
    [Description("The revision number of the uploaded file.")]
    public int Revision { get; set; } = 0;

    /// <summary>
    /// The project identifier where the file is uploaded.
    /// </summary>
    [JsonPropertyName("projectId")]
    [Description("The project identifier associated with the file upload.")]
    public string ProjectId { get; set; } = string.Empty;
}

public enum OperationType
{
    Upsert,
    Delete,
    Update
}

public class SyncOperation(OperationType operationType, string objectId, Dictionary<string, object> fields)
{
    public OperationType OperationType { get; set; } = operationType;
    public string ObjectId { get; set; } = objectId;
    public Dictionary<string, object> Fields { get; set; } = fields;
}

public class SourceFileUpload
{
    public required string FileId { get; set; }
    public required string Status { get; set; }
    public required string VersionId { get; set; }
    public int Revision { get; set; }
    public required string ProjectId { get; set; }
}

public class FileUploadContentDto
{
    public required string ParentId { get; set; }
    public required string ParentType { get; set; }
    public required string SyncSessionId { get; set; }
    public required string BatchId { get; set; }
}
