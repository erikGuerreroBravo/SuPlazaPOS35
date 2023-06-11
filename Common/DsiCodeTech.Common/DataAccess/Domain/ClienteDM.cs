using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class ClienteDM
    {
        public Guid IdCliente { get; set; }

        public string Rfc { get; set; }

        public string RazonSocial { get; set; }

        public string Contacto { get; set; }

        public string Email { get; set; }

        public string Email2 { get; set; }

        public string RegimenFiscal { get; set; }

        public DireccionClienteDM Direccion { get; set; }
    }
}
