namespace Inventory.Common.Enumerable;

public class CachedAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    private readonly List<T> _cache = [];

    private readonly IAsyncEnumerator<T> _sourceEnumerator;

    private int _nextIndex;

    public CachedAsyncEnumerable(IAsyncEnumerable<T> source)
    {
        if (source == null) throw new ArgumentNullException(nameof(source));
        _sourceEnumerator = source.GetAsyncEnumerator();
    }

    private async ValueTask<(bool HasValue, T Item)> GetOrFetchItemAsync(int index, CancellationToken cancellationToken)
    {
        if (index < _cache.Count)
        {
            return (true, _cache[index]);
        }

        while (_cache.Count <= index)
        {
            if (await _sourceEnumerator.MoveNextAsync(cancellationToken))
            {
                _cache.Add(_sourceEnumerator.Current);
            }
            else
            {
                return (false, default!);
            }
        }
        return (true, _cache[index]);
    }

    public async ValueTask<List<T>> GetNextAsync(int count, CancellationToken cancellationToken = default)
    {
        if (_cache.Count >= _nextIndex + count)
        {
            var batch = _cache.GetRange(_nextIndex, count);
            _nextIndex += count;
            return batch;
        }

        List<T> batchList = new List<T>(count);
        while (_nextIndex < _cache.Count && batchList.Count < count)
        {
            batchList.Add(_cache[_nextIndex]);
            _nextIndex++;
        }

        while (batchList.Count < count)
        {
            var (hasValue, item) = await GetOrFetchItemAsync(_nextIndex, cancellationToken).ConfigureAwait(false);
            if (!hasValue)
            {
                break;
            }

            batchList.Add(item);
            _nextIndex++;
        }

        return batchList;
    }

    public async IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        int index = 0;
        while (true)
        {
            var (hasValue, item) = await GetOrFetchItemAsync(index, cancellationToken);
            if (!hasValue)
            {
                yield break;
            }

            yield return item;
            index++;
        }
    }

    public static CachedAsyncEnumerable<T> FromAsyncEnumerable(IAsyncEnumerable<T> source) => new(source);
}
