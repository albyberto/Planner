using System.Text;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Planner.Clients;
using Planner.Components;
using Planner.Models;

var builder = WebApplication.CreateBuilder(args);

// Windows Service
builder.Host.UseWindowsService();

// Forza la porta 2306
var port = builder.Configuration.GetValue<string>("Port");
port = string.IsNullOrEmpty(port) ? "2306" : port;

builder.WebHost.UseUrls($"http://*:{port}");

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor
builder.Services.AddMudServices();

// Configura Jira - API
builder.Services.AddOptions<JiraApiOptions>()
    .BindConfiguration(JiraApiOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Configura Jira - Query
builder.Services.AddOptions<JiraQueryOptions>()
    .BindConfiguration(JiraQueryOptions.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Configura Jira - Filtri
builder.Services.AddOptions<JiraFilterOptions>()
    .BindConfiguration(JiraFilterOptions.SectionName);

builder.Services.AddHttpClient<JiraClient>((provider, client) =>
{
    var settings = provider.GetRequiredService<IOptions<JiraApiOptions>>().Value;

    var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{settings.Email}:{settings.ApiToken}"));

    client.BaseAddress = new(settings.BaseUrl.TrimEnd('/') + "/");
    client.DefaultRequestHeaders.Authorization = new("Basic", credentials);
    client.DefaultRequestHeaders.Accept.Add(new("application/json"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();