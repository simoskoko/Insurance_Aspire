using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using OsiguranjeAspire.Contracts.Korisnici;

namespace OsiguranjeAspire.Web;

public sealed class ProfileApiClient(
    HttpClient httpClient,
    AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<ProfileDTO> GetCurrentAsync()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "api/profile/me");
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var username = authState.User.FindFirst("username")?.Value;

        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException("Korisnik nije prijavljen.");

        request.Headers.Add("X-Username", username);

        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Učitavanje profila nije uspelo ({(int)response.StatusCode}).");

        return await response.Content.ReadFromJsonAsync<ProfileDTO>()
            ?? throw new InvalidOperationException("API nije vratila podatke profila.");
    }
}
