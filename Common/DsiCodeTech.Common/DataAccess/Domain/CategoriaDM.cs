using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class CategoriaDM
    {
        public long IdClasificacion { get; set; }

        public long? FkClasificacion { get; set; }

        public string Descripcion { get; set; }

        public short Nivel { get; set; }
    }
}
