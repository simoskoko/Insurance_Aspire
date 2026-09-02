using System.Net.Http.Json;

namespace OsiguranjeAspire.Web.Services;

public sealed class AuthApi(HttpClient http)
{
    public record LoginRequest(string Username, string Password);
    public record LoginResponse(string username);

    public async Task<string?> LoginAsync(string username, string password)
    {
        var resp = await http.PostAsJsonAsync("/api/auth/login", new LoginRequest(username, password));

        if (!resp.IsSuccessStatusCode)
        {
            var errorContent = await resp.Content.ReadAsStringAsync();
            Console.WriteLine($"Login failed with status {resp.StatusCode}: {errorContent}");
            return null;
        }

        var json = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        return json?.username;
    }
}
