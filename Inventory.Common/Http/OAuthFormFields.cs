using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Inventory.Common.Http;

public class OAuthFormFields : IEnumerable<KeyValuePair<string, OAuthFieldValueCollection>>
{
    private readonly Dictionary<string, OAuthFieldValueCollection> _fields = new();

    public string? GrantType
    {
        get => GetSingleValue("grant_type");
        set => SetSingleValue("grant_type", value);
    }

    public string? ClientId
    {
        get => GetSingleValue("client_id");
        set => SetSingleValue("client_id", value);
    }

    public string? ClientSecret
    {
        get => GetSingleValue("client_secret");
        set => SetSingleValue("client_secret", value);
    }

    public string? RefreshToken
    {
        get => GetSingleValue("refresh_token");
        set => SetSingleValue("refresh_token", value);
    }

    public string? RedirectUri
    {
        get => GetSingleValue("redirect_uri");
        set => SetSingleValue("redirect_uri", value);
    }

    public string? ResponseType
    {
        get => GetSingleValue("response_type");
        set => SetSingleValue("response_type", value);
    }

    public string? Username
    {
        get => GetSingleValue("username");
        set => SetSingleValue("username", value);
    }

    public string? Password
    {
        get => GetSingleValue("password");
        set => SetSingleValue("password", value);
    }

    public OAuthFieldValueCollection Scope => GetMultipleValues("scope");

    #region Helper Values
    private string? GetSingleValue(string key) =>
        _fields.TryGetValue(key, out var values) && values.Any() ? values[0] : null;

    private bool TryGetSingleValue(string key, [MaybeNullWhen(false)] out string value)
    {
        if (_fields.TryGetValue(key, out var values) && values.Any())
        {
            value = values[0];
            return true;
        }
        value = null;
        return false;
    }

    private void SetSingleValue(string key, string? value)
    {
        if (!_fields.ContainsKey(key))
        {
            _fields[key] = new OAuthFieldValueCollection();
        }
        _fields[key].Clear();
        if (value is not null)
        {
            _fields[key].Add(value);
        }
    }

    private OAuthFieldValueCollection GetMultipleValues(string key)
    {
        if (!_fields.ContainsKey(key))
        {
            _fields[key] = new OAuthFieldValueCollection();
        }

        return _fields[key];
    }

    private bool TryGetMultipleValues(string key, [MaybeNullWhen(false)] out OAuthFieldValueCollection values) =>
        _fields.TryGetValue(key, out values);
    #endregion

    public OAuthFieldValueCollection this[string key]
    {
        get
        {
            if (_fields.TryGetValue(key, out var value))
                return value;

            value = new OAuthFieldValueCollection();
            _fields[key] = value;
            return value;
        }
        set => _fields[key] = value;
    }

    public IEnumerator<KeyValuePair<string, OAuthFieldValueCollection>> GetEnumerator() => _fields.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() =>
        string.Join("&", _fields.SelectMany(kvp => kvp.Value.Select(value => $"{kvp.Key}={value}")));

    public IEnumerable<KeyValuePair<string, string>> AsFields()
    {
        return _fields.Select(kvp => new KeyValuePair<string, string>(kvp.Key, kvp.Value.ToString()));
        //return _fields.SelectMany(kvp => kvp.Value.Select(value => new KeyValuePair<string, string>(kvp.Key, value)));
    }
}