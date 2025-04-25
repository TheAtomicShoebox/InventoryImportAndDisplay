using System.Collections;
using System.Text.Json.Serialization;

namespace Inventory.Common.Results;

public readonly record struct Errors(List<Error>? ErrorList) : IList<Error>
{
    [JsonConstructor]
    public Errors() : this([]) { }

    private List<Error> ErrorList { get; init; } = ErrorList ?? [];

    public IEnumerator<Error> GetEnumerator() => ErrorList.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static bool IsNone(IEnumerable<Error> errors) => !errors.Any();

    private bool IsNone() => IsNone(this);

    public int Count => ErrorList.Count;
    bool ICollection<Error>.IsReadOnly => false;

    public Error this[int index]
    {
        get => ErrorList[index];
        set => ErrorList[index] = value;
    }

    public void Add(Error error)
    {
        if (error.IsNone())
        {
            return;
        }

        ErrorList.Add(error);
    }

    public void AddRange(IEnumerable<Error> errors) =>
        ErrorList.AddRange(errors);

    public void Clear() => ErrorList.Clear();

    public bool Contains(Error item) =>
        ErrorList.Contains(item);

    public void CopyTo(Error[] array, int arrayIndex) =>
        ErrorList.CopyTo(array, arrayIndex);

    public int IndexOf(Error error) =>
        ErrorList.IndexOf(error);

    public void Insert(int index, Error error) =>
        ErrorList.Insert(index, error);

    public bool Remove(Error error) =>
        ErrorList.Remove(error);

    public void RemoveAt(int index) =>
        ErrorList.RemoveAt(index);

    public static bool operator ==(Errors errors, Error error) => errors.FirstOrDefault() == error;

    public static bool operator !=(Errors errors, Error error) => !(errors == error);

    public static bool operator ==(Error error, Errors errors) => errors == error;

    public static bool operator !=(Error error, Errors errors) => errors != error;
}