using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.model
{
    public class venta_cancelada
    {
		public List<venta_cancelada_articulo> venta_cancelada_articulo;

		public int id_pos { get; set; }

		public Guid id_venta_cancel { get; set; }

		public string vendedor { get; set; }

		public DateTime fecha { get; set; }

		public decimal total_vendido { get; set; }

		public decimal pago_efectivo { get; set; }

		public decimal pago_cheque { get; set; }

		public decimal pago_vales { get; set; }

		public decimal pago_tc { get; set; }

		public string status { get; set; }

		public string supervisor { get; set; }

		///se modifica  con dos columnas mas
		public decimal pago_td { get; set; }

		public decimal pago_spei { get; set; }

	}
}
