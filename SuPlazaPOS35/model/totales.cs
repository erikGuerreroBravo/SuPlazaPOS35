using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.model
{
    public class totales
    {
        public decimal total_articulos { get; set; }

        public decimal sub_total { get; set; }

        public decimal descuento { get; set; }

        public decimal iva { get; set; }

        public decimal total { get; set; }

        public decimal ieps { get; set; }

        public decimal impuestos { get; set; }
    }
}
