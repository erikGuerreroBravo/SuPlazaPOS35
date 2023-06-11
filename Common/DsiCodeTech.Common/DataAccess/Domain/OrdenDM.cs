using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class OrdenDM
    {
        public long NumPedido { get; set; }

        public string StatusPedido { get; set; }

        public short NumDias { get; set; }

        public string Plan { get; set; }

        public short Anio { get; set; }

        public short Mes { get; set; }

        public Guid IdPedido { get; set; }

        public DateTime FechaPedido { get; set; }

        public DateTime? FechaAutorizado { get; set; }

        public DateTime FechaRegistro { get; set; }

        public List<OrdenArticuloDM> Articulos { get; set; }

        public Guid IdProveedor { get; set; }

        public ProveedorDM Proveedor { get; set; }
    }
}
