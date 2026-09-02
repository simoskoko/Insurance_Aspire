using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsiguranjeAspire.ApiService.Models;

[Table("Korisnici")]
public class User
{
    [Key]
    [Column("Username")]
    public string? Username { get; set; }

    [Column("Password")]
    public string? Password { get; set; }
}
