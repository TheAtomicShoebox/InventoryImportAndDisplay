namespace Inventory.Common.Http;

public sealed record HttpRequestBuilder
{
    private readonly HttpRequestMessageConfigurator _baseRequest;
    private Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? _configureRequest;

    private HttpRequestBuilder(HttpMethod method, Uri uri)
    {
        _baseRequest = new(method, uri);
    }

    public Uri Uri
    {
        get => _baseRequest.RequestUri ?? throw new InvalidOperationException("Request URI must not be null.");
        set => _baseRequest.RequestUri = value;
    }

    public HttpRequestMessageConfigurator Build()
    {
        return _baseRequest.Configure(_configureRequest);
    }

    public HttpRequestBuilder ConfigureRequest(Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator> configure)
    {
        return this with
        {
            _configureRequest = _configureRequest is not null
                ? request => configure(_configureRequest(request))
                : configure
        };
    }

    #region Create Methods
    public static HttpRequestBuilder Create(HttpMethod method, string endpoint) => new(method, new(endpoint));

    public static HttpRequestBuilder Create(HttpMethod method, Uri uri) => new(method, uri);
    #endregion

    #region Authorization Configuration
    public HttpRequestBuilder WithBearerToken(string accessToken)
        => ConfigureRequest(request => request.AddBearerToken(accessToken));

    public HttpRequestBuilder WithBasicAuth(string username, string password)
        => ConfigureRequest(request => request.AddBasicAuthHeader(username, password));
    public HttpRequestBuilder WithClientCredentials()
        => ConfigureRequest(request => request.AddOAuthFields(fields => { fields.GrantType = "client_credentials"; }));

    public HttpRequestBuilder WithClientCredentials(string clientId, string clientSecret)
        => ConfigureRequest(request => request.AddOAuthFields(fields =>
        {
            fields.GrantType = "client_credentials";
            fields.ClientId = clientId;
            fields.ClientSecret = clientSecret;
        }));

    public HttpRequestBuilder AddHeaders(params (string Name, string Value)[] headers)
      => ConfigureRequest(request => request.AddHeaders(headers));

    public HttpRequestBuilder AddFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> fields)
      => ConfigureRequest(request => request.AddFormUrlEncodedContent(fields));

    public HttpRequestBuilder WithUri(Uri uri)
        => this with { Uri = uri };

    public HttpRequestBuilder WithUri(string endpoint)
        => this with { Uri = new(endpoint) };


    public HttpRequestBuilder WithRefreshToken(string refreshToken)
        => ConfigureRequest(
            request => request.AddOAuthFields(fields =>
            {
                fields.GrantType = "refresh_token";
                fields.RefreshToken = refreshToken;
            }));
    #endregion
}