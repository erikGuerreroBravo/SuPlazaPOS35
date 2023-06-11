using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class OrdenArticuloDM
    {
        public short NumArticulo { get; set; }

        public string CodigoBarras { get; set; }

        public string CodigoAnexo { get; set; }

        public decimal Cantidad { get; set; }

        public decimal PrecioArticulo { get; set; }

        public decimal PorSurtir { get; set; }

        public decimal PorSurtirPza { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
