using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
// keep ServiceDefaults if referenced; if not, remove the two lines that mention it
using Microsoft.Extensions.Hosting;
using Microsoft.JSInterop;
using OsiguranjeAspire.ApiService.Data;
using OsiguranjeAspire.ApiService.Models;
using OsiguranjeAspire.Contracts.Polise;
using OsiguranjeAspire.Contracts.Korisnici;
using OsiguranjeAspire.Web;
using OsiguranjeAspire.Web.Auth;
using OsiguranjeAspire.Web.Components;
using OsiguranjeAspire.Web.Components.Pages;
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

// Add authentication for cookies
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies");

var app = builder.Build();

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage(); // shows full stack traces in dev
else
    app.UseExceptionHandler();   // generic problem details in prod

app.UseAuthentication();
app.UseAuthorization();

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

app.MapGet("/api/polise/next-number", async (OsiguranjeContext db) =>
{
    var lastNumber = await db.Polise
        .Select(p => (int?)p.BrPolise)
        .MaxAsync();

    return lastNumber.GetValueOrDefault() + 1;
});

app.MapGet("/api/sifarnici/lob", async (OsiguranjeContext db) =>
    await db.SifarnikLOB
        .OrderBy(s => s.NazivLob)
        .Select(s => new SifarnikLobDTO
        {
            LobId = s.LobId,
            NazivLob = s.NazivLob
        })
        .ToListAsync());

app.MapGet("/api/sifarnici/vrste-placanja", async (OsiguranjeContext db) =>
    await db.SifarnikVrstaPlacanja
        .OrderBy(s => s.NazivVrstaPlacanja)
        .Select(s => new SifarnikVrstaPlacanjaDTO
        {
            VrstaPlacanjaId = s.VrstaPlacanjaId,
            NazivVrstaPlacanja = s.NazivVrstaPlacanja
        })
        .ToListAsync());

app.MapGet("/api/polise", async (OsiguranjeContext db) =>
    await db.Polise
        .Select(p => new PolisaDTO
        {
            BrPolise = p.BrPolise,
            ImeNosilac = p.ImeNosilac,
            JMBGNosilac = p.JMBGNosilac,
            TipNosilac = p.TipNosilac,
            LOBId = p.LOBId,
            Premija = p.Premija,
            VrstaPlacanjaId = p.VrstaPlacanjaId,
            DatumPocetka = p.DatumPocetka,
            DatumIsteka = p.DatumIsteka, 
            IdZaposlenog = p.IdZaposlenog
            // map fields explicitly
        })
        .ToListAsync());

async Task<Zaposleni?> GetCurrentEmployee(HttpContext context, OsiguranjeContext db)
{
    var username = context.User.FindFirst("username")?.Value
        ?? context.Request.Headers["X-Username"].FirstOrDefault();

    return string.IsNullOrWhiteSpace(username)
        ? null
        : await db.Zaposleni.FirstOrDefaultAsync(z => z.Username == username);
}

app.MapGet("/api/polise/{brPolise:int}", async (int brPolise, HttpContext context, OsiguranjeContext db) =>
{
    var employee = await GetCurrentEmployee(context, db);
    if (employee is null)
        return Results.Unauthorized();

    var polisa = await db.Polise
        .Where(p => p.BrPolise == brPolise)
        .Select(p => new PolisaDTO
        {
            BrPolise = p.BrPolise,
            ImeNosilac = p.ImeNosilac,
            JMBGNosilac = p.JMBGNosilac,
            TipNosilac = p.TipNosilac,
            LOBId = p.LOBId,
            Premija = p.Premija,
            VrstaPlacanjaId = p.VrstaPlacanjaId,
            DatumPocetka = p.DatumPocetka,
            DatumIsteka = p.DatumIsteka,
            IdZaposlenog = p.IdZaposlenog
        })
        .SingleOrDefaultAsync();

    return polisa is null ? Results.NotFound() : Results.Ok(polisa);
});

app.MapPost("/api/polise", async (PolisaDTO request, HttpContext context, OsiguranjeContext db) =>
{
    var employee = await GetCurrentEmployee(context, db);
    if (employee is null)
        return Results.Unauthorized();

    if (employee.RoleId is not (2 or 3))
        return Results.Forbid();

    if (string.IsNullOrWhiteSpace(request.JMBGNosilac) ||
        string.IsNullOrWhiteSpace(request.ImeNosilac) ||
        string.IsNullOrWhiteSpace(request.TipNosilac))
    {
        return Results.BadRequest("Podaci nosioca polise su obavezni.");
    }

    if (request.DatumIsteka < request.DatumPocetka)
        return Results.BadRequest("Datum isteka mora biti nakon datuma početka.");

    var polisa = new Polisa
    {
        JMBGNosilac = request.JMBGNosilac,
        ImeNosilac = request.ImeNosilac,
        TipNosilac = request.TipNosilac,
        LOBId = request.LOBId,
        Premija = request.Premija,
        VrstaPlacanjaId = request.VrstaPlacanjaId,
        DatumPocetka = request.DatumPocetka,
        DatumIsteka = request.DatumIsteka,
        IdZaposlenog = employee.Id
    };

    db.Polise.Add(polisa);
    await db.SaveChangesAsync();

    return Results.Ok(new PolisaDTO
    {
        BrPolise = polisa.BrPolise,
        JMBGNosilac = polisa.JMBGNosilac,
        ImeNosilac = polisa.ImeNosilac,
        TipNosilac = polisa.TipNosilac,
        LOBId = polisa.LOBId,
        Premija = polisa.Premija,
        VrstaPlacanjaId = polisa.VrstaPlacanjaId,
        DatumPocetka = polisa.DatumPocetka,
        DatumIsteka = polisa.DatumIsteka,
        IdZaposlenog = polisa.IdZaposlenog
    });
});

