using DsiCodeTech.Common.DataAccess.Filter.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Filter.Query
{
    public class InventarioFisicoQuery
    {
        public Guid? IdProveedor { get; set; }

        public PageRequest Page { get; set; }
    }
}
