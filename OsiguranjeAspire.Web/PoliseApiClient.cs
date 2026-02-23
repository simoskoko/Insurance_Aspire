using OsiguranjeAspire.Contracts.Polise;

namespace OsiguranjeAspire.Web
{
    public class PoliseApiClient
    {
        private readonly HttpClient _client;

        public PoliseApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<PolisaDTO>> GetAllAsync()
        {
            return await _client.GetFromJsonAsync<List<PolisaDTO>>("api/polise") ?? new();
        }

        public async Task<List<PolisaDTO>> GetByZaposleniAsync(int idZaposlenog)
        {
            return await _client.GetFromJsonAsync<List<PolisaDTO>>($"api/polise/zaposleni/{idZaposlenog}") ?? new();
        }
    }
}
  