using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class UsuarioPermisoDM
    {
        public string IdPermiso { get; set; }

        public string UserName { get; set; }

        public decimal? ValorNum { get; set; }

        public DateTime FechaRegistro { get; set; }
    }
}
