using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class VentaCanceladaDM
    {
        public int IdPos { get; set; }

        public Guid IdVentaCancel { get; set; }

        public string Vendedor { get; set; }

        public DateTime Fecha { get; set; }

        public decimal TotalVendido { get; set; }

        public decimal PagoEfectivo { get; set; }

        public decimal PagoSpei { get; set; }

        public decimal PagoVales { get; set; }

        public decimal PagoTarCredito { get; set; }

        public decimal PagoTarDebito { get; set; }

        public string Status { get; set; }

        public string Supervisor { get; set; }

        public bool Upload { get; set; }

        public List<VentaCanceladaArticulo> Articulos { get; set; }
    }
}
