var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddSqlServer("sqlServer")
    .WithDbGate()
    .WithLifetime(ContainerLifetime.Persistent);

var databaseName = "inventoryDb";

//lang=SQL
var script = $"""
             IF DB_ID('{databaseName}') IS NOT NULL
                CREATE DATABASE [{databaseName}]
             GO
                       
             USE [{databaseName}];
             GO
             
             IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = (N'[dbo].[Item]') AND type IN (N'U'))
             BEGIN
                 CREATE TABLE [dbo].[Item] ( 
                   [ItemNo] BIGINT NOT NULL,
                   [ItemDescription] NVARCHAR(MAX) NOT NULL,
                   [Quantity] INT NOT NULL,
                   [Price] INT NOT NULL,
                   CONSTRAINT [PK_Item] PRIMARY KEY ([ItemNo])
                 )
             END
             GO
             """;

var sqlDb = sqlServer
    .AddDatabase(databaseName)
    .WithCreationScript(script);

var importer = builder.AddProject<Projects.AppHost_ImportData>("importer")
    .WithReference(sqlDb)
    .WaitFor(sqlDb);

var apiService = builder.AddProject<Projects.Inventory_ApiService>("apiservice")
    .WithHttpHealthCheck("/health")
    .WithReference(sqlDb)
    .WaitForCompletion(importer)
    .WaitFor(sqlDb);

builder.AddProject<Projects.Inventory_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithHttpHealthCheck("/health")
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();