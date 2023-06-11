using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.model
{
    public class venta_cancelada_articulo
    {
        public int id_pos { get; set; }

        public Guid id_venta_cancel { get; set; }

        public long no_articulo { get; set; }

        public string cod_barras { get; set; }

        public string descripcion { get; set; }

        public string unidad_medida { get; set; }

        public decimal cantidad { get; set; }

        public bool articulo_ofertado { get; set; }

        public decimal precio_regular { get; set; }

        public bool cambio_precio { get; set; }

        public decimal iva { get; set; }

        public decimal precio_vta { get; set; }

        public decimal porcent_desc { get; set; }

        public decimal cant_dev { get; set; }

        public string user_name { get; set; }
        /// <summary>
        /// se agrega  el campo ieps en la venta cancelada articulo el campo de iva ya lo tenia
        /// </summary>
        public decimal ieps { get; set; }


        public decimal cant_vta()
        {
            return cantidad;
        }

        public decimal subTotal()
        {
            return precio_vta * cant_vta() / (1.0m + iva);
        }

        public decimal descuento()
        {
            return subTotal() * porcent_desc;
        }

        public decimal getIVA()
        {
            return (subTotal() - descuento()) * iva;
        }

        public decimal total()
        {
            return subTotal() + getIVA() - descuento();
        }
    }
}
