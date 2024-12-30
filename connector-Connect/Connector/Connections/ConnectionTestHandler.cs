using Connector.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Xchange.Connector.SDK.Client.Testing;

namespace Connector.Connections
{
    public class ConnectionTestHandler(ILogger<IConnectionTestHandler> logger, ApiClient apiClient) : IConnectionTestHandler
    {
        private readonly ILogger<IConnectionTestHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        private readonly ApiClient _apiClient = apiClient ?? throw new ArgumentNullException(nameof(apiClient));

        public async Task<TestConnectionResult> TestConnection()
        {
            _logger.LogInformation("Starting connection test...");

            try
            {
                var response = await _apiClient.TestConnection();

                if (response == null)
                {
                    _logger.LogError("No response received from the API.");
                    return new TestConnectionResult
                    {
                        Success = false,
                        Message = "No response received from the server.",
                        StatusCode = 500
                    };
                }

                // Handle successful response
                if (response.IsSuccessful)
                {
                    _logger.LogInformation("Connection test successful: {StatusCode}", response.StatusCode);
                    return new TestConnectionResult
                    {
                        Success = true,
                        Message = "Connection test successful.",
                        StatusCode = response.StatusCode
                    };
                }

                // Handle specific HTTP status codes
                switch (response.StatusCode)
                {
                    case 400:
                        _logger.LogWarning("Bad request.");
                        return new TestConnectionResult
                        {
                            Success = false,
                            Message = "Bad request. Please check the configuration.",
                            StatusCode = 400
                        };
                    case 401:
                        _logger.LogWarning("Unauthorized.");
                        return new TestConnectionResult
                        {
                            Success = false,
                            Message = "Unauthorized: Invalid credentials.",
                            StatusCode = 401
                        };
                    case 403:
                        _logger.LogWarning("Forbidden.");
                        return new TestConnectionResult
                        {
                            Success = false,
                            Message = "Forbidden: Access denied.",
                            StatusCode = 403
                        };
                    case 404:
                        _logger.LogWarning("Not Found.");
                        return new TestConnectionResult
                        {
                            Success = false,
                            Message = "Resource not found. Check the API endpoint.",
                            StatusCode = 404
                        };
                    default:
                        _logger.LogWarning("Unexpected response: {StatusCode}", response.StatusCode);
                        return new TestConnectionResult
                        {
                            Success = false,
                            Message = "Unexpected error occurred.",
                            StatusCode = response.StatusCode
                        };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred during connection test.");
                return new TestConnectionResult
                {
                    Success = false,
                    Message = $"Exception occurred: {ex.Message}",
                    StatusCode = 500
                };
            }
        }
    }
}
