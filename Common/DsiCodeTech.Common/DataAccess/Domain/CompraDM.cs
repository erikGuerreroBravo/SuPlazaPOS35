using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class CompraDM
    {
        public Guid IdCompra { get; set; }
        public bool Cancelada { get; set; }
        public short NumEntrada { get; set; }
        public DateTime FechaCompra { get; set; }
        public string NoFactura { get; set; }
        public string Observaciones { get; set; }
        public Guid IdProveedor { get; set; }
        public Guid IdPedido { get; set; }
        public List<CompraArticuloDM> Articulos { get; set; }
    }
}
