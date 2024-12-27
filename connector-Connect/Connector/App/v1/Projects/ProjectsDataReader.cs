using Connector.Client;
using System;
using ESR.Hosting.CacheWriter;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Xchange.Connector.SDK.CacheWriter;
using System.Net.Http;

namespace Connector.App.v1.Projects
{
    public class ProjectsDataReader : TypedAsyncDataReaderBase<ProjectsDataObject>
    {
        private readonly ILogger<ProjectsDataReader> _logger;
        private readonly ApiClient _apiClient;
        private int _currentPage = 0;

        public ProjectsDataReader(
            ILogger<ProjectsDataReader> logger,
            ApiClient apiClient)  // Inject ApiClient here
        {
            _logger = logger;
            _apiClient = apiClient;
        }

        public override async IAsyncEnumerable<ProjectsDataObject> GetTypedDataAsync(DataObjectCacheWriteArguments? dataObjectRunArguments, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            while (true)
            {
                var response = new ApiResponse<PaginatedResponse<ProjectsDataObject>>();

                // Try making a request to the API to retrieve data
                try
                {
                    response = await _apiClient.GetRecords<ProjectsDataObject>(
                        relativeUrl: "projects",
                        page: _currentPage,
                        cancellationToken: cancellationToken)
                        .ConfigureAwait(false);
                }
                catch (HttpRequestException exception)
                {
                    _logger.LogError(exception, "Exception while making a read request to data object 'ProjectsDataObject'");
                    throw;
                }

                if (!response.IsSuccessful)
                {
                    throw new Exception($"Failed to retrieve records for 'ProjectsDataObject'. API StatusCode: {response.StatusCode}");
                }

                if (response.Data == null || !response.Data.Items.Any()) break;

                // Yield each project data object to the caller
                foreach (var item in response.Data.Items)
                {
                    yield return item;
                }

                // Move to the next page for pagination
                _currentPage++;
                if (_currentPage >= response.Data.TotalPages)
                {
                    break;
                }
            }
        }
    }
}
