using System.Net.Http.Json;
using Inventory.Common.Entities;
using Inventory.Common.Enumerable;
using Inventory.Common.Http;
using Inventory.Common.Results;
using static Inventory.Common.Results.Result;

namespace Inventory.Common.Clients;

public class InventoryApiClient(HttpClient httpClient)
{
    public IAsyncEnumerable<Result<Item>> GetInventoryAsync(
        int page = 1,
        int pageSize = 100)
    {
        var initialRequest = HttpRequestBuilder.Create(HttpMethod.Get, new Uri($"/inventory?pageSize={pageSize}&page={page}", UriKind.Relative));
        return new PagedHttpRequestEnumerable<PagedResponse<Item>, Item>(
            () => Task.FromResult(Success(initialRequest)),
            response => response.Results ?? [],
            response => !string.IsNullOrEmpty(response.Next),
            response =>
            {
                page++;
                return initialRequest.WithUri(new Uri(response.Next!, UriKind.Relative));
            },
            async (message, token) =>
            {
                HttpResponseMessage response = await httpClient.SendAsync(message, token);
                return response.IsSuccessStatusCode
                    ? Success(
                        (await response.Content.ReadFromJsonAsync<PagedResponse<Item>>(token))!)
                    : Failure("Inventory.GetInventoryPage", $"Error fetching page {page}: {response.ReasonPhrase}",
                        ErrorType.Problem);
            });
    }
}