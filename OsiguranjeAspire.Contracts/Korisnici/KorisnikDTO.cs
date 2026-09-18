namespace OsiguranjeAspire.Contracts.Korisnici;

public sealed class KorisnikDTO
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int Lob { get; set; }
    public int RoleId { get; set; }
    public string Jmbg { get; set; } = string.Empty;
    public int? NadredjeniId { get; set; }
    public string ImePrezime { get; set; } = string.Empty;
}
