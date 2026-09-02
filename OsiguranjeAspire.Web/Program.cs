using OsiguranjeAspire.Web;
using OsiguranjeAspire.Web.Components;
using OsiguranjeAspire.Web.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;


var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddRazorComponents().AddInteractiveServerComponents(); //ne secam se da li sam ovo dodao ja ili je ovo default, ali neka ostane
builder.Services.AddOutputCache();
builder.Services.AddHttpClient<WeatherApiClient>(client =>
    {
        client.BaseAddress = new("http://apiservice");
    }
);

builder.Services.AddHttpClient<PoliseApiClient>(client =>
{
    client.BaseAddress = new("http://apiservice");
});

builder.Services.AddHttpClient<ZaposleniApiClient>(client =>
{
    client.BaseAddress = new("http://apiservice");
});

// register auth API client used by the login endpoint
builder.Services.AddHttpClient<AuthApi>(client =>
{
    client.BaseAddress = new("http://apiservice");
});

//auth servis
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

var app = builder.Build();
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

//baca 404 na / ...logicno
app.MapGet("/", () => Results.Redirect("/home"));

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAntiforgery();
app.UseOutputCache();

app.MapDefaultEndpoints();
app.Run();
