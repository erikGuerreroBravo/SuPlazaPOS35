using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class ImpuestoDM
    {
        public int? Id { get; set; }

        public string CodBarras { get; set; }

        public string Descripcion { get; set; }

        public decimal Iva { get; set; }

        public decimal Ieps { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
