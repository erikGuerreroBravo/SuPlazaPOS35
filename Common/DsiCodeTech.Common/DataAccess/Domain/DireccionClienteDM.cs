using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DsiCodeTech.Common.DataAccess.Domain
{
    public class DireccionClienteDM
    {
        public int Id { get; set; }

        public Guid IdCliente { get; set; }

        public string CodPostal { get; set; }

        public string NoInterior { get;set; }

        public string NoExterior { get; set; }

        public string Calle { get; set; }

        public string Colonia { get; set; }

        public DateTime FechaRegistro { get; set; }

        public short? IdMunicipio { get; set; }

        public short? IdEntidad { get; set; }
    }
}
