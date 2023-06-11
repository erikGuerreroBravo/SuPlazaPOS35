using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class VentaDevolucionDM
    {
        public Guid IdDevolucion { get; set; }

        public long Folio { get; set; }

        public int IdPos { get; set; }

        public Guid IdVenta { get; set; }

        public DateTime FechaDevolucion { get; set; }

        public string Vendedor { get; set; }

        public string Supervisor { get; set; }

        public decimal CantidadDevuelta { get; set; }

        public bool Upload { get; set; }

        public decimal Impuestos { get; set; }

        public decimal Descuento { get; set; }

        public List<VentaDevolucionArticuloDM> Articulos { get; set; }
    }
}
