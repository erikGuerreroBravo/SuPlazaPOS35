using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.model
{
    public class corte
    {
        public int id_pos { get; set; }

        public DateTime fecha_ini { get; set; }

        public DateTime fecha_fin { get; set; }

        public long folio_ini { get; set; }

        public long folio_fin { get; set; }

        public decimal efectivo { get; set; }

        public decimal cheques { get; set; }

        public decimal vales { get; set; }

        public decimal tc { get; set; }

        public int no_transacciones { get; set; }

        public decimal total_vendido { get; set; }

        public decimal total_desglosado_iva { get; set; }

        public decimal total_desglosado_ieps { get; set; }

        public decimal total_exentos { get; set; }

        public decimal iva { get; set; }

        public decimal ieps { get; set; }

        public decimal total_devuelto { get; set; }

        public decimal pago_td { get; set; }

        public decimal pago_tc { get; set; }

        public decimal pago_spei { get; set; }

        public decimal pago_vales { get; set; }

        public decimal pago_efectivo { get; set; }
    }
}