using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class FacturaRelacionDM
    {
        public int Id { get; set; }

        public long IdFactura { get; set; }

        public string IdTipoRelacion { get; set; }

        public List<UuidRelacionDM> UuidRalacionados { get; set; }
    }
}
