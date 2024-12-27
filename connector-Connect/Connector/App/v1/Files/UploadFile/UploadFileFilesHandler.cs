using ESR.Hosting.Action;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xchange.Connector.SDK.Action;
using Xchange.Connector.SDK.CacheWriter;
using Connector.Client;
using Connector.App.v1.Files.UploadFile;
using ESR.Hosting.CacheWriter;

namespace Connector.App.v1.Files.UploadFile
{
    public class UploadFileFilesHandler : IActionHandler<UploadFileFilesAction>
    {
        private readonly ILogger<UploadFileFilesHandler> _logger;
        private readonly ApiClient _apiClient;

        public UploadFileFilesHandler(
            ILogger<UploadFileFilesHandler> logger,
            ApiClient apiClient)
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(ActionInstance actionInstance, CancellationToken cancellationToken)
        {
            if (actionInstance == null || string.IsNullOrWhiteSpace(actionInstance.InputJson))
            {
                _logger.LogError("Invalid action instance or input JSON.");
                return ActionHandlerOutcome.Failed(new StandardActionFailure
                {
                    Code = "400",
                    Errors = new[]
                    {
                        new Xchange.Connector.SDK.Action.Error
                        {
                            Source = new[] { "UploadFileFilesHandler" },
                            Text = "Invalid action instance or input JSON."
                        }
                    }
                });
            }

            var input = JsonSerializer.Deserialize<UploadFileFilesActionInput>(actionInstance.InputJson);
            if (input == null)
            {
                _logger.LogError("Failed to deserialize input JSON.");
                return ActionHandlerOutcome.Failed(new StandardActionFailure
                {
                    Code = "400",
                    Errors = new[]
                    {
                        new Xchange.Connector.SDK.Action.Error
                        {
                            Source = new[] { "UploadFileFilesHandler" },
                            Text = "Failed to deserialize input JSON."
                        }
                    }
                });
            }

            try
            {
                using (Stream fileStream = new MemoryStream(input.FileContent)) // Explicit cast
                {
                    if (fileStream == null)
                    {
                        throw new InvalidOperationException("FileStream is unexpectedly null.");
                    }

                    _logger.LogInformation("Uploading file to parent ID: {ParentId}", input.ParentId);

                    var response = await _apiClient.UploadFileAsync(
                        fileStream, 
                        input.ParentId, 
                        input.ParentType, 
                        input.SyncSessionId ?? string.Empty, // Handle optional params
                        input.BatchId ?? string.Empty,
                        cancellationToken);

                    if (!response.IsSuccessful)
                    {
                        _logger.LogError("File upload failed with status code: {StatusCode}", response.StatusCode);
                        return ActionHandlerOutcome.Failed(new StandardActionFailure
                        {
                            Code = response.StatusCode.ToString(),
                            Errors = new[]
                            {
                                new Xchange.Connector.SDK.Action.Error
                                {
                                    Source = new[] { "UploadFileFilesHandler" },
                                    Text = $"File upload failed with status code: {response.StatusCode}"
                                }
                            }
                        });
                    }

                    if (response.Data == null || string.IsNullOrEmpty(response.Data.FileId))
                    {
                        _logger.LogError("Failed to retrieve upload details.");
                        return ActionHandlerOutcome.Failed(new StandardActionFailure
                        {
                            Code = "500",
                            Errors = new[]
                            {
                                new Xchange.Connector.SDK.Action.Error
                                {
                                    Source = new[] { "UploadFileFilesHandler" },
                                    Text = "Failed to retrieve upload details."
                                }
                            }
                        });
                    }

                    var uploadDetails = response.Data;

                    _logger.LogInformation("File uploaded successfully. FileId: {FileId}", uploadDetails.FileId);

                    var operations = new List<ESR.Hosting.CacheWriter.SyncOperation>
                    {
                        new ESR.Hosting.CacheWriter.SyncOperation(OperationType.Upsert.ToString(), uploadDetails.FileId, new Dictionary<string, object>
                        {
                            { "FileId", uploadDetails.FileId },
                            { "Status", uploadDetails.Status },
                            { "VersionId", uploadDetails.VersionId },
                            { "Revision", uploadDetails.Revision },
                            { "ProjectId", uploadDetails.ProjectId }
                        })
                    };

                    var cacheSync = new List<CacheSyncCollection>
                    {
                        new CacheSyncCollection
                        {
                            DataObjectType = typeof(UploadFileFilesActionOutput),
                            CacheChanges = operations.ToArray()
                        }
                    };

                    return ActionHandlerOutcome.Successful(uploadDetails, cacheSync);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while handling the file upload.");
                return ActionHandlerOutcome.Failed(new StandardActionFailure
                {
                    Code = "500",
                    Errors = new[]
                    {
                        new Xchange.Connector.SDK.Action.Error
                        {
                            Source = new[] { "UploadFileFilesHandler" },
                            Text = ex.Message
                        }
                    }
                });
            }
        }
    }
}
