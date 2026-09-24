using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsiguranjeAspire.ApiService.Models;

[Table("SifarnikLOB")]
public sealed class SifarnikLob
{
    [Key]
    public int LobId { get; set; }
    public string NazivLob { get; set; } = string.Empty;
}

[Table("SifarnikVrstaPlacanja")]
public sealed class SifarnikVrstaPlacanja
{
    [Key]
    public int VrstaPlacanjaId { get; set; }
    public string NazivVrstaPlacanja { get; set; } = string.Empty;
}