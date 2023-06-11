using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Common.Exception;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using DsiCodeTech.Repository.PosCaja;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DsiCodeTech.Business
{
    public class VentaBusiness : IVentaBusiness
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly VentaRepository repository;
        private readonly IArticuloBusiness articuloBusiness;
        public VentaBusiness(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            repository = new VentaRepository(unitOfWork);
        }


        public VentaBusiness()
        {
            unitOfWork = new UnitOfWork();
            repository = new VentaRepository(unitOfWork);
            articuloBusiness = new ArticuloBusiness(unitOfWork);
        }

        public VentaDM GetVentaByFolio(long folio)
        {
            try
            {
                Repository.PosCaja.venta venta = this.repository.GetIncludeAll(p => p.folio == folio, "venta_articulo").FirstOrDefault();

                if (venta == null)
                {
                    throw new BusinessException(DsiCodeConst.RESULT_WITHOUT_DATA_ID, DsiCodeConst.RESULT_WITHOUT_DATA);
                }
                var ventaDM = new VentaDM
                {
                    IdPos = venta.id_pos,
                    IdVenta = venta.id_venta,
                    Vendedor = venta.vendedor,
                    Folio = venta.folio,
                    FechaVenta = venta.fecha_venta,
                    TotalVendido = venta.total_vendido,
                    PagoEfectivo = venta.pago_efectivo,
                    PagoVales = venta.pago_vales,
                    PagoTarCredito = venta.pago_tc,
                    Supervisor = venta.supervisor,
                    Upload = venta.upload,
                    NumRegistros = venta.num_registros,
                    PagoTarDebito = venta.pago_td,
                    PagoSpei = venta.pago_spei,
                    VentaArticulos = venta.venta_articulo is null ? null : venta.venta_articulo.Select(va => new VentaArticuloDM()
                    {
                        IdPos = va.id_pos,
                        IdVenta = va.id_venta,
                        NumArticulo = va.no_articulo,
                        CodBarras = va.cod_barras,
                        UserCodeBascula = va.user_code_bascula != null ? va.user_code_bascula.Value : (short)0,
                        Cantidad = va.cantidad,
                        ArticuloOfertado = va.articulo_ofertado,
                        PrecioRegular = va.precio_regular,
                        CambioPrecio = va.cambio_precio,
                        Iva = va.iva,
                        PrecioVenta = va.precio_vta,
                        PorcDescuento = va.porcent_desc,
                        CantDevuelta = va.cant_devuelta,
                        UserName = va.user_name,
                        IdPromo = va.id_promo != null ? va.id_promo.Value : Guid.Empty,
                        NoPromoAplicado = va.no_promo_aplicado != null ? va.no_promo_aplicado.Value : (short)0,
                        Ieps = va.ieps.Value,
                        PrecioCompra = va.articulo.precio_compra,
                        Utilidad = va.articulo.utilidad
                    }).ToList(),


                };
                ventaDM.VentaArticulos = ventaDM.VentaArticulos.Select(p =>
                {
                    VentaArticuloDM vt = p;
                    articulo articulo = this.articuloBusiness.GetArticleByBarCode(p.CodBarras);
                    vt.Utilidad = articulo.utilidad;
                    vt.PrecioCompra = articulo.precio_compra;
                    
                    return vt;
                }).ToList();
                return ventaDM;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }

        }
        public VentaDM GetVentaByIdVenta(Guid IdVenta)
        {
            try
            {
                Repository.PosCaja.venta venta = this.repository.GetIncludeAll(p => p.id_venta == IdVenta, "venta_articulo").FirstOrDefault();
                if (venta == null)
                {
                    throw new BusinessException(DsiCodeConst.RESULT_WITHOUT_DATA_ID, DsiCodeConst.RESULT_WITHOUT_DATA);
                }
                return new VentaDM
                {
                    IdPos = venta.id_pos,
                    IdVenta = venta.id_venta,
                    Vendedor = venta.vendedor,
                    Folio = venta.folio,
                    FechaVenta = venta.fecha_venta,
                    TotalVendido = venta.total_vendido,
                    PagoEfectivo = venta.pago_efectivo,
                    PagoVales = venta.pago_vales,
                    PagoTarCredito = venta.pago_tc,
                    Supervisor = venta.supervisor,
                    Upload = venta.upload,
                    NumRegistros = venta.num_registros,
                    PagoTarDebito = venta.pago_td,
                    PagoSpei = venta.pago_spei,
                    VentaArticulos = venta.venta_articulo is null ? null : venta.venta_articulo.Select(va => new VentaArticuloDM()
                    {
                        IdPos = va.id_pos,
                        IdVenta = va.id_venta,
                        NumArticulo = va.no_articulo,
                        CodBarras = va.cod_barras,
                        UserCodeBascula = va.user_code_bascula.Value,
                        Cantidad = va.cantidad,
                        ArticuloOfertado = va.articulo_ofertado,
                        PrecioRegular = va.precio_regular,
                        CambioPrecio = va.cambio_precio,
                        Iva = va.iva,
                        PrecioVenta = va.precio_vta,
                        PorcDescuento = va.porcent_desc,
                        CantDevuelta = va.cant_devuelta,
                        UserName = va.user_name,
                        IdPromo = va.id_promo.Value,
                        NoPromoAplicado = va.no_promo_aplicado,
                        Ieps = va.ieps.Value
                    }).ToList(),


                };
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }

        public List<venta> GetVentasByDates(DateTime start, DateTime end)
        {
            try
            {
                List<venta> ventas = this.repository.GetIncludeAll(v => v.fecha_venta >= start && v.fecha_venta <= end, "venta_articulo").ToList();

                if (!ventas.Any())
                {
                    throw new BusinessException("PV-VENTAS-001", string.Format("No se encontrarón ventas para el rango de fechas informadas {0} - {1}", start, end));
                }

                return ventas;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }
    }
}