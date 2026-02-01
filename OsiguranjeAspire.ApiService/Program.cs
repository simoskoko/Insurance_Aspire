using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
// keep ServiceDefaults if referenced; if not, remove the two lines that mention it
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using OsiguranjeAspire.ApiService.Data;
using OsiguranjeAspire.ApiService.Models;
using OsiguranjeAspire.Contracts.Polise;
using OsiguranjeAspire.Web;
using OsiguranjeAspire.Web.Auth;
using OsiguranjeAspire.Web.Components;
using OsiguranjeAspire.Web.Services;
using System.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped(sp =>
{
    var http = new HttpClient { BaseAddress = new Uri("http://apiservice") };
    return http;
});


builder.AddServiceDefaults();          // registers health checks, OTEL, discovery, etc.

builder.Services.AddDbContext<OsiguranjeContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient("api", client =>
{
    client.BaseAddress = new Uri("http://apiservice");
});//.AddHttpMessageHandler<TokenHttpClientHandler>();

builder.Services.AddProblemDetails(); // no options for older 
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(); // or AddInteractiveWebAssemblyComponents() if that’s your setup

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage(); // shows full stack traces in dev
else
    app.UseExceptionHandler();   // generic problem details in prod

app.MapDefaultEndpoints();
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

var summaries = new[]
{
    "Ledeno", "Hladno", "Hladnjikavo", "Prijatno", "Toplo", "Vrelo"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-15, 50),
            summaries[Random.Shared.Next(summaries.Length)])).ToArray();
    return forecast;
});

app.MapGet("/api/polise", async (OsiguranjeContext db) =>
    await db.Polise
        .Select(p => new PolisaDTO
        {
            BrPolise = p.BrPolise,
            ImeNosilac = p.ImeNosilac,
            JMBGNosilac = p.JMBGNosilac,
            TipNosilac = p.TipNosilac,
            VrstaId = p.VrstaId,
            LOBId = p.LOBId,
            Premija = p.Premija,
            VrstaPlacanjaId = p.VrstaPlacanjaId,
            DatumPocetka = p.DatumPocetka,
            DatumIsteka = p.DatumIsteka
            // map fields explicitly
        })
        .ToListAsync());


app.Run();
