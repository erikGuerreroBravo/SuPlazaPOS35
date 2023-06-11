using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class InventarioFisicoDM
    {
        public Guid IdInventario { get; set; }

        public DateTime FechaInicial { get; set; }

        public DateTime? FechaFinal { get; set; }

        public DateTime FechaRegistro { get; set; }

        public Guid IdProveedor { get; set; }

        public ProveedorDM Proveedor { get; set; }

        public string UserName { get; set; }

        public UsuarioDM Usuario { get; set; }

        public List<CapturaInventarioDM> Articulos { get; set; }
    }
}
