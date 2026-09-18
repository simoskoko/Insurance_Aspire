using OsiguranjeAspire.Contracts.Polise;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Json;

namespace OsiguranjeAspire.Web
{
    public sealed class PoliseApiException(HttpStatusCode statusCode) : Exception
    {
        public HttpStatusCode StatusCode { get; } = statusCode;
    }

    public class PoliseApiClient
    {
        private readonly HttpClient _client;
        private readonly AuthenticationStateProvider _authProvider;

        public PoliseApiClient(HttpClient client, AuthenticationStateProvider authProvider)
        {
            _client = client;
            _authProvider = authProvider;
        }

        public async Task<List<PolisaDTO>> GetAllAsync()
        {
            return await _client.GetFromJsonAsync<List<PolisaDTO>>("api/polise") ?? new();
        }

        public async Task<List<PolisaDTO>> GetByZaposleniAsync(int idZaposlenog)
        {
            return await _client.GetFromJsonAsync<List<PolisaDTO>>($"api/polise/zaposleni/{idZaposlenog}") ?? new();
        }

        public async Task<PolisaDTO> GetAsync(int brPolise)
        {
            using var request = await CreateRequestAsync(HttpMethod.Get, $"api/polise/{brPolise}");
            using var response = await _client.SendAsync(request);
            await EnsureSuccessAsync(response);
            return await response.Content.ReadFromJsonAsync<PolisaDTO>()
                ?? throw new InvalidOperationException("API nije vratila polisu.");
        }

        public async Task<PolisaDTO> UpdateAsync(PolisaDTO polisa)
        {
            using var request = await CreateRequestAsync(HttpMethod.Put, $"api/polise/{polisa.BrPolise}");
            request.Content = JsonContent.Create(polisa);
            using var response = await _client.SendAsync(request);
            await EnsureSuccessAsync(response);
            return await response.Content.ReadFromJsonAsync<PolisaDTO>()
                ?? throw new InvalidOperationException("API nije vratila ažuriranu polisu.");
        }

        private async Task<HttpRequestMessage> CreateRequestAsync(HttpMethod method, string uri)
        {
            var request = new HttpRequestMessage(method, uri);
            var authState = await _authProvider.GetAuthenticationStateAsync();
            var username = authState.User.FindFirst("username")?.Value;
            if (!string.IsNullOrWhiteSpace(username))
                request.Headers.Add("X-Username", username);
            return request;
        }

        private static async Task EnsureSuccessAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                _ = await response.Content.ReadAsStringAsync();
                throw new PoliseApiException(response.StatusCode);
            }
        }
    }
}
  