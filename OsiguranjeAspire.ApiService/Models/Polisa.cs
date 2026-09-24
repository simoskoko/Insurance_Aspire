using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OsiguranjeAspire.ApiService.Models
{
    public class Polisa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BrPolise { get; set; }
        public string JMBGNosilac { get; set; }
        public string ImeNosilac { get; set; }
        public string TipNosilac { get; set; }
        public int LOBId { get; set; }
        public decimal Premija { get; set; }
        public int VrstaPlacanjaId { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumIsteka { get; set; }
        public int IdZaposlenog { get; set; }
    }
}
