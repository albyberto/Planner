using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MudBlazor.Services;
using Planner.Components;
using Planner.Models;
using Planner.Services;

var builder = WebApplication.CreateBuilder(args);

// Windows Service
builder.Host.UseWindowsService();

// Forza la porta 2306
builder.WebHost.UseUrls("http://*:2306");

// Add services to the container.
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

// MudBlazor
builder.Services.AddMudServices();

// Configura Jira - API
builder.Services.AddOptions<JiraApiSettings>()
    .BindConfiguration(JiraApiSettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Configura Jira - Query
builder.Services.AddOptions<JiraQuerySettings>()
    .BindConfiguration(JiraQuerySettings.SectionName)
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Configura Jira - Filtri
builder.Services.AddOptions<JiraFilterSettings>()
    .BindConfiguration(JiraFilterSettings.SectionName);

builder.Services.AddHttpClient<JiraClient>((provider, client) =>
{
    var settings = provider.GetRequiredService<IOptions<JiraApiSettings>>().Value;

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