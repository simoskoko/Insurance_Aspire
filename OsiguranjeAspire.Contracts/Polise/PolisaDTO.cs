using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OsiguranjeAspire.Contracts.Polise
{
    public class PolisaDTO
    {
        public int BrPolise { get; set; }
        [Required]
        public string JMBGNosilac { get; set; }
        [Required]
        public string ImeNosilac { get; set; }
        [Required]
        public string TipNosilac { get; set; }
        public int LOBId { get; set; }
        [Range(0, double.MaxValue)]
        public decimal Premija { get; set; }
        public int VrstaPlacanjaId { get; set; }
        public DateTime DatumPocetka { get; set; }
        public DateTime DatumIsteka { get; set; }
        public int IdZaposlenog { get; set; }

    }
}
