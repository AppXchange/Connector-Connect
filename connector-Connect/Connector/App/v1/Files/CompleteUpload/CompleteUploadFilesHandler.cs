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
using System.Text.Json.Serialization;
using Xchange.Connector.SDK.Client.AppNetwork;

namespace Connector.App.v1.Files.CompleteUpload
{
    // Action handler for completing file upload
    public class CompleteUploadFilesHandler : IActionHandler<CompleteUploadFilesAction>
    {
        private readonly ILogger<CompleteUploadFilesHandler> _logger;
        private readonly ApiClient _apiClient;

        public CompleteUploadFilesHandler(
            ILogger<CompleteUploadFilesHandler> logger,
            ApiClient apiClient) // Injecting ApiClient directly
        {
            _logger = logger;
            _apiClient = apiClient; // Assigning the ApiClient
        }

        public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(ActionInstance actionInstance, CancellationToken cancellationToken)
        {
            var input = JsonSerializer.Deserialize<CompleteUploadFilesActionInput>(actionInstance.InputJson);
            if (input == null)
            {
                return ActionHandlerOutcome.Failed(new StandardActionFailure
                {
                    Code = "400",
                    Errors = new[]
                    {
                        new Xchange.Connector.SDK.Action.Error
                        {
                            Source = new[] { "CompleteUploadFilesHandler" },
                            Text = "Input deserialization failed or input is null."
                        }
                    }
                });
            }

            try
            {
                // Make a call to the API using ApiClient
                var response = await _apiClient.CompleteFileUploadAsync(input.UploadId, cancellationToken)
                    .ConfigureAwait(false);

                // Use the response directly as the action output
                var result = response.Data;

                // Build sync operations to update the local cache and the Xchange cache system
                var operations = new List<SyncOperation>();
                var keyResolver = new DefaultDataObjectKey();
                var key = keyResolver.BuildKeyResolver()(result);
                operations.Add(SyncOperation.CreateSyncOperation(UpdateOperation.Upsert.ToString(), key.UrlPart, key.PropertyNames, result));

                var resultList = new List<CacheSyncCollection>
                {
                    new CacheSyncCollection { DataObjectType = typeof(FilesDataObject), CacheChanges = operations.ToArray() }
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
                    Errors = new[]
                    {
                        new Xchange.Connector.SDK.Action.Error
                        {
                            Source = errorSource.ToArray(),
                            Text = exception.Message
                        }
                    }
                });
            }
        }
    }
}
