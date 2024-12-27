using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic; // Added for KeyValuePair
using Xchange.Connector.SDK.Client.AuthTypes;
using Xchange.Connector.SDK.Client.ConnectionDefinitions.Attributes;

namespace Connector.Connections
{
    [ConnectionDefinition(title: "OAuth2CodeFlow", description: "Handles OAuth2 Authorization Code Flow for AppXchange connector.")]
    public class OAuth2CodeFlow : OAuth2CodeFlowBase
    {
        [ConnectionProperty(title: "Connection Environment", description: "The environment for the connection (Production or Test).", isRequired: true, isSensitive: false)]
        public ConnectionEnvironmentOAuth2CodeFlow ConnectionEnvironment { get; set; } = ConnectionEnvironmentOAuth2CodeFlow.Unknown;

        [ConnectionProperty(title: "Client ID", description: "The OAuth2 Client ID for authentication.", isRequired: true, isSensitive: false)]
        public new string ClientId { get; set; } = string.Empty; // Added 'new' and default value

        [ConnectionProperty(title: "Client Secret", description: "The OAuth2 Client Secret for authentication.", isRequired: true, isSensitive: true)]
        public new string ClientSecret { get; set; } = string.Empty; // Added 'new' and default value

        [ConnectionProperty(title: "Redirect URI", description: "The Redirect URI for the OAuth2 flow.", isRequired: true, isSensitive: false)]
        public string RedirectUri { get; set; } = string.Empty; // Added default value

        [ConnectionProperty(title: "Authorization Endpoint", description: "The endpoint to initiate the OAuth2 Authorization flow.", isRequired: true, isSensitive: false)]
        public string AuthorizationEndpoint => $"{BaseUrl}/oauth2/authorize";

        [ConnectionProperty(title: "Token Endpoint", description: "The endpoint to retrieve access tokens.", isRequired: true, isSensitive: false)]
        public string TokenEndpoint => $"{BaseUrl}/oauth2/token";

        public string BaseUrl
        {
            get
            {
                return ConnectionEnvironment switch
                {
                    ConnectionEnvironmentOAuth2CodeFlow.Production => "https://connect.trimble.com",
                    ConnectionEnvironmentOAuth2CodeFlow.Test => "https://web.stage.connect.trimble.com",
                    _ => throw new Exception("No base URL was set. Ensure the environment is configured properly."),
                };
            }
        }

        public async Task<string> ExchangeAuthorizationCodeForTokenAsync(string authorizationCode)
        {
            using var httpClient = new HttpClient();
            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("client_id", ClientId),
                new KeyValuePair<string, string>("client_secret", ClientSecret),
                new KeyValuePair<string, string>("redirect_uri", RedirectUri),
                new KeyValuePair<string, string>("code", authorizationCode)
            });

            var response = await httpClient.PostAsync(TokenEndpoint, requestBody);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return responseContent;
        }

        public string GetAuthorizationUrl(string state, string scope = "openid profile email")
        {
            var url = $"{AuthorizationEndpoint}?response_type=code&client_id={ClientId}&redirect_uri={RedirectUri}&scope={Uri.EscapeDataString(scope)}&state={Uri.EscapeDataString(state)}";
            return url;
        }
    }

    public enum ConnectionEnvironmentOAuth2CodeFlow
    {
        Unknown = 0,
        Production = 1,
        Test = 2
    }
}
