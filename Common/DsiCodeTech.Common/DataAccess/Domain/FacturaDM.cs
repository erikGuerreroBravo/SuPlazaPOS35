using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class FacturaDM
    {
        public long IdFactura { get; set; }

        public DateTime FechaRemision { get; set; }

        public Guid IdCliente { get; set; }

        public string IdComprobante { get; set; }

        public string IdMetodoPago { get; set; }

        public string IdUsoCfdi { get; set; }

        public string IdFormaPago { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public string PathXml { get; set; }

        public CfdiFormaPagoDM FormaPago { get; set; }

        public CfdiUsoCfdiDM UsoCfdi { get; set; }

        public CfdiMetodoPagoDM MetodoPago { get; set; }

        public CfdiTipoComprobanteDM TipoComprobante { get; set; }

        public ClienteDM Cliente { get; set; }

        public List<FacturaVentaDM> Ventas { get; set; }

        public List<FacturaRelacionDM> CfdiRelacionados { get; set;}
    }
}
