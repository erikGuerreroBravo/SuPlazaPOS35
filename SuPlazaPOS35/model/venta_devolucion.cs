using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.model
{
    public class venta_devolucion
    {
		public Guid id_devolucion { get; set; }

		public long folio { get; set; }

		public int id_pos { get; set; }

		public Guid id_venta { get; set; }

		public DateTime fecha_dev { get; set; }

		public decimal cant_dev { get; set; }

        public decimal descuento { get; set; }

        public decimal impuestos { get; set; }

        public string vendedor { get; set; }

		public string supervisor { get; set; }
	}
}
