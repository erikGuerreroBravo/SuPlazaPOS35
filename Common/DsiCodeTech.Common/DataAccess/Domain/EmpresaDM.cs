using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class EmpresaDM
    {
        public string Rfc { get; set; }

        public string RazonSocial { get; set; }

        public string Representante { get; set; }

        public string Calle { get; set; }

        public string NoExterior { get; set; }

        public string NoInterior { get; set; }

        public string Colonia { get; set; }

        public string Municipio { get; set; }

        public string Estado { get; set; }

        public string Pais { get; set; }

        public string CodPostal { get; set; }

        public string TelPrincipal { get; set; }

        public string Email { get; set; }

        public DateTime FechaRegistro { get; set; }

        public string IdRegimenFiscal { get; set; }

        public string RegimenFiscal { get; set; }
    }
}
