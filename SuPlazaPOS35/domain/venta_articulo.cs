using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DsiCodeTech.Common.Util;

namespace SuPlazaPOS35.domain
{
    /// <summary>
    /// Company: DSI Tech
    /// Date: 18-05-2022
    /// Information: Está clase agrega funcionalidad fuera de la implementación de Linq.
    /// </summary>
    public partial class venta_articulo
    {
        public string unidad_medida { get; set; }

        public decimal cantidad_a_devolver { get; set; }

        public bool eliminar { get; set; }

        public decimal cant_vta()
        {
            return cantidad;
        }

        public decimal subTotal()
        {
            decimal subtotalGlobal = GetSubTotalGlobal();
            decimal unitarioDescuento = DsiCodeUtil.Round6Positions(subtotalGlobal * porcent_desc);
            decimal unitarioTotal = DsiCodeUtil.Round6Positions(subtotalGlobal - unitarioDescuento);
            return DsiCodeUtil.Round6Positions(unitarioTotal * cant_vta());
        }

        public decimal descuento()
        {
            return DsiCodeUtil.Round6Positions(TotalSale() * porcent_desc);
        }

        public decimal getIeps()
        {
            return hasIeps() ? DsiCodeUtil.Round6Positions(subTotal() * getValueIeps()) : 0;
        }

        public decimal getIVA()
        {
            return hasIva() ? DsiCodeUtil.Sum(subTotal(), getIeps()) * getValueIva() : 0;
        }

        public decimal total()
        {
            return DsiCodeUtil.Sum(subTotal(), getIeps(), getIVA());
        }

        public decimal subTotalDevolucion()
        {
            decimal subtotalGlobal = GetSubTotalGlobal();
            decimal unitarioDescuento = DsiCodeUtil.Round6Positions(subtotalGlobal * porcent_desc);
            decimal unitarioTotal = DsiCodeUtil.Round6Positions(subtotalGlobal - unitarioDescuento);
            return DsiCodeUtil.Round6Positions(unitarioTotal * cantidad_a_devolver);
        }

        public decimal descuentoDevolucion()
        {
            return DsiCodeUtil.Round6Positions(TotalSaleBack() * porcent_desc);
        }

        public decimal getIepsDevolucion()
        {
            return hasIeps() ? DsiCodeUtil.Round6Positions(subTotalDevolucion() * getValueIeps()) : 0;
        }

        public decimal getIVADevolucion()
        {
            return hasIva() ? DsiCodeUtil.Sum(subTotalDevolucion(), getIepsDevolucion()) * getValueIva() : 0;

        }

        public decimal totalDevolucion()
        {
            return DsiCodeUtil.Sum(subTotalDevolucion(), getIepsDevolucion(), getIVADevolucion());
        }

        public decimal cantidad_por_devolver()
        {
            return cantidad - cant_devuelta;
        }

        private decimal TotalSale()
        {
            return precio_vta * cant_vta();
        }

        private decimal TotalSaleBack()
        {
            return precio_vta * cantidad_a_devolver;
        }

        private decimal GetSubTotalGlobal()
        {
            decimal unitarioIva = hasIva() ? DsiCodeUtil.GetTotalIva(precio_vta, getValueIva()) : 0;
            decimal unitarioIeps = hasIeps() ? DsiCodeUtil.GetTotalIeps(precio_vta - unitarioIva, getValueIeps()) : 0;
            decimal unitarioSubtotal = DsiCodeUtil.GetSubtotal(precio_vta, unitarioIva, unitarioIeps);
            return DsiCodeUtil.Round6Positions(unitarioSubtotal);
        }

        decimal getValueIva() => articulo.impuestos[0].iva / 100;
        decimal getValueIeps() => articulo.impuestos[0].ieps / 100;

        public bool hasIva() => articulo.impuestos[0].iva > 0;
        public bool hasIeps() => articulo.impuestos[0].ieps > 0;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}



