namespace Connector.App.v1.Files.CompleteUpload
{
    using Json.Schema.Generation;
    using System;
    using System.Text.Json.Serialization;
    using Xchange.Connector.SDK.Action;

    /// <summary>
    /// Action object that will represent an action in the Xchange system. This will contain an input object type,
    /// an output object type, and a Action failure type (this will default to <see cref="StandardActionFailure"/>
    /// but that can be overridden with your own preferred type). These objects will be converted to a JsonSchema, 
    /// so add attributes to the properties to provide any descriptions, titles, ranges, max, min, etc... 
    /// These types will be used for validation at runtime to make sure the objects being passed through the system 
    /// are properly formed. The schema also helps provide integrators more information for what the values 
    /// are intended to be.
    /// </summary>
    [Description("CompleteUploadFilesAction Action description goes here")]
    public class CompleteUploadFilesAction : IStandardAction<CompleteUploadFilesActionInput, CompleteUploadFilesActionOutput>
    {
        public CompleteUploadFilesActionInput ActionInput { get; set; } = new CompleteUploadFilesActionInput(); // Ensuring input is not null
        public CompleteUploadFilesActionOutput ActionOutput { get; set; } = new CompleteUploadFilesActionOutput(); // Ensuring output is not null
        public StandardActionFailure ActionFailure { get; set; } = new StandardActionFailure(); // Ensuring failure is not null

        public bool CreateRtap => true;
    }

    public class CompleteUploadFilesActionInput
    {
        [JsonPropertyName("format")]
        [Description("The format of the file.")]
        public string Format { get; set; } = string.Empty; // Setting a default value to prevent null

        [JsonPropertyName("parts")]
        [Description("The parts of the multipart upload.")]
        public MultipartUploadPart[] Parts { get; set; } = Array.Empty<MultipartUploadPart>(); // Setting a default value to prevent null

        [JsonPropertyName("uploadId")]
        [Description("The unique identifier for the upload.")]
        public string UploadId { get; set; } = string.Empty; // Default to empty string or any non-nullable default value
    }

    public class CompleteUploadFilesActionOutput
    {
        [JsonPropertyName("id")]
        [Description("The unique identifier for the completed upload.")]
        public Guid Id { get; set; } = Guid.NewGuid(); // Providing a default value for a non-nullable type
    }

    public class MultipartUploadPart
    {
        [JsonPropertyName("etag")]
        [Description("The ETag of the part.")]
        public string Etag { get; set; } = string.Empty; // Setting a default value to prevent null

        [JsonPropertyName("part_number")]
        [Description("The part number of the part.")]
        public int PartNumber { get; set; }
    }
}
