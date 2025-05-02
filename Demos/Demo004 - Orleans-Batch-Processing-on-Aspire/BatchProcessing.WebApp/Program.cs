using BatchProcessing.Abstractions.Grains;
using BatchProcessing.WebApp.Components;
using BatchProcessing.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FluentUI.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.AddKeyedRedisClient("redis");
builder.UseOrleansClient();
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddFluentUIComponents();
builder.Services.AddProblemDetails();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BatchProcessing.WebApp.Client._Imports).Assembly);

app.MapPost("/batchProcessing", async (IClusterClient client, [FromBody] int records, [FromQuery] string region = "NA") =>
{
    RequestContext.Set("cloud.region", region);
    var grain = client.GetGrain<IEngineGrain>(Guid.NewGuid());
    await grain.RunAnalysis(records);
    return grain.GetPrimaryKey();
});

app.MapGet("/batchProcessing/{id}/status", async (IClusterClient client, Guid id) =>
{
    var grain = client.GetGrain<IEngineGrain>(id);
    var status = await grain.GetStatus();
    return status;
});

app.MapGet("/batchProcessing", async (IClusterClient client) =>
{
    var grain = client.GetGrain<IBatchProcessManagerGrain>(0);
    var processes = await grain.GetBatchProcesses();
    return processes;
});

app.MapGet("/batchProcessing/{id}", async (IClusterClient client, Guid id) =>
{
    var grain = client.GetGrain<IBatchProcessManagerGrain>(0);
    var process = await grain.GetBatchProcess(id);
    return process;
});

app.MapDefaultEndpoints();
app.Run();
