using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SuPlazaPOS35.model
{
    public class venta_devolucion_articulo
    {
        public string cod_barras { get; set; }

        public long no_articulo { get; set; }

        public decimal cantidad { get; set; }

        public string descripcion { get; set; }

        public string descripcion_corta { get; set; }

        public string medida { get; set; }

        public decimal iva { get; set; }

        public decimal precio_vta { get; set; }

        public decimal porcent_desc { get; set; }

        public decimal cant_devuelta { get; set; }

        public decimal subTotalDevolucion()
        {
            return precio_vta * cant_devuelta / (1.0m + iva);
        }

        public decimal descuentoDevolucion()
        {
            return subTotalDevolucion() * porcent_desc;
        }

        public decimal getIVADevolucion()
        {
            return (subTotalDevolucion() - descuentoDevolucion()) * iva;
        }

        public decimal totalDevolucion()
        {
            return subTotalDevolucion() + getIVADevolucion() - descuentoDevolucion();
        }
    }
}
