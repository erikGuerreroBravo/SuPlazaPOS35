using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class VentaDM
    {
        public int IdPos { get; set; }

        public Guid IdVenta { get; set; }

        public string Vendedor { get; set; }

        public long Folio { get; set; }

        public DateTime FechaVenta { get; set; }

        public decimal TotalVendido { get; set; }

        public decimal PagoEfectivo { get; set; }

        public decimal PagoSpei { get; set; }

        public decimal PagoVales { get; set; }

        public decimal PagoTarCredito { get; set; }

        public decimal PagoTarDebito { get; set; }

        public string Supervisor { get; set; }

        public bool Upload { get; set; }

        public short NumRegistros { get; set; }

        public decimal Subtotal { get; set; }

        public decimal IepsDesglosado { get; set; }

        public decimal IvaDesglosado { get; set; }

        public decimal Impuestos { get; set; }

        public decimal Descuento { get; set; }

        public List<VentaArticuloDM> VentaArticulos { get; set; }
    }
}
