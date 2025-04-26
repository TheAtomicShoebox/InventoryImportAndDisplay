using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Inventory.Common.Http;

public class HttpRequestMessageConfigurator : HttpRequestMessage
{
    private OAuthFormFields? _formFields;

    private OAuthFormFields FormFields => _formFields ??= new OAuthFormFields();

    public string BearerToken { get; private set; }

    public HttpRequestMessageConfigurator(HttpMethod method, string requestUri) : base(method, requestUri) { }

    public HttpRequestMessageConfigurator(HttpMethod method, Uri requestUri) : base(method, requestUri) { }

    public HttpRequestMessageConfigurator AddOAuthFields(Action<OAuthFormFields> configureOAuthFields)
    {
        configureOAuthFields(FormFields);
        Content = new FormUrlEncodedContent(FormFields.AsFields());
        return this;
    }

    public HttpRequestMessageConfigurator AddHeaders(params (string Name, string Value)[] headers)
    {
        foreach (var (name, value) in headers)
        {
            Headers.Add(name, value);
        }

        return this;
    }

    public HttpRequestMessageConfigurator AddFormUrlEncodedContent(IEnumerable<KeyValuePair<string, string>> fields)
    {
        Content = new FormUrlEncodedContent(fields);
        return this;
    }

    public HttpRequestMessageConfigurator AddBasicAuthHeader(string username, string password)
    {
        var authToken = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{username}:{password}"));
        Headers.Authorization = new("Basic", authToken);
        return this;
    }

    public HttpRequestMessageConfigurator AddBearerToken(string token)
    {
        BearerToken = token;
        Headers.Authorization = new("Bearer", BearerToken);
        return this;
    }

    public HttpRequestMessageConfigurator AddProductInfoHeaderValue(ProductInfoHeaderValue authenticationHeaderValue)
    {
        Headers.UserAgent.Add(authenticationHeaderValue);
        return this;
    }

    public HttpRequestMessageConfigurator AddProductInfoHeaderValue(string productName, string productVersion)
    {
        return AddProductInfoHeaderValue(new(productName, productVersion));
    }

    public HttpRequestMessageConfigurator AddJsonContent<T>(T content, JsonSerializerOptions? options = null)
    {
        Content = new StringContent(JsonSerializer.Serialize(content, options), Encoding.UTF8, "application/json");
        return this;
    }

    public HttpRequestMessageConfigurator Configure(Func<HttpRequestMessageConfigurator, HttpRequestMessageConfigurator>? configure)
    {
        return configure?.Invoke(this) ?? this;
    }

    public HttpRequestMessageConfigurator SetBody(HttpContent content)
    {
        Content = content;
        return this;
    }
}
