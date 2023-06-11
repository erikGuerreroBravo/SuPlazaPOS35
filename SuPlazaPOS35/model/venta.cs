using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.model
{
    public class venta
    {
		public int id_pos { get; set; }

		public Guid id_venta { get; set; }

		public string vendedor { get; set; }

		public long folio { get; set; }

		public DateTime fecha_venta { get; set; }

		public decimal total_vendido { get; set; }

		public decimal pago_efectivo { get; set; }

		public decimal pago_cheque { get; set; }

		public decimal pago_vales { get; set; }

		public decimal pago_tc { get; set; }

		public string supervisor { get; set; }

		/// <summary>
		/// se modifica la propiedad para el pago de la tarjeta de debito
		/// </summary>
		public decimal pago_td { get; set; }

		public int no_articulo { get; set; }

		/// <summary>
		/// Se agrega la propiedad spei spei para transacciones bancarias
		/// </summary>
		public decimal pago_spei { get; set; }

		public List<venta_articulo> articulos { get; set; }


    }
}
