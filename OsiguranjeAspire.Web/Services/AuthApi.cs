using System.Net.Http.Json;

namespace OsiguranjeAspire.Web.Services;

public sealed class AuthApi(HttpClient http)
{
    public record LoginRequest(string Email, string Password);
    public async Task<string?> LoginAsync(string email, string password)
    {
        var resp = await http.PostAsJsonAsync("/api/auth/login", new LoginRequest(email, password));
        if (!resp.IsSuccessStatusCode) return null;
        var json = await resp.Content.ReadFromJsonAsync<LoginResponse>();
        return json?.token;
    }
    private record LoginResponse(string token);
}
