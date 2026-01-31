using System;
using System.Collections.Generic;
using System.Text;

namespace OsiguranjeAspire.Contracts.Polise
{
    public class PolisaDTO
    {
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
    }
}
