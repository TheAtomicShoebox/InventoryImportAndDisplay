using System.Collections;

namespace Inventory.Common.Http;

public sealed class OAuthFieldValueCollection : IEnumerable<string>
{
    private readonly List<string> _values = new();

    public void Add(string value) => _values.Add(value);

    public bool Remove(string value) => _values.Remove(value);

    public void Clear() => _values.Clear();

    public string this[int index]
    {
        get => _values[index];
        set => _values[index] = value;
    }

    public IEnumerator<string> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public override string ToString() => string.Join(" ", _values);
}
