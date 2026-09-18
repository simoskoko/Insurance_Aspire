using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace OsiguranjeAspire.Web.Auth;

public sealed class SessionAuthenticationStateProvider : AuthenticationStateProvider
{
    private ClaimsPrincipal currentUser = new(new ClaimsIdentity());

    public override Task<AuthenticationState> GetAuthenticationStateAsync()
        => Task.FromResult(new AuthenticationState(currentUser));

    public void SetUser(string username, int? roleId, int? id)
    {
        var claims = new List<Claim>
        {
            new("username", username)
        };

        if (roleId.HasValue)
            claims.Add(new Claim("roleId", roleId.Value.ToString()));
        if (id.HasValue)
            claims.Add(new Claim("id", id.Value.ToString()));

        currentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "session"));
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
