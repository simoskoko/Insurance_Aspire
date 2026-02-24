using OsiguranjeAspire.Web;
using OsiguranjeAspire.Web.Components;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents(); //ne secam se da li sam ovo dodao ja ili je ovo default, ali neka ostane
builder.Services.AddOutputCache();
builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        client.BaseAddress = new("https+http://apiservice");
    }
);

builder.Services.AddHttpClient<PoliseApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

builder.Services.AddHttpClient<ZaposleniApiClient>(client =>
{
    client.BaseAddress = new("https+http://apiservice");
});

//auth servis
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthorization();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//baca 404 na / ...logicno
app.MapGet("/", () => Results.Redirect("/home"));

/////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////
app.MapGet("/auth/callback", async (HttpContext ctx) =>
{
    const string secret = "aaanetsecret";
    
    var token = ctx.Request.Query["token"].ToString();
    if (string.IsNullOrWhiteSpace(token))
        return Results.Unauthorized();


    if (!TryValidateToken(token, secret, out var payload))
        return Results.Unauthorized();

    var claims = new List<Claim>
    {
        new("id", payload.id.ToString()),
        new("username", payload.username),
        new("imePrezime", payload.imePrezime),
        new("roleId", payload.roleId.ToString()),
    };

    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.Redirect("/home");
});

static bool TryValidateToken(string token, string secret, out (int id, string username, string imePrezime, int roleId) payload)
{
    payload = default;

    var parts = token.Split('.');
    if (parts.Length != 3) return false;

    var headerB64 = parts[0];
    var payloadB64 = parts[1];
    var sigB64 = parts[2];

    var data = $"{headerB64}.{payloadB64}";
    var expectedSig = HmacSha256B64Url(data, secret);

    if (!CryptographicOperations.FixedTimeEquals(Encoding.UTF8.GetBytes(sigB64), Encoding.UTF8.GetBytes(expectedSig)))
        return false;

    var json = Encoding.UTF8.GetString(B64UrlDecode(payloadB64));
    using var doc = JsonDocument.Parse(json);

    var exp = doc.RootElement.GetProperty("exp").GetInt64();
    if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() > exp) return false;

    var id = doc.RootElement.GetProperty("id").GetInt32();
    var username = doc.RootElement.GetProperty("username").GetString() ?? "";
    var imePrezime = doc.RootElement.GetProperty("imePrezime").GetString() ?? "";
    var roleId = doc.RootElement.GetProperty("roleId").GetInt32();

    if (string.IsNullOrWhiteSpace(username)) return false;

    payload = (id, username, imePrezime, roleId);
    return true;
}

static string HmacSha256B64Url(string data, string secret)
{
    var key = Encoding.UTF8.GetBytes(secret);
    using var hmac = new HMACSHA256(key);
    var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
    return B64UrlEncode(hash);
}

static byte[] B64UrlDecode(string s)
{
    s = s.Replace('-', '+').Replace('_', '/');
    switch (s.Length % 4)
    {
        case 2: s += "=="; break;
        case 3: s += "="; break;
    }
    return Convert.FromBase64String(s);
}

static string B64UrlEncode(byte[] bytes)
{
    return Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
}

/////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseOutputCache();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapDefaultEndpoints();
app.Run();
