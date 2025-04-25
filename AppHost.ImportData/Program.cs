// See https://aka.ms/new-console-template for more information

using AppHost.ImportData;
using CommandLine;
using Inventory.Common.Context;
using Inventory.Common.Entities;
using Inventory.Common.Results;
using Inventory.Common.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

# region Get Args
var parsedArgs = Parser.Default.ParseArguments<ImportDataOptions>(args);
var forceEmpty = parsedArgs?.Value.ForceEmpty ?? false;
# endregion

# region Startup
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddScoped<IDataImporter, DataImporter>();
builder.AddSqlServerDbContext<InventoryContext>("Inventory");
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
});

var app = builder.Build();
app.RunAsync();
# endregion

# region Console Run
// Using a using statement instead of a block since this all one scope
using var scope = app.Services.CreateScope();
var logger = app.Services.GetRequiredService<ILogger<Program>>();
var importer = scope.ServiceProvider.GetRequiredService<IDataImporter>();

var totalEvents = 0;
var successfulEventCount = 0;
var failedEventCount = 0;


List<Result<Item>> endResults = [];
await foreach(var itemResult in importer.GrabbyGrabby(logger, forceEmpty))
{
    totalEvents++;
    endResults.Add(
        itemResult.Match<Result<Item>>(
            onSuccess: item =>
            {
                successfulEventCount++;
                return item;
            },
            onFailure: errors =>
            {
                failedEventCount++;
                return errors;
            }));
}

logger.LogInformation(
    """
    Run method completed with:
    {TotalEventCount} total imports,
    {SuccessfulEventCount} successful imports, 
    {FailedEventCount} failed imports",
    """,
    totalEvents, successfulEventCount, failedEventCount);

var mergedResults = endResults.Merge();
// Merged results could be used to create ProblemDetails, or otherwise inspect the condition
#endregion

await app.StopAsync();

namespace AppHost.ImportData
{
    record ImportDataOptions
    {
        [Option('f', "forceEmpty", Required = false, HelpText = "Force empty database")]
        public bool ForceEmpty { get; set; }
    }
}