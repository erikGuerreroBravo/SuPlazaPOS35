using DsiCodeTech.Common.DataAccess.Filter.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DsiCodeTech.Common.DataAccess.Filter.Query
{
    public class KitArticuloQuery
    {
        public string CodBarras { get; set; }

        public string CodInterno { get; set; }

        public string Descripcion { get; set; }

        public PageRequest Page { get; set; }
    }
}
