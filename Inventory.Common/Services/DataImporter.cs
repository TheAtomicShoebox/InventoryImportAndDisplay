using System.Diagnostics;
using Inventory.Common.Context;
using Inventory.Common.Entities;
using Inventory.Common.Results;
using Microsoft.Extensions.Logging;
using static Inventory.Common.Results.Result;

namespace Inventory.Common.Services;

public interface IDataImporter
{
    IAsyncEnumerable<Result<Item>> GrabbyGrabby(ILogger logger, bool forceEmpty = false);
}

public static class DataImporterErrors
{
   public static readonly Error FileStreamEndOnStart =  new("DataImporter.ImportDataFromFile.EmptyStream", "File stream failed to start", ErrorType.Problem);
   
   public static readonly Error CouldNotGetFileLine =  new("DataImporter.ProcessInventoryLine.NullLine", "File line was null", ErrorType.Problem);
   public static readonly Error InvalidItemNumber = new("DataImporter.ProcessInventoryLine.InvalidItemNumber", "ItemNo was invalid", ErrorType.Problem);
   public static readonly Error InvalidItemDescription = new("DataImporter.ProcessInventoryLine.InvalidItemDescription", "Description was null or empty", ErrorType.Problem);
   public static readonly Error InvalidItemQuantity = new("DataImporter.ProcessInventoryLine.InvalidItemQuantity", "Quantity was invalid", ErrorType.Problem);
   public static readonly Error InvalidItemPrice = new("DataImporter.ProcessInventoryLine.InvalidItemPrice", "Price was invalid", ErrorType.Problem);
   public static readonly Error ItemAlreadyExists = new("DataImporter.GrabbyGrabby.ItemAlreadyExists", "Item already exists in database", ErrorType.None);
}

public class DataImporter(InventoryContext context) : IDataImporter
{
    public static async IAsyncEnumerable<Result<Item>> ImportDataFromFile()
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
            yield return DataImporterErrors.FileStreamEndOnStart;
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
                yield return ProcessInventoryLine(header.AsSpan());
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
                yield return DataImporterErrors.CouldNotGetFileLine;
            }
            yield return ProcessInventoryLine(line.AsSpan());
        }
    }

    private static Result<Item> ProcessInventoryLine(ReadOnlySpan<char> spanLine)
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
            return DataImporterErrors.CouldNotGetFileLine;
        }
            
        var itemDescription = spanLine.Slice(itemDescRange.Start, itemDescRange.Length).ToString();
        if (string.IsNullOrEmpty(itemDescription))
        {
            return DataImporterErrors.InvalidItemNumber;
        }
            
        if(!int.TryParse(spanLine.Slice(quantityRange.Start, quantityRange.Length), out var quantity))
        {
            return DataImporterErrors.InvalidItemQuantity;
        }
            
        if (!decimal.TryParse(spanLine[priceRange.Start..], out var price))
        {
            return DataImporterErrors.InvalidItemPrice;
        }

        return new Item()
        {
            ItemNo = itemNo,
            ItemDescription = itemDescription,
            Quantity = quantity,
            Price = price
        };
    }

    public async IAsyncEnumerable<Result<Item>> GrabbyGrabby(ILogger logger, bool forceEmpty = false)
    {
        if (forceEmpty)
        {
            context.Items.RemoveRange(context.Items);
            await context.SaveChangesAsync();
        }
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var items = context.Items.AsQueryable();
        var resultImportedEnumerable = ImportDataFromFile().Where(itemResult => itemResult.Match(onSuccess: item => !items.Contains(item), onFailure: _ => false));
        await foreach (var itemResult in resultImportedEnumerable)
        {
            yield return await itemResult
                .Bind(async ValueTask<Result<(Item original, Item? found)>>(item) => (item, await context.Items.FindAsync(item.ItemNo)))
                .Bind(t => t.found is null
                          ? Failure<Item>(DataImporterErrors.ItemAlreadyExists)
                          : Success(t.original))
                .Tap(item => context.Add(item));
        }
        var count = await context.SaveChangesAsync();
        stopwatch.Stop();
        logger.LogInformation("Added {Count} items to the database in {Time}", count, stopwatch.Elapsed);
    }
}