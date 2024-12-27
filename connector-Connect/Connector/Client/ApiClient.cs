using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Connector.App.v1.Projects; 
using Connector.App.v1.Folders.Create;
using Connector.App.v1.Files;
using Connector.App.v1.Files.CompleteUpload;
using Connector.App.v1.Files.UploadFile;
using System.Linq;


namespace Connector.Client
{
    /// <summary>
    /// A client for interfacing with the Trimble Connect API.
    /// </summary>
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiClient"/> class.
        /// </summary>
        /// <param name="httpClient">The HTTP client instance.</param>
        /// <param name="baseUrl">The base URL for the API.</param>
        public ApiClient(HttpClient httpClient, string baseUrl)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new System.Uri(baseUrl);
        }

        /// <summary>
        /// Retrieves a paginated response from the specified endpoint.
        /// </summary>
        /// <typeparam name="T">The type of the data items in the paginated response.</typeparam>
        /// <param name="relativeUrl">The relative URL of the API endpoint.</param>
        /// <param name="page">The page number to retrieve.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> containing the paginated data.</returns>
        public async Task<ApiResponse<PaginatedResponse<T>>> GetRecords<T>(string relativeUrl, int page, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetAsync($"{relativeUrl}?page={page}", cancellationToken: cancellationToken).ConfigureAwait(false);

            return new ApiResponse<PaginatedResponse<T>>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Data = response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<PaginatedResponse<T>>(cancellationToken: cancellationToken)
                    : default,
                RawResult = await response.Content.ReadAsStreamAsync(cancellationToken: cancellationToken)
            };
        }

        /// <summary>
        /// Sends a GET request to a no-content endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An <see cref="ApiResponse"/> representing the result.</returns>
        public async Task<ApiResponse> GetNoContent(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
                .GetAsync("no-content", cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return new ApiResponse
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                RawResult = await response.Content.ReadAsStreamAsync(cancellationToken: cancellationToken)
            };
        }

        /// <summary>
        /// Tests the connection to the API by sending a GET request to the "projects" endpoint.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An <see cref="ApiResponse"/> representing the result.</returns>
        public async Task<ApiResponse> TestConnection(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
                .GetAsync("projects", cancellationToken: cancellationToken)
                .ConfigureAwait(false);

            return new ApiResponse
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode
            };
        }

        /// <summary>
        /// Retrieves a list of projects from the API.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{List{ProjectsDataObject}}"/> representing the result.</returns>
        public async Task<ApiResponse<List<ProjectsDataObject>>> GetProjectsAsync(CancellationToken cancellationToken = default)
        {
            // Define the API endpoint
            const string endpoint = "projects";

            try
            {
                // Send the GET request
                var response = await _httpClient
                    .GetAsync(endpoint, cancellationToken)
                    .ConfigureAwait(false);

                // Deserialize the response into the appropriate object if successful
                if (response.IsSuccessStatusCode)
                {
                    var projects = await response
                        .Content
                        .ReadFromJsonAsync<List<ProjectsDataObject>>(cancellationToken: cancellationToken)
                        .ConfigureAwait(false) ?? new List<ProjectsDataObject>();

                    return new ApiResponse<List<ProjectsDataObject>>
                    {
                        IsSuccessful = true,
                        StatusCode = (int)response.StatusCode,
                        Data = projects
                    };
                }

                // Return an unsuccessful response with the status code if the call failed
                return new ApiResponse<List<ProjectsDataObject>>
                {
                    IsSuccessful = false,
                    StatusCode = (int)response.StatusCode,
                    Data = null
                };
            }
            catch (Exception)
            {
                // Handle unexpected exceptions
                return new ApiResponse<List<ProjectsDataObject>>
                {
                    IsSuccessful = false,
                    StatusCode = 500,
                };
            }
        }

        /// <summary>
        /// Retrieves the latest files and folders structure from a project.
        /// </summary>
        /// <param name="projectId">The ID of the project.</param>
        /// <param name="includeDeleted">Whether to include deleted files and folders.</param>
        /// <param name="includeAttachment">Whether to include file attachments.</param>
        /// <param name="skipToken">Cursor for pagination.</param>
        /// <param name="maxItems">Maximum number of items to retrieve.</param>
        /// <param name="objectTypes">Comma-separated types to filter by (e.g., FILE,FOLDER).</param>
        /// <param name="objectNames">Comma-separated names to filter by (e.g., file1,file2).</param>
        /// <returns>An <see cref="ApiResponse{List{FilesDataObject}}"/> representing the result.</returns>
        public async Task<ApiResponse<List<FilesDataObject>>> GetFilesSnapshot(
            string projectId,
            bool includeDeleted = false,
            bool includeAttachment = false,
            string? skipToken = null,
            int maxItems = 100000,
            string objectTypes = "FILE,FOLDER",
            string? objectNames = null)
        {
            var queryParameters = new Dictionary<string, string>
            {
                { "projectId", projectId },
                { "includeDeleted", includeDeleted.ToString().ToLower() },
                { "includeAttachment", includeAttachment.ToString().ToLower() },
                { "maxItems", maxItems.ToString() },
                { "objectTypes", objectTypes }
            };

            if (!string.IsNullOrEmpty(skipToken))
                queryParameters.Add("skipToken", skipToken);

            if (!string.IsNullOrEmpty(objectNames))
                queryParameters.Add("objectNames", objectNames);

            var queryString = string.Join("&", queryParameters.Select(kv => $"{kv.Key}={Uri.EscapeDataString(kv.Value)}"));

            var response = await _httpClient
                .GetAsync($"files/fs/snapshot?{queryString}")
                .ConfigureAwait(false);

            return new ApiResponse<List<FilesDataObject>>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Data = response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<List<FilesDataObject>>()
                    : default
            };
        }

        /// <summary>
        /// Creates a new folder in the API.
        /// </summary>
        /// <param name="input">The input for creating the folder.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{CreateFoldersActionOutput}"/> representing the result.</returns>
        public async Task<ApiResponse<CreateFoldersActionOutput>> PostFoldersDataObject(
            Connector.App.v1.Folders.Create.CreateFoldersActionInput input, 
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
                .PostAsJsonAsync("folders", input, cancellationToken)
                .ConfigureAwait(false);

            return new ApiResponse<CreateFoldersActionOutput>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Data = response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<CreateFoldersActionOutput>(cancellationToken: cancellationToken)
                    : default,
                RawResult = await response.Content.ReadAsStreamAsync(cancellationToken: cancellationToken)
            };
        }

        /// <summary>
        /// Uploads a file to the specified endpoint.
        /// </summary>
        /// <param name="fileContent">The content of the file to upload.</param>
        /// <param name="parentId">The parent ID to which the file should be uploaded.</param>
        /// <param name="parentType">The type of the parent (e.g., FOLDER).</param>
        /// <param name="syncSessionId">The sync session ID.</param>
        /// <param name="batchId">The batch ID for multi-file uploads.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{SourceFileUpload}"/> representing the result.</returns>
        public async Task<ApiResponse<SourceFileUpload>> UploadFileAsync(
            Stream fileContent,
            string parentId,
            string parentType,
            string? syncSessionId = null,
            string? batchId = null,
            CancellationToken cancellationToken = default)
        {
            var fileUploadContent = new FileUploadContentDto
            {
                ParentId = parentId,
                ParentType = parentType,
                SyncSessionId = syncSessionId ?? string.Empty,  // Default to an empty string if null
                BatchId = batchId ?? string.Empty  // Default to an empty string if null
            };

            using var content = new MultipartFormDataContent
            {
                { new StreamContent(fileContent), "file", "sourceFile.skp" }
            };

            var response = await _httpClient
                .PostAsync("files/fs/upload", content, cancellationToken)
                .ConfigureAwait(false);

            return new ApiResponse<SourceFileUpload>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Data = response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<SourceFileUpload>(cancellationToken: cancellationToken)
                    : default,
                RawResult = await response.Content.ReadAsStreamAsync(cancellationToken: cancellationToken)
            };
        }
        
        /// <summary>
        /// Completes an upload for a specific file.
        /// </summary>
        /// <param name="uploadId">The ID of the upload to complete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{CompleteUploadFilesActionOutput}"/> representing the result.</returns>
        public async Task<ApiResponse<CompleteUploadFilesActionOutput>> CompleteFileUploadAsync(
            string uploadId,
            CancellationToken cancellationToken = default)
        {
            var response = await _httpClient
                .PostAsync($"files/fs/upload/{uploadId}/complete", null, cancellationToken)
                .ConfigureAwait(false);

            return new ApiResponse<CompleteUploadFilesActionOutput>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Data = response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<CompleteUploadFilesActionOutput>(cancellationToken: cancellationToken)
                    : default,
                RawResult = await response.Content.ReadAsStreamAsync(cancellationToken: cancellationToken)
            };
        }

        /// <summary>
        /// Posts a file data object to the API.
        /// </summary>
        /// <param name="input">The input data object to post.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>An <see cref="ApiResponse{T}"/> representing the result.</returns>
        public async Task<ApiResponse<FilesDataObject>> PostFilesDataObjectAsync(FilesDataObject input, CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.PostAsJsonAsync("files/data-object", input, cancellationToken).ConfigureAwait(false);

            return new ApiResponse<FilesDataObject>
            {
                IsSuccessful = response.IsSuccessStatusCode,
                StatusCode = (int)response.StatusCode,
                Data = response.IsSuccessStatusCode
                    ? await response.Content.ReadFromJsonAsync<FilesDataObject>(cancellationToken: cancellationToken)
                    : default,
                RawResult = await response.Content.ReadAsStreamAsync(cancellationToken: cancellationToken)
            };
        }

    }
}
