using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class UsuarioDM
    {
        public string UserName { get; set; }

        public string Password { get; set; }

        public string TipoUsuario { get; set; }

        public DateTime FechaRegistro { get; set; }

        public short? UserCodeBacula { get; set; }

        public bool IsEnable { get; set; }

        public List<UsuarioPermisoDM> Permisos { get; set; }
    }
}
