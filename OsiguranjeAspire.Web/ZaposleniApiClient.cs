using OsiguranjeAspire.Contracts.Zaposleni;

namespace OsiguranjeAspire.Web
{
    public class ZaposleniApiClient
    {
        private readonly HttpClient _client;

        public ZaposleniApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<ZaposleniDTO>> GetZaposleni()
    => await _client.GetFromJsonAsync<List<ZaposleniDTO>>("api/zaposleni") ?? new();
    }
}
