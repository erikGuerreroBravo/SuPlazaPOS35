using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class CapturaInventarioDM
    {
        public Guid IdInventario { get; set; }

        public Guid IdCaptura { get; set; }

        public long NumCaptura { get; set; }

        public string CodigoBarras { get; set; }

        public DateTime FechaCaptura { get; set; }

        public decimal CantidadCja { get; set; }

        public decimal CantidadPza { get; set; }

        public bool Upload { get; set; }
    }
}
