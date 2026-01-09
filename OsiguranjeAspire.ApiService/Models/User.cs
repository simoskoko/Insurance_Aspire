using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsiguranjeAspire.ApiService.Models;

[Table("Users")]
public class User
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("email")]
    public string? Email { get; set; }

    [Column("passwordHash")]
    public string? Password { get; set; }
}
