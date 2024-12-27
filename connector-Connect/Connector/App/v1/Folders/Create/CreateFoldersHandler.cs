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
using System.Text.Json.Serialization; // For JsonPropertyNameAttribute
using System.ComponentModel; // For DescriptionAttribute

namespace Connector.App.v1.Folders.Create
{
    public class CreateFoldersHandler : IActionHandler<CreateFoldersAction>
    {
        private readonly ILogger<CreateFoldersHandler> _logger;
        private readonly ApiClient _apiClient; // Use ApiClient directly

        public CreateFoldersHandler(
            ILogger<CreateFoldersHandler> logger,
            ApiClient apiClient) // Inject the ApiClient
        {
            _logger = logger;
            _apiClient = apiClient; // Assign the ApiClient
        }

        public async Task<ActionHandlerOutcome> HandleQueuedActionAsync(ActionInstance actionInstance, CancellationToken cancellationToken)
        {
            var input = JsonSerializer.Deserialize<Connector.App.v1.Folders.Create.CreateFoldersActionInput>(actionInstance.InputJson);
            try
            {
                // Given the input for the action, make a call to your API/system
                var uploadId = input?.UploadId; // Use null conditional operator here to prevent null reference dereference
                var response = await _apiClient.PostFoldersDataObject(input, cancellationToken).ConfigureAwait(false);

                // If the response is already the output object for the action, use the response directly
                var result = response.Data;

                // Build sync operations to update the local cache as well as the Xchange cache system (if the data type is cached)
                var operations = new List<SyncOperation>();
                var keyResolver = new DefaultDataObjectKey();
                var key = keyResolver.BuildKeyResolver()(result);
                operations.Add(SyncOperation.CreateSyncOperation(UpdateOperation.Upsert.ToString(), key.UrlPart, key.PropertyNames, result));

                var resultList = new List<CacheSyncCollection>
                {
                    new CacheSyncCollection() { DataObjectType = typeof(FoldersDataObject), CacheChanges = operations.ToArray() }
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
                    Errors = new []
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

public class CreateFoldersActionInput
{
    [JsonPropertyName("uploadId")]
    [Description("The unique identifier for the folder upload.")]
    public string? UploadId { get; set; } // Nullable to match .NET conventions

    [JsonPropertyName("otherPropertyName")]
    [Description("Description for other property.")]
    public string? OtherProperty { get; set; } // Example of additional property
}
