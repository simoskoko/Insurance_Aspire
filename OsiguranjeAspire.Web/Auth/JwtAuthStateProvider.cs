using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace OsiguranjeAspire.Web.Auth;

public sealed class JwtAuthStateProvider(IJSRuntime js) : AuthenticationStateProvider
{
    const string Key = "auth_token";

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", Key);
        return BuildState(token);
    }

    public async Task SetTokenAsync(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            await js.InvokeVoidAsync("localStorage.removeItem", Key);
        else
            await js.InvokeVoidAsync("localStorage.setItem", Key, token);

        NotifyAuthenticationStateChanged(Task.FromResult(BuildState(token)));
    }

    public static ClaimsPrincipal ParseClaimsFromJwt(string? jwt)
    {
        if (string.IsNullOrWhiteSpace(jwt)) return new ClaimsPrincipal(new ClaimsIdentity());
        var parts = jwt.Split('.');
        if (parts.Length != 3) return new ClaimsPrincipal(new ClaimsIdentity());

        var payload = parts[1].PadRight(parts[1].Length + (4 - parts[1].Length % 4) % 4, '=');
        var json = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(payload.Replace('-', '+').Replace('_', '/')));
        using var doc = JsonDocument.Parse(json);
        var claims = doc.RootElement.EnumerateObject()
            .Select(p => new Claim(p.Name switch
            {
                "name" => ClaimTypes.Name,
                "unique_name" => ClaimTypes.Name,
                "nameid" => ClaimTypes.NameIdentifier,
                "role" or "roles" => ClaimTypes.Role,
                _ => p.Name
            }, p.Value.ValueKind == JsonValueKind.Array
                    ? string.Join(",", p.Value.EnumerateArray().Select(e => e.GetString()))
                    : p.Value.ToString()));
        return new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
    }

    static AuthenticationState BuildState(string? token)
        => new(ParseClaimsFromJwt(token));
}

public sealed class TokenHttpClientHandler(JwtAuthStateProvider auth, IJSRuntime js) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        var token = await js.InvokeAsync<string?>("localStorage.getItem", "auth_token");
        if (!string.IsNullOrWhiteSpace(token))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return await base.SendAsync(request, ct);
    }
}
