using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class UuidRelacionDM
    {
        public int Id { get; set; }

        public int IdFacturaRelacion { get; set; }

        public Guid Uuid { get; set; }
    }
}
