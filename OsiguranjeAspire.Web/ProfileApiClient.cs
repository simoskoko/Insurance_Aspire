using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;
using OsiguranjeAspire.Contracts.Korisnici;

namespace OsiguranjeAspire.Web;

public sealed class ProfileApiClient(
    HttpClient httpClient,
    AuthenticationStateProvider authenticationStateProvider)
{
    public async Task<IReadOnlyList<AdminProfileSearchDTO>> GetAdminProfilesAsync()
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, "api/admin/profiles");
        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Učitavanje korisnika nije uspelo.");

        return await response.Content.ReadFromJsonAsync<List<AdminProfileSearchDTO>>() ?? [];
    }

    public async Task<ProfileDTO> GetByIdAsync(int userId)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, $"api/profile/{userId}");
        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Učitavanje profila nije uspelo ({(int)response.StatusCode}).");

        return await response.Content.ReadFromJsonAsync<ProfileDTO>()
            ?? throw new InvalidOperationException("API nije vratila podatke profila.");
    }

    public async Task UpdateAsync(int userId, ProfileUpdateDTO profile)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put, $"api/profile/{userId}");
        request.Content = JsonContent.Create(profile);

        using var response = await httpClient.SendAsync(request);
        
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(
            $"Čuvanje profila nije uspelo. " +
            $"Status: {(int)response.StatusCode} {response.ReasonPhrase}. " +
            $"Response: {body}");
        }

        //if (!response.IsSuccessStatusCode)
        //    throw new HttpRequestException("Čuvanje profila nije uspelo.");
    }

    public async Task<ProfileDTO> GetCurrentAsync()
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Get, "api/profile/me");

        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException($"Učitavanje profila nije uspelo ({(int)response.StatusCode}).");

        return await response.Content.ReadFromJsonAsync<ProfileDTO>()
            ?? throw new InvalidOperationException("API nije vratila podatke profila.");
    }

    public async Task UpdateAsync(ProfileUpdateDTO profile)
    {
        using var request = await CreateAuthenticatedRequestAsync(HttpMethod.Put, "api/profile/me");
        request.Content = JsonContent.Create(profile);

        using var response = await httpClient.SendAsync(request);
        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException("Čuvanje profila nije uspelo.");
    }

    private async Task<HttpRequestMessage> CreateAuthenticatedRequestAsync(HttpMethod method, string uri)
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var username = authState.User.FindFirst("username")?.Value;

        if (string.IsNullOrWhiteSpace(username))
            throw new InvalidOperationException("Korisnik nije prijavljen.");

        var request = new HttpRequestMessage(method, uri);
        request.Headers.Add("X-Username", username);
        return request;
    }
}