app.MapPut("/api/polise/{brPolise:int}", async (int brPolise, PolisaDTO request, HttpContext context, OsiguranjeContext db) =>
{
    var employee = await GetCurrentEmployee(context, db);
    if (employee is null)
        return Results.Unauthorized();

    if (request.BrPolise != brPolise)
        return Results.BadRequest("Broj polise ne odgovara adresi zahteva.");

    if (request.DatumIsteka < request.DatumPocetka)
        return Results.BadRequest("Datum isteka mora biti nakon datuma početka.");

    var polisa = await db.Polise.SingleOrDefaultAsync(p => p.BrPolise == brPolise);
    if (polisa is null)
        return Results.NotFound();

    var canEdit = employee.RoleId == 1 ||
        (employee.RoleId == 3 && employee.Id == polisa.IdZaposlenog);
    if (!canEdit)
        return Results.Forbid();

    polisa.JMBGNosilac = request.JMBGNosilac;
    polisa.ImeNosilac = request.ImeNosilac;
    polisa.TipNosilac = request.TipNosilac;
    polisa.LOBId = request.LOBId;
    polisa.Premija = request.Premija;
    polisa.VrstaPlacanjaId = request.VrstaPlacanjaId;
    polisa.DatumPocetka = request.DatumPocetka;
    polisa.DatumIsteka = request.DatumIsteka;
    polisa.IdZaposlenog = request.IdZaposlenog;

    await db.SaveChangesAsync();

    return Results.Ok(new PolisaDTO
    {
        BrPolise = polisa.BrPolise,
        JMBGNosilac = polisa.JMBGNosilac,
        ImeNosilac = polisa.ImeNosilac,
        TipNosilac = polisa.TipNosilac,
        LOBId = polisa.LOBId,
        Premija = polisa.Premija,
        VrstaPlacanjaId = polisa.VrstaPlacanjaId,
        DatumPocetka = polisa.DatumPocetka,
        DatumIsteka = polisa.DatumIsteka,
        IdZaposlenog = polisa.IdZaposlenog
    });
});

app.MapGet("/api/polise/zaposleni/{IdZaposlenog:int}",
    async (int IdZaposlenog, OsiguranjeContext db) =>
    {
        var polise = await db.Polise
        .Where(p => p.IdZaposlenog == IdZaposlenog).ToListAsync();

        return Results.Ok(polise);
    }
);

app.MapGet("/api/korisnici/ids", async (OsiguranjeContext db) =>
    await db.Zaposleni
        .Select(p => p.ImePrezime)
        .Distinct()
        .ToListAsync());

app.MapGet("/api/korisnici", async (OsiguranjeContext db) =>
{
    return await db.Zaposleni
        .Select(z => new KorisnikDTO
        {
            Id = z.Id,
            ImePrezime = z.ImePrezime
        })
        .ToListAsync();
});

app.MapGet("/api/korisnici/podredjeni/{nadredjeniId:int}", async (int nadredjeniId, OsiguranjeContext db) =>
{
    return await db.Zaposleni
        .Where(z => z.NadredjeniId == nadredjeniId)
        .Select(z => new KorisnikDTO //realno pametnije napraviti poseban DTO za ovu rutu da se ne salje i pw pri punjenju dropdowna, ali ajde
        {
            Id = z.Id,
            ImePrezime = z.ImePrezime
        })
        .ToListAsync();
});

app.MapPost("/api/auth/login", async (AuthLoginRequest request, HttpContext ctx, OsiguranjeContext db) =>
{
    if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
    {
        Console.WriteLine("Login failed: empty username or password");
        return Results.Unauthorized();
    }

    // Validate credentials against the Korisnici database table.
    var employee = await db.Zaposleni
        .AsNoTracking()
        .FirstOrDefaultAsync(z => z.Username == request.Username);

    if (employee is null)
    {
        Console.WriteLine($"Login failed: user '{request.Username}' not found in database");
        return Results.Unauthorized();
    }

    if (employee.Password != request.Password)
    {
        Console.WriteLine($"Login failed: password mismatch for user '{request.Username}'");
        return Results.Unauthorized();
    }

    Console.WriteLine($"Login successful for user '{request.Username}'");

    var claims = new List<System.Security.Claims.Claim>
    {
        new("username", request.Username),
        new("roleId", (employee?.RoleId ?? 0).ToString()),
        new("id", (employee?.Id ?? 0).ToString())
    };

    var identity = new System.Security.Claims.ClaimsIdentity(claims, "Cookies");
    var principal = new System.Security.Claims.ClaimsPrincipal(identity);

    await ctx.SignInAsync("Cookies", principal);

    return Results.Ok(new
    {
        username = request.Username,
        roleId = employee?.RoleId,
        id = employee?.Id
    });
});

app.UseAuthentication();
app.UseAuthorization();

app.Run();

record AuthLoginRequest(string Username, string Password);