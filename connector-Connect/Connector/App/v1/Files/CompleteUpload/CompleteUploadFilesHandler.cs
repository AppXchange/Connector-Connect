using Connector.Client;
using ESR.Hosting.Action;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xchange.Connector.SDK.Action;
using Xchange.Connector.SDK.CacheWriter;
using Xchange.Connector.SDK.Client.AppNetwork;

namespace Connector.App.v1.Files.CompleteUpload
{
    // Action handler for completing file upload
    public class CompleteUploadFilesHandler(
        ILogger<CompleteUploadFilesHandler> logger,
        ApiClient apiClient) : IActionHandler<CompleteUploadFilesAction>
    {
        private readonly ILogger<CompleteUploadFilesHandler> _logger = logger;
        private readonly ApiClient _apiClient = apiClient;

        public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(ActionInstance actionInstance, CancellationToken cancellationToken)
        {
            var input = JsonSerializer.Deserialize<CompleteUploadFilesActionInput>(actionInstance.InputJson);
            if (input == null)
            {
                return ActionHandlerOutcome.Failed(new StandardActionFailure
                {
                    Code = "400",
                    Errors =
                    [
                        new Xchange.Connector.SDK.Action.Error
                        {
                            Source = ["CompleteUploadFilesHandler"],
                            Text = "Input deserialization failed or input is null."
                        }
                    ]
                });
            }

            try
            {
                // Make a call to the API using ApiClient
                var response = await _apiClient.CompleteFileUploadAsync(input.UploadId, cancellationToken).ConfigureAwait(false);

                if (response.Data == null)
                {
                    return ActionHandlerOutcome.Failed(new StandardActionFailure
                    {
                        Code = "500",
                        Errors =
                        [
                            new Xchange.Connector.SDK.Action.Error
                            {
                                Source = ["CompleteUploadFilesHandler"],
                                Text = "API response did not contain valid data."
                            }
                        ]
                    });
                }

                // Use the response directly as the action output
                var result = response.Data;

                // Build sync operations to update the local cache and the Xchange cache system
                var operations = new List<SyncOperation>();
                var keyResolver = new DefaultDataObjectKey();

                try
                {
                    var (UrlPart, PropertyNames) = keyResolver.BuildKeyResolver()(result);
                    operations.Add(SyncOperation.CreateSyncOperation(UpdateOperation.Upsert.ToString(), UrlPart, PropertyNames, result));
                }
                catch (System.Exception ex)
                {
                    return ActionHandlerOutcome.Failed(new StandardActionFailure
                    {
                        Code = "500",
                        Errors =
                        [
                            new Xchange.Connector.SDK.Action.Error
                            {
                                Source = ["CompleteUploadFilesHandler"],
                                Text = $"Error resolving key: {ex.Message}"
                            }
                        ]
                    });
                }

                var resultList = new List<CacheSyncCollection>
                {
                    new() { DataObjectType = typeof(FilesDataObject), CacheChanges = operations.ToArray() }
                };

                return ActionHandlerOutcome.Successful(result, resultList);
            }
            catch (HttpRequestException exception)
            {
                // Create a failure result for the action
                var errorSource = new List<string> { "CompleteUploadFilesHandler" };
                if (!string.IsNullOrEmpty(exception.Source)) errorSource.Add(exception.Source);

                return ActionHandlerOutcome.Failed(new StandardActionFailure
                {
                    Code = exception.StatusCode?.ToString() ?? "500",
                    Errors =
                    [
                        new Xchange.Connector.SDK.Action.Error
                        {
                            Source = errorSource.ToArray(),
                            Text = exception.Message
                        }
                    ]
                });
            }
        }
    }
}
