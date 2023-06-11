using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class FacturaVentaDM
    {
        public int IdPos { get; set; }

        public Guid IdVenta { get; set; }

        public long IdFactura { get; set; }

        public long Folio { get; set; }

        public string PathXml { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
