using OsiguranjeAspire.Contracts.Korisnici;

namespace OsiguranjeAspire.Web
{
    public class ZaposleniApiClient
    {
        private readonly HttpClient _client;

        public ZaposleniApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task<List<KorisnikDTO>> GetZaposleni()
    => await _client.GetFromJsonAsync<List<KorisnikDTO>>("api/korisnici") ?? new();

        public async Task<List<KorisnikDTO>> GetPodredjeniAsync(int nadredjeniId)
        {
            return await _client.GetFromJsonAsync<List<KorisnikDTO>>(
                $"api/korisnici/podredjeni/{nadredjeniId}"
            ) ?? new();
        }
    }
}
