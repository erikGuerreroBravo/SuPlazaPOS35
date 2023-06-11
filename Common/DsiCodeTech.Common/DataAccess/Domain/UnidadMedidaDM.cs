using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class UnidadMedidaDM
    {
        public Guid IdUnidad { get; set; }

        public string Descripcion { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string CveSat { get; set; }
    }
}
