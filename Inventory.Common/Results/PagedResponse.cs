namespace Inventory.Common.Results;

public class PagedResponse<T>
    where T : class
{
    public List<T>? Results { get; set; }

    public string? Next { get; set; }

    public string? Prev { get; set; }

    public int? Total { get; set; }
}