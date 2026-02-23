using System.ComponentModel.DataAnnotations;

namespace OsiguranjeAspire.ApiService.Models
{
    public class Zaposleni
    {
        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Lob { get; set; }
        public int RoleId { get; set; }
        public string Jmbg { get; set; }
        public int? NadredjeniId { get; set; }
        public string ImePrezime { get; set; }
    }
}
