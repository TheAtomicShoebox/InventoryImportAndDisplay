# Inventory Management System

This solution demonstrates an Aspire-powered inventory display system with a Blazor web frontend and a REST API service backed by SQL Server.

## Prerequisites

- .NET 9 SDK
- Docker Desktop (must be running before launching the application)
- Visual Studio/Rider (optional, but it does make it simpler than using the CLI directly)

## Getting Started

### Running the Application

1. Clone this repository
2. Navigate to the solution directory
3. Run the application using:

`dotnet run --project AppHost.AppHost/AppHost.AppHost.csproj`

4. The Aspire dashboard will automatically open in your default browser

### First-Time Setup

**Note:** First-time startup may take several minutes as the SQL Server database container image is downloaded and initialized.

## Application Components

### Data Import

On startup, the system imports sample inventory items from the `RandomInterviewItems.txt` file before launching the API service and Blazor web application.

- Control the import behavior using command-line arguments:

`dotnet run --project AppHost.AppHost/AppHost.AppHost.csproj -- --force-import`

This forces a new data import even if data already exists in the database.

This can also be controlled via the `AppHost.AppHost/Program.cs` file:

```csharp
var importer = builder.AddProject<Projects.AppHost_ImportData>("importer")
    .WithArgs("--forceEmpty")
    .WithReference(sqlDb)
    .WaitFor(sqlDb);
```

### Accessing the Web Application

The Blazor web application may not automatically open in your browser. To access it:

1. Open the Aspire dashboard (automatically launched on startup)
2. Find the "webfrontend" service
3. Click the URL link to open the web application

## Database Inspection

- The Aspire host will automatically create a database named `Inventory` in the SQL Server instance.
- To inspect the SQL Server database, you can use any typical SQL Client tool
- Additionally, DBGate is a lightweight SQL client that can be used to connect to the SQL Server instance running in Docker.
- The Aspire host will create a DBGate gateway for you, which can be accessed via the dashboard
- The database will be persistent, and won't be recreated on every startup
- As long as Docker is running, the database will be available for inspection

## Solution Architecture

- **Inventory.ApiService**: REST API providing inventory data
- **Inventory.Web**: Blazor web frontend for displaying inventory
- **AppHost.ImportData**: Console utility for importing sample inventory data
- **AppHost.AppHost**: Aspire application host that orchestrates all services

## Development Notes

- The application uses .NET 9 with preview language features
- Entity Framework Core is used for data access
- The solution leverages Aspire for distributed application development
- Services communicate through HTTP APIs
    - Minimal APIs are used for simplicity

## Troubleshooting

- If you encounter database connection issues, ensure Docker is running
- Check the Aspire dashboard for service health and logs
- Verify the various containers are running via Docker
