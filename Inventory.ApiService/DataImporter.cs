using Inventory.ApiService.Context;
using Inventory.ApiService.Entities;

namespace Inventory.ApiService;

public interface IDataImporter
{
    Task<List<Item>> GrabbyGrabby(ILogger logger, bool forceEmpty = false);
}

public class DataImporter(InventoryContext context) : IDataImporter
{
    public static async IAsyncEnumerable<Item> ImportDataFromFile()
    {
        await using var fileStream = File.Open("RandomInterviewItems.txt", new FileStreamOptions()
        {
            Access = FileAccess.Read,
            Share = FileShare.Read,
            Mode = FileMode.Open,
            Options = FileOptions.Asynchronous | FileOptions.SequentialScan
        });
        using var streamReader = new StreamReader(fileStream);
        if (streamReader.EndOfStream)
        {
            yield break;
        }
        var header = await streamReader.ReadLineAsync();
            
        // This should be a smarter check, but if we know the file is only in this format, this is fine
        // This line just assumes that, if the first line isn't the header, it's viable data
        // If it is just a header, we can skip it
        // TODO: Create a more general file-ingestion process when we get more inventory file formats
        if (header != "ItemNo|ItemDescription|Quantity|Price")
        {
            // I feel like there's a way to merge this first-time check with the loop elegantly
            // But I can't think of it right now
            if(header is not null)
            {
                var line = ProcessInventoryLine(header.AsSpan());
                if(line is not null)
                {
                    yield return line;
                }
            }
        }
        
        while(!streamReader.EndOfStream)
        {
            // Using Span here could be overkill
            // This would primarily be useful if file importing by line is high-frequency
            // Especially if the files get large
            
            var line = await streamReader.ReadLineAsync();
            if(line is null)
            {
                // As noted in ProcessInventoryLine, this should be a nicer version that returns a Result<T> containing an Error
                continue;
            }
            var item = ProcessInventoryLine(line.AsSpan());
            if(item is null)
            {
                continue;
            }
            yield return item;
        }
    }

    private static Item? ProcessInventoryLine(ReadOnlySpan<char> spanLine)
    {
        // Using Span here could be overkill
        // This would primarily be useful if file importing by line is high-frequency
        // Especially if the files get large
            
        var itemNoPos = spanLine.IndexOf('|');
        var itemDescEndPos = spanLine[(itemNoPos + 1)..].IndexOf('|') + itemNoPos + 1;
        (int Start, int Length) itemDescRange = (itemNoPos + 1, itemDescEndPos - (itemNoPos + 1));
        var quantityEndPos = spanLine[(itemDescEndPos + 1)..].IndexOf('|') + itemDescEndPos + 1;
        (int Start, int Length) quantityRange = (itemDescEndPos + 1, quantityEndPos - (itemDescEndPos + 1));
        var priceEndPos = spanLine[(quantityEndPos + 1)..].IndexOf('|') + quantityEndPos + 1;
        (int Start, int Length) priceRange = (quantityEndPos + 1, priceEndPos - (quantityEndPos + 1));
            
        // I am assuming that all fields are absolutely required for a valid item
        if(!long.TryParse(spanLine[..itemNoPos], out var itemNo))
        {
            // A nicer version would return a Result<T> containing an Error like ("InventoryImport", "InvalidItemNo", ErrorType.Problem)
            return null;
        }
            
        var itemDescription = spanLine.Slice(itemDescRange.Start, itemDescRange.Length);
            
        if(!int.TryParse(spanLine.Slice(quantityRange.Start, quantityRange.Length), out var quantity))
        {
            return null;
        }
            
        if (!decimal.TryParse(spanLine[priceRange.Start..], out var price))
        {
            return null;
        }

        var item = new Item()
        {
            ItemNo = itemNo,
            ItemDescription = itemDescription.ToString(),
            Quantity = quantity,
            Price = price
        };
        return item;
    }

    public async Task<List<Item>> GrabbyGrabby(ILogger logger, bool forceEmpty =  false)
    {
        if (forceEmpty)
        {
            context.Item.RemoveRange(context.Item);
            await context.SaveChangesAsync();
        }
        List<Item> endResults = [];
        await foreach (var item in ImportDataFromFile())
        {
            if (await context.Item.FindAsync(item.ItemNo) is not null)
            {
                continue;
            }
            context.Item.Add(item);
            endResults.Add(item);
        }
        await context.SaveChangesAsync();
        logger.LogInformation("Added {Count} items to the database", endResults.Count);
        return endResults;
    }
}