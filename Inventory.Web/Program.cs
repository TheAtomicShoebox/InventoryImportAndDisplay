using AppHost.ServiceDefaults;
using Inventory.Common.Clients;
using Inventory.Web;
using Inventory.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire client integrations.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services
       .AddRazorComponents()
       .AddInteractiveServerComponents()
       .AddCircuitOptions(opt => opt.DetailedErrors = true);

builder.Services.AddOutputCache();

builder.Services.AddHttpClient<InventoryApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.UseOutputCache();

app.MapStaticAssets();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();