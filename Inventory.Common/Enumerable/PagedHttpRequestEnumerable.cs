using Inventory.Common.Http;
using Inventory.Common.Results;

namespace Inventory.Common.Enumerable;

/// <summary>
/// Provides an async enumerable implementation for handling paginated HTTP API responses.
/// </summary>
/// <typeparam name="TResponse">The type of response returned by the API for each page.</typeparam>
/// <typeparam name="TItem">The type of individual items extracted from each response.</typeparam>
public class PagedHttpRequestEnumerable<TResponse, TItem> : IAsyncEnumerable<Result<TItem>>
{
    private readonly Func<Task<Result<HttpRequestBuilder>>> _initialBuilderFactory;
    private readonly Func<TResponse, IEnumerable<TItem>> _itemSelector;
    private readonly Func<TResponse, bool> _hasMorePages;
    private readonly Func<TResponse, HttpRequestBuilder> _nextPageRequestBuilderFactory;
    private readonly Func<HttpRequestMessage, CancellationToken, Task<Result<TResponse>>> _sendRequestAsync;
    private readonly int _maxPages;

    /// <summary>
    /// Creates a paged HTTP request enumerable that automatically handles pagination for API responses.
    /// </summary>
    /// <param name="initialBuilderFactory">Factory function that creates the initial request builder for the first page.</param>
    /// <param name="itemSelector">Function that extracts the collection of items from each response.</param>
    /// <param name="hasMorePages">Predicate that determines if more pages need to be fetched based on the current response.</param>
    /// <param name="nextPageFactory">Factory function that creates a new request builder for the next page based on the current response.</param>
    /// <param name="sendRequestAsync">Function that sends the HTTP request and returns the parsed response.</param>
    /// <param name="maxPages">Maximum number of pages to fetch (default: 100).</param>
    ///     <remarks>
    /// This class handles automatic pagination for API endpoints that return collections across multiple pages.
    /// 
    /// CAUTION: Ensure the <paramref name="hasMorePages" /> delegate properly terminates to avoid infinite loops.
    /// The <paramref name="maxPages" /> parameter provides a safety limit to prevent runaway requests if <paramref name="hasMorePages" />
    /// is improperly implemented.
    /// 
    /// The <paramref name="itemSelector" /> keeps pagination logic separate from item processing, maintaining clean separation of concerns.
    /// </remarks>
    public PagedHttpRequestEnumerable(
        Func<Task<Result<HttpRequestBuilder>>> initialBuilderFactory,
        Func<TResponse, IEnumerable<TItem>> itemSelector,
        Func<TResponse, bool> hasMorePages,
        Func<TResponse, HttpRequestBuilder> nextPageFactory,
        Func<HttpRequestMessage, CancellationToken, Task<Result<TResponse>>> sendRequestAsync,
        int maxPages = 100)
    {
        _initialBuilderFactory = initialBuilderFactory;
        _itemSelector = itemSelector;
        _hasMorePages = hasMorePages;
        _nextPageRequestBuilderFactory = nextPageFactory;
        _sendRequestAsync = sendRequestAsync;
        _maxPages = maxPages;
    }

    /// <summary>
    /// Gets an async enumerator that iterates through all items across all pages of the API response.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the enumeration operation.</param>
    /// <returns>An async enumerator that yields results containing either items or errors.</returns>
    public async IAsyncEnumerator<Result<TItem>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        var currentBuilderResult = await _initialBuilderFactory();
        if (currentBuilderResult.IsFailure)
        {
            yield return currentBuilderResult.ToResult<TItem>();
            yield break;
        }
        var currentBuilder = currentBuilderResult.Value;
        var pageCount = 1;

        do
        {
            // Safety check to prevent infinite loops
            if (pageCount > _maxPages)
            {
                yield break;
            }
            
            // Send request for current page
            var result = await _sendRequestAsync(currentBuilder.Build(), cancellationToken);
            
            if (result.IsFailure)
            {
                yield return result.ToResult<TItem>();
                yield break;
            }

            var responseObject = result.Value;
            
            // Yield items from current page
            foreach (var item in _itemSelector(responseObject))
            {
                yield return item;
            }

            // Check if we need to fetch more pages
            if (!_hasMorePages(responseObject))
                yield break;
            
            pageCount++;
            
            // Create a new request builder for the next page
            currentBuilder = _nextPageRequestBuilderFactory(responseObject);
        } while (true);
    }
}
