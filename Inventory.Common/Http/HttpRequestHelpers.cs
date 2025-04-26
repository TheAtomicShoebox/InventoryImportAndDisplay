namespace Inventory.Common.Http;

public static class HttpRequestHelpers
{
    /// <summary>
    /// Creates an HTTP request with a Bearer token for authentication.
    /// </summary>
    /// <param name="method">The HTTP method to use (e.g., GET, POST).</param>
    /// <param name="uri">The target URI for the request.</param>
    /// <param name="accessToken">The Bearer token for authentication.</param>
    /// <param name="configure">Optional configurator for additional request customization.</param>
    /// <returns>A configured HttpRequestBuilder instance.</returns>
    public static HttpRequestBuilder CreateBearerTokenRequest(
        HttpMethod method,
        Uri uri,
        string accessToken,
        Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure = null)
        => HttpRequestBuilder.Create(method, uri)
            .WithBearerToken(accessToken)
            .ConfigureRequest((request) => configure is not null ? configure(request) : request);

    /// <summary>
    /// Creates an HTTP request with a Bearer token for authentication.
    /// </summary>
    /// <param name="method">The HTTP method to use (e.g., GET, POST).</param>
    /// <param name="endpoint">The target endpoint for the request.</param>
    /// <param name="accessToken">The Bearer token for authentication.</param>
    /// <param name="configure">Optional configurator for additional request customization.</param>
    /// <returns>A configured HttpRequestBuilder instance.</returns>
    public static HttpRequestBuilder CreateBearerTokenRequest(
        HttpMethod method,
        string endpoint,
        string accessToken,
        Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure = null)
        => HttpRequestBuilder.Create(method, endpoint)
            .WithBearerToken(accessToken)
            .ConfigureRequest((request) => configure is not null ? configure(request) : request);

    /// <summary>
    /// Creates an HTTP request with client credentials for authentication.
    /// </summary>
    /// <param name="method">The HTTP method to use (e.g., GET, POST).</param>
    /// <param name="uri">The target URI for the request.</param>
    /// <param name="clientId">The client ID for authentication.</param>
    /// <param name="clientSecret">The client secret for authentication.</param>
    /// <param name="configure">Optional configurator for additional request customization.</param>
    /// <returns>A configured HttpRequestBuilder instance.</returns>
    public static HttpRequestBuilder CreateClientCredentialsRequest(
        HttpMethod method,
        Uri uri,
        string clientId,
        string clientSecret,
        Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure = null)
        => HttpRequestBuilder.Create(method, uri)
            .WithClientCredentials(clientId, clientSecret)
            .ConfigureRequest((request) => configure is not null ? configure(request) : request);

    /// <summary>
    /// Creates an HTTP request with client credentials for authentication.
    /// </summary>
    /// <param name="method">The HTTP method to use (e.g., GET, POST).</param>
    /// <param name="endpoint">The target endpoint for the request.</param>
    /// <param name="clientId">The client ID for authentication.</param>
    /// <param name="clientSecret">The client secret for authentication.</param>
    /// <param name="configure">Optional configurator for additional request customization.</param>
    /// <returns>A configured HttpRequestBuilder instance.</returns>
    public static HttpRequestBuilder CreateClientCredentialsRequest(
        HttpMethod method,
        string endpoint,
        string clientId,
        string clientSecret,
        Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure = null)
        => HttpRequestBuilder.Create(method, endpoint)
            .WithClientCredentials(clientId, clientSecret)
            .ConfigureRequest((request) => configure is not null ? configure(request) : request);

    /// <summary>
    /// Creates an HTTP request with a refresh token for authentication.
    /// </summary>
    /// <param name="method">The HTTP method to use (e.g., GET, POST).</param>
    /// <param name="uri">The target URI for the request.</param>
    /// <param name="refreshToken">The refresh token for authentication.</param>
    /// <param name="configure">Optional configurator for additional request customization.</param>
    /// <returns>A configured HttpRequestBuilder instance.</returns>
    public static HttpRequestBuilder CreateRefreshTokenRequest(
        HttpMethod method,
        Uri uri,
        string refreshToken,
        Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure = null)
        => HttpRequestBuilder.Create(method, uri)
            .WithRefreshToken(refreshToken)
            .ConfigureRequest((request) => configure is not null ? configure(request) : request);

    /// <summary>
    /// Creates an HTTP request with a refresh token for authentication.
    /// </summary>
    /// <param name="method">The HTTP method to use (e.g., GET, POST).</param>
    /// <param name="endpoint">The target endpoint for the request.</param>
    /// <param name="refreshToken">The refresh token for authentication.</param>
    /// <param name="configure">Optional configurator for additional request customization.</param>
    /// <returns>A configured HttpRequestBuilder instance.</returns>
    public static HttpRequestBuilder CreateRefreshTokenRequest(
        HttpMethod method,
        string endpoint,
        string refreshToken,
        Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure = null)
        => HttpRequestBuilder.Create(method, endpoint)
            .WithRefreshToken(refreshToken)
            .ConfigureRequest((request) => configure is not null ? configure(request) : request);

    /// <summary>
    /// Creates an HTTP request with Basic Authentication.
    /// </summary>
    /// <param name="method">The HTTP method to use (e.g., GET, POST).</param>
    /// <param name="uri">The target URI for the request.</param>
    /// <param name="username">The username for Basic Authentication.</param>
    /// <param name="password">The password for Basic Authentication.</param>
    /// <param name="configure">Optional configurator for additional request customization.</param>
    /// <returns>A configured HttpRequestBuilder instance.</returns>
    public static HttpRequestBuilder CreateBasicAuthRequest(
        HttpMethod method,
        Uri uri,
        string username,
        string password,
        Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure = null)
        => HttpRequestBuilder.Create(method, uri)
            .WithBasicAuth(username, password)
            .ConfigureRequest((request) => configure is not null ? configure(request) : request);

    /// <summary>
    /// Creates an HTTP request with Basic Authentication.
    /// </summary>
    /// <param name="method">The HTTP method to use (e.g., GET, POST).</param>
    /// <param name="endpoint">The target endpoint for the request.</param>
    /// <param name="username">The username for Basic Authentication.</param>
    /// <param name="password">The password for Basic Authentication.</param>
    /// <param name="configure">Optional configurator for additional request customization.</param>
    /// <returns>A configured HttpRequestBuilder instance.</returns>
    public static HttpRequestBuilder CreateBasicAuthRequest(
        HttpMethod method,
        string endpoint,
        string username,
        string password,
        Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure = null)
        => HttpRequestBuilder.Create(method, endpoint)
            .WithBasicAuth(username, password)
            .ConfigureRequest((request) => configure is not null ? configure(request) : request);
}