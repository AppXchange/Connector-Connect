using Connector.Client;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using Xchange.Connector.SDK.CacheWriter;

namespace Connector.App.v1.Files
{
    /// <summary>
    /// Reads file data objects from an API and streams them for caching.
    /// </summary>
    public class FilesDataReader(
        ILogger<FilesDataReader> logger,
        ApiClient apiClient) : TypedAsyncDataReaderBase<FilesDataObject>
    {
        private readonly ILogger<FilesDataReader> _logger = logger;
        private readonly ApiClient _apiClient = apiClient; // Use ApiClient directly.
        private int _currentPage = 0;


        /// <summary>
        /// Retrieves and streams file data objects asynchronously.
        /// </summary>
        /// <param name="dataObjectRunArguments">Optional arguments for the data retrieval operation.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>Async enumerable of <see cref="FilesDataObject"/>.</returns>
        public override async IAsyncEnumerable<FilesDataObject> GetTypedDataAsync(
            DataObjectCacheWriteArguments? dataObjectRunArguments,
            [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (true)
            {
                ApiResponse<PaginatedResponse<FilesDataObject>> response;

                try
                {
                    // Fetch a paginated list of files from the API
                    response = await _apiClient.GetRecords<FilesDataObject>(
                        relativeUrl: "files",
                        page: _currentPage,
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException exception)
                {
                    _logger.LogError(exception, "Exception occurred while making a read request to 'FilesDataObject'");
                    throw;
                }

                if (!response.IsSuccessful)
                {
                    throw new Exception($"Failed to retrieve records for 'FilesDataObject'. API StatusCode: {response.StatusCode}");
                }

                if (response.Data == null || !response.Data.Items.Any())
                {
                    _logger.LogInformation("No more data to retrieve for 'FilesDataObject'.");
                    break;
                }

                foreach (var item in response.Data.Items)
                {
                    // Additional transformations or validations can be applied here if needed.
                    yield return item;
                }

                _currentPage++;

                if (_currentPage >= response.Data.TotalPages)
                {
                    _logger.LogInformation("Reached the last page for 'FilesDataObject'.");
                    break;
                }
            }
        }
    }
}
