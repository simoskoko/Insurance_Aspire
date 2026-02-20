using System.ComponentModel.DataAnnotations;

namespace OsiguranjeAspire.ApiService.Models
{
    public class Polisa
    {
        [Key]
        public int BrPolise { get; set; }
        public string JMBGNosilac { get; set; }
        public string ImeNosilac { get; set; }
        public string TipNosilac { get; set; }
        public int VrstaId { get; set; }
        public int LOBId { get; set; }
        public decimal Premija { get; set; }
        public int VrstaPlacanjaId { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumIsteka { get; set; }
        public int IdZaposlenog { get; set; }
    }
}
