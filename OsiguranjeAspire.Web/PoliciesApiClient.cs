using OsiguranjeAspire.Contracts.Polise;

namespace OsiguranjeAspire.Web
{
    public class PoliciesApiClient
    {
            private readonly HttpClient _client;

            public PoliciesApiClient(HttpClient client)
            {
                _client = client;
            }

            public async Task<List<PolisaDTO>> GetAllAsync()
            {
                return await _client.GetFromJsonAsync<List<PolisaDTO>>("api/polise") ?? new();
            }
    }
}
    