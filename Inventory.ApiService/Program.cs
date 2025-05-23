using AppHost.ServiceDefaults;
using Inventory.Common.Context;
using Inventory.Common.Entities;
using Inventory.Common.Results;
using Inventory.Common.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.AddSqlServerDbContext<InventoryContext>("Inventory");

builder.Services.AddScoped<IDataImporter, DataImporter>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet(
    "/inventory",
    async
    (
        [FromServices] InventoryContext context,
        [FromQuery] int pageSize = 10,
        [FromQuery] int page = 1) =>
    {
        if (!context.Items.TryGetNonEnumeratedCount(out var count))
        {
            count = await context.Items.CountAsync();
        }

        var pagesToSkip = page - 1;

        var items = await context.Items
            .Skip(pagesToSkip * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var next = !items.Any() || pagesToSkip * pageSize <= count ? $"/inventory?pageSize={pageSize}&page={page + 1}" : null;

        return new PagedResponse<Item> { Next = next, Total = count, Results = items };
    });

app.MapDefaultEndpoints();

app.Run();