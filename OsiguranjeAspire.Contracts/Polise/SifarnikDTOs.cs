namespace OsiguranjeAspire.Contracts.Polise;

public sealed class SifarnikLobDTO
{
    public int LobId { get; set; }
    public string NazivLob { get; set; } = string.Empty;
}

public sealed class SifarnikVrstaPlacanjaDTO
{
    public int VrstaPlacanjaId { get; set; }
    public string NazivVrstaPlacanja { get; set; } = string.Empty;
}
