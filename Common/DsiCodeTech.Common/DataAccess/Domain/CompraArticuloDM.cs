using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class CompraArticuloDM
    {
        public string CodigoBarras { get; set; }
        public short NumArticulos { get; set; }
        public decimal CantidadCja { get; set; }
        public decimal CantidadPza { get; set; }
        public decimal PrecioCompra { get; set; }
        public short NumCaptura { get; set; }
        public decimal NumEntrega { get; set; }
    }
}
