using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class ProveedorDM
    {
        public Guid IdProveedor { get; set; }

        public string Rfc { get; set; }

        public string RazonSocial { get; set; }

        public string NombreContacto { get; set; }

        public string TelPrincipal { get; set; }

        public string TelMovil { get; set; }

        public string Email { get; set; }

        public string Status { get; set; } = null;

        public DateTime FechaRegistro { get; set; }
    }
}
