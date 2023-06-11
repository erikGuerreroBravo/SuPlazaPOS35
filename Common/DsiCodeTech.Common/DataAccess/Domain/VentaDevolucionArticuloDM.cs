using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class VentaDevolucionArticuloDM
    {
        public Guid IdDevolucion { get; set; }

        public long NoArticulo { get; set; }

        public string CodBarras { get; set; }

        public decimal Cantidad { get; set; }
    }
}
