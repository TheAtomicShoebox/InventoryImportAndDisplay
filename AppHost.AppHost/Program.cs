var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder
    .AddSqlServer("sqlServer")
    .WithDbGate()
    .WithLifetime(ContainerLifetime.Persistent);

var databaseName = "Inventory";

//lang=SQL
var script = $"""
             IF DB_ID('{databaseName}') IS NULL
                CREATE DATABASE [{databaseName}]
             GO
                       
             USE [{databaseName}];
             GO
             
             IF OBJECT_ID(N'dbo.Item', N'U') IS NULL
             BEGIN
                 CREATE TABLE [dbo].[Item] ( 
                   [ItemNo] BIGINT NOT NULL,
                   [ItemDescription] NVARCHAR(MAX) NOT NULL,
                   [Quantity] INT NOT NULL,
                   [Price] DECIMAL NOT NULL,
                   CONSTRAINT [PK_Item] PRIMARY KEY ([ItemNo])
                 )
             END
             GO
             """;

var sqlDb = sqlServer
    .AddDatabase(databaseName)
    .WithCreationScript(script);

var importer = builder.AddProject<Projects.AppHost_ImportData>("importer")
    //.WithArgs("--forceEmpty")
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