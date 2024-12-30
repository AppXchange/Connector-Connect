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

namespace Connector.App.v1.Folders.Create
{
    public class CreateFoldersHandler(
        ILogger<CreateFoldersHandler> logger,
        ApiClient apiClient) : IActionHandler<CreateFoldersAction>
    {
        private readonly ILogger<CreateFoldersHandler> _logger = logger;
        private readonly ApiClient _apiClient = apiClient; // Use ApiClient directly


        public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(ActionInstance actionInstance, CancellationToken cancellationToken)
        {
            var input = JsonSerializer.Deserialize<CreateFoldersActionInput>(actionInstance.InputJson);
            try
            {
                // Given the input for the action, make a call to your API/system
                var uploadId = input?.UploadId; // Use null conditional operator here to prevent null reference dereference
                if (input == null)
                {
                    throw new System.ArgumentNullException(nameof(input), "Input for CreateFoldersAction cannot be null.");
                }

                var response = await _apiClient.PostFoldersDataObject(input, cancellationToken).ConfigureAwait(false);

                // If the response is already the output object for the action, use the response directly
                var result = response.Data;

                // Build sync operations to update the local cache as well as the Xchange cache system (if the data type is cached)
                var operations = new List<SyncOperation>();
                var keyResolver = new DefaultDataObjectKey();
                if (result == null)
                {
                    throw new System.InvalidOperationException("API response did not contain a valid result.");
                }

                var (UrlPart, PropertyNames) = keyResolver.BuildKeyResolver()(result);
                operations.Add(SyncOperation.CreateSyncOperation(UpdateOperation.Upsert.ToString(), UrlPart, PropertyNames, result));

                var resultList = new List<CacheSyncCollection>
                {
                    new() { DataObjectType = typeof(FoldersDataObject), CacheChanges = operations.ToArray() }
                };

                return ActionHandlerOutcome.Successful(result, resultList);
            }
            catch (HttpRequestException exception)
            {
                // If an error occurs, we want to create a failure result for the action that matches
                // the failure type for the action. 
                // Common to create extension methods to map to Standard Action Failure

                var errorSource = new List<string> { "CreateFoldersHandler" };
                if (string.IsNullOrEmpty(exception.Source)) errorSource.Add(exception.Source!);

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