using System;
using System.Collections.Generic;
using System.Linq;

using DsiCodeTech.Business;
using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository.Infraestructure;
using SuPlazaPOS35.model;

using DsiCodeTech.Common.Util;

namespace SuPlazaPOS35.controller
{
    public class SuPlazaPosUtil
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOfertaBusiness _ofertaBusiness;
        private readonly IVentaBusiness _ventaBusiness;
        private readonly IVentaDevolucionBusiness _ventaDevolucionBusiness;
        private readonly IPosSettingsBusiness _posSettingsBusiness;

        private static readonly DsiCodeUtil DsiCodeUtil = DsiCodeUtil.Instance;
        public SuPlazaPosUtil()
        {
            this._unitOfWork = new UnitOfWork();
            this._ofertaBusiness = new OfertaBusiness(this._unitOfWork);
            this._ventaBusiness = new VentaBusiness(this._unitOfWork);
            this._ventaDevolucionBusiness = new VentaDevolucionBusiness(this._unitOfWork);
            this._posSettingsBusiness = new PosSettingsBusiness(this._unitOfWork);
        }

        public SuPlazaPOS35.domain.venta_articulo GetOffer(SuPlazaPOS35.domain.venta_articulo venta)
        {
            DsiCodeTech.Repository.PosCaja.oferta_articulo oferta = this._ofertaBusiness.GetActiveOfferByCodBar(venta.cod_barras);

            venta.articulo_ofertado = !(oferta is null);
            venta.precio_vta = !(oferta is null) ? oferta.precio_oferta : venta.precio_vta;

            return venta;
        }

        public SuPlazaPOS35.model.corte GetCorteByDate(DateTime end)
        {
            DateTime start = this._posSettingsBusiness.GetPosSettings().last_corte_z;

            //DateTime inicio = new DateTime(2023, 05, 07, 17, 07, 01);
            //DateTime fin = new DateTime(2023, 06, 14, 17, 30, 35);

            List<DsiCodeTech.Repository.PosCaja.venta> ventas = this._ventaBusiness.GetVentasByDates(start, end);

            //Acomulador cuando un producto tiene Iva
            decimal total_desglosado_iva = 0m;
            //Acomulador cuando un producto tiene Ieps.
            decimal total_desglosado_ieps = 0m;

            decimal total_iva = 0m;
            decimal total_ieps = 0m;
            decimal total_exento = 0;

            List<DsiCodeTech.Repository.PosCaja.venta_articulo> articulos = ventas.SelectMany(va => va.venta_articulo).ToList();

            foreach (var item in articulos)
            {
                decimal cantidad_real = item.cantidad - item.cant_devuelta;

                if (cantidad_real == 0)
                    continue;

                decimal productoPrecioVta = item.precio_vta;
                decimal productoIva = item.iva;
                decimal productoIeps = item.ieps.Value;
                decimal productoPorcentaje = item.porcent_desc;

                /* Identificamos si aplica impuestos */
                bool containsIeps = item.ieps > 0.0m;
                bool containsIva = item.iva > 0.0m;

                /* Calculos para obtener el subtotal */
                decimal unitarioIva = containsIva ? DsiCodeUtil.GetTotalIva(productoPrecioVta, productoIva) : 0;
                decimal unitarioIeps = containsIeps ? DsiCodeUtil.GetTotalIeps(productoPrecioVta - unitarioIva, productoIeps) : 0;
                decimal unitarioSubtotal = DsiCodeUtil.GetSubtotal(productoPrecioVta, unitarioIva, unitarioIeps);
                decimal valorUnitario = DsiCodeUtil.Round6Positions(unitarioSubtotal);

                decimal unitarioDescuento = DsiCodeUtil.Round6Positions(valorUnitario * productoPorcentaje);
                decimal unitarioTotal = DsiCodeUtil.Round6Positions(valorUnitario - unitarioDescuento);
                decimal subtotalFinal = DsiCodeUtil.Round6Positions(unitarioTotal * cantidad_real);

                decimal totalIeps = containsIeps ? DsiCodeUtil.Round6Positions(subtotalFinal * productoIeps) : 0;
                decimal totalIva = containsIva ? DsiCodeUtil.Round6Positions(DsiCodeUtil.Sum(subtotalFinal, totalIeps) * productoIva) : 0;

                /* Calculamos los subtotales */
                total_desglosado_ieps += containsIeps && !containsIva ? subtotalFinal : 0;
                total_desglosado_iva += containsIva ? DsiCodeUtil.Round6Positions(DsiCodeUtil.Sum(subtotalFinal, totalIeps)) : 0;

                total_ieps += containsIeps && !containsIva ? totalIeps : 0;
                total_iva += containsIva ? totalIva : 0;
            }

            decimal total_spei = ventas.Where(spei => spei.pago_spei > 0m).Aggregate(0m, (total, spei) => total + spei.pago_spei);
            decimal total_td = ventas.Where(debito => debito.pago_td > 0m).Aggregate(0m, (total, debito) => total + debito.pago_td);
            decimal total_tc = ventas.Where(credito => credito.pago_tc > 0m).Aggregate(0m, (total, credito) => total + credito.pago_tc);
            decimal total_vales = ventas.Where(vales => vales.pago_vales > 0m).Aggregate(0m, (total, vales) => total + vales.pago_vales);
            decimal total_vendido = ventas.Aggregate(0m, (total, vendido) => total + vendido.total_vendido);
            decimal total_efectivo = total_vendido - DsiCodeUtil.Sum(total_spei, total_td, total_tc, total_vales);

            long min_folio = ventas.Min(folio => folio.folio);
            long max_folio = ventas.Max(folio => folio.folio);
            int num_transacciones = ventas.Count;
            int pos = ventas.First().id_pos;

            List<DsiCodeTech.Repository.PosCaja.venta_devolucion> devoluciones = this._ventaDevolucionBusiness.GetDevolucionesByDates(start, end);

            decimal total_devolucion = 0m;
            if (devoluciones.Any())
            {
                total_devolucion = devoluciones.Sum(d => d.cant_dev);
            }

            decimal impuestos = DsiCodeUtil.Sum(total_desglosado_ieps, total_desglosado_iva, total_ieps, total_iva);
            decimal venta_total = total_vendido - total_devolucion;
            total_exento = venta_total - impuestos;

            decimal impuestoRound = DsiCodeUtil.Sum(
                Math.Round(total_desglosado_ieps, 2),
                Math.Round(total_desglosado_iva, 2),
                Math.Round(total_ieps, 2),
                Math.Round(total_iva, 2));
            total_exento = DsiCodeUtil.Sum(Math.Round(total_exento, 2), impuestoRound) <= venta_total ? Math.Round(total_exento, 2) : total_exento;

            return new corte
            {
                id_pos = pos,
                folio_ini = min_folio,
                folio_fin = max_folio,
                fecha_ini = start,
                fecha_fin = end,
                efectivo = total_efectivo,
                pago_spei = total_spei,
                pago_td = total_td,
                pago_vales = total_vales,
                pago_tc = total_tc,
                pago_efectivo = total_efectivo,
                total_exentos = total_exento,
                total_vendido = total_vendido,
                total_desglosado_iva = total_desglosado_iva,
                total_desglosado_ieps = total_desglosado_ieps,
                total_devuelto = total_devolucion,
                iva = total_iva,
                ieps = total_ieps,
                no_transacciones = num_transacciones
            };
        }


        public void CuadrarTotales(ref decimal subtotal,  decimal venta_total, decimal venta_calculada) 
        {
            if (venta_calculada > venta_total)
            {
                decimal min = venta_calculada - venta_total;
                subtotal = subtotal - min;
            }
            else if (venta_calculada < venta_total)
            {
                decimal max = venta_total - venta_calculada;
                subtotal = subtotal + max;
            }
        }
    }
}