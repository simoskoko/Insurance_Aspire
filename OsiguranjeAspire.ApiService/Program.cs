using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
// keep ServiceDefaults if referenced; if not, remove the two lines that mention it
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using OsiguranjeAspire.ApiService.Data;
using OsiguranjeAspire.ApiService.Models;
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


//builder.Services.AddAuthorizationCore();
//builder.Services.AddScoped<JwtAuthStateProvider>();
//builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthStateProvider>());
//builder.Services.AddScoped<TokenHttpClientHandler>();
//builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("api"));
//builder.Services.AddScoped<AuthApi>();


builder.AddServiceDefaults();          // registers health checks, OTEL, discovery, etc.
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    var cs = builder.Configuration.GetConnectionString("DefaultConnection"); // or "Default"
    if (string.IsNullOrWhiteSpace(cs))
        throw new InvalidOperationException("Missing ConnectionStrings:DefaultConnection");
    opt.UseSqlServer(cs);
});

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
//app.MapGet("/api/pingdb", async (AppDbContext db) => await db.Database.CanConnectAsync());

//app.MapGet("/api/users", async (AppDbContext db) =>
//    await db.Users.AsNoTracking().ToListAsync());

//app.MapFallbackToPage("/_Host");
app.Run();
