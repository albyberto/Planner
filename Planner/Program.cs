using Planner;
using Planner.Components;
using Planner.Infrastructure;
using Planner.Infrastructure.Core;

var builder = WebApplication.CreateBuilder(args);

AsWindowsService(builder);

builder.Services
    .AddMudBlazor()
    .AddPlannerOptions()
    .AddStores()
    .AddPlannerServices()
    .AddBackgroundServices()
    .AddClientsCore()
    .AddInfrastructureCore();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
return;

void AsWindowsService(WebApplicationBuilder webApplicationBuilder)
{
    webApplicationBuilder.Host.UseWindowsService();

    var port = webApplicationBuilder.Configuration.GetValue<string>("Port");
    port = string.IsNullOrEmpty(port) ? "2306" : port;

    webApplicationBuilder.WebHost.UseUrls($"http://*:{port}");
}