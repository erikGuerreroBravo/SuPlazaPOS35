using DsiCodeTech.Common.DataAccess.Infraestructure.Contract;
using DsiCodeTech.Repository;
using DsiCodeTech.Repository.Infraestructure;
using System.Collections.Generic;
using System;
using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodeTech.Common.Constant;
using DsiCodeTech.Common.Exception;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using DsiCodeTech.Repository.PosCaja;
using DsiCodeTech.Business.Interface;

namespace DsiCodeTech.Business
{
    public class VentaCanceladaBusiness: IVentaCanceladaBusiness
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly VentaCanceladaRepository repository;
        private readonly IArticuloBusiness articuloBusiness;
        public VentaCanceladaBusiness(IUnitOfWork _unitOfWork)
        {
            unitOfWork = _unitOfWork;
            repository = new VentaCanceladaRepository(unitOfWork);
        }

        public VentaCanceladaBusiness()
        {
            unitOfWork = new UnitOfWork();
            repository = new VentaCanceladaRepository(unitOfWork);
            articuloBusiness = new ArticuloBusiness(unitOfWork);
        }
        public VentaCanceladaDM GetCancelSaleByIdCancelSale(Guid idVentaCancelada)
        {
            try
            {
                List<VentaCanceladaArticulo> articulos = new List<VentaCanceladaArticulo>();
                venta_cancelada vtCancelada = repository.GetIncludeAll(v => v.id_venta_cancel.Equals(idVentaCancelada), "venta_cancelada_articulo").FirstOrDefault();
                VentaCanceladaDM vtCanceladaDM = new VentaCanceladaDM();
                vtCanceladaDM.IdPos = vtCancelada.id_pos;
                vtCanceladaDM.IdVentaCancel = vtCancelada.id_venta_cancel;
                vtCanceladaDM.Vendedor = vtCancelada.vendedor;
                vtCanceladaDM.Fecha = vtCancelada.fecha != null ? vtCancelada.fecha : DateTime.Now;
                vtCanceladaDM.TotalVendido = vtCancelada.total_vendido;
                vtCanceladaDM.PagoEfectivo = vtCancelada.pago_efectivo;
                vtCanceladaDM.PagoVales = vtCancelada.pago_vales;
                vtCanceladaDM.PagoTarCredito = vtCancelada.pago_tc;
                vtCanceladaDM.Status = vtCancelada.status;
                vtCanceladaDM.Supervisor = vtCancelada.supervisor;
                vtCanceladaDM.Upload =vtCancelada.upload;
                vtCanceladaDM.PagoTarDebito = vtCancelada.pago_td;
                vtCanceladaDM.PagoSpei = vtCancelada.pago_spei;
                foreach (var vt in vtCancelada.venta_cancelada_articulo)
                {
                    articulo articulo=  articuloBusiness.GetArticleByBarCode(vt.cod_barras);
                    VentaCanceladaArticulo ventaCancelada = new VentaCanceladaArticulo();
                    ventaCancelada.IdPos = vt.id_pos;
                    ventaCancelada.IdVentaCancel = vt.id_venta_cancel;
                    ventaCancelada.NumArticulo = vt.no_articulo;
                    ventaCancelada.CodBarras = vt.cod_barras;
                    ventaCancelada.Cantidad = vt.cantidad;
                    ventaCancelada.ArticuloOfertado = vt.articulo_ofertado;
                    ventaCancelada.PrecioRegular = vt.precio_regular;
                    ventaCancelada.CambioPrecio = vt.cambio_precio;
                    ventaCancelada.PrecioVenta = vt.precio_vta;
                    ventaCancelada.PorcDescuento  = vt.porcent_desc;
                    ventaCancelada.UserName = vt.user_name;
                    ventaCancelada.Ieps = vt.ieps != null ?  vt.ieps.Value : 0.0m;
                    ventaCancelada.Iva = vt.iva != null ? vt.iva.Value : 0.0m;
                    ventaCancelada.PrecioCompra = articulo.precio_compra;
                    ventaCancelada.Utilidad = articulo.utilidad;
                    articulos.Add(ventaCancelada);
                }
                vtCanceladaDM.Articulos = articulos;
                return vtCanceladaDM;
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }
     


        /// <summary>
        /// Este metodo se encarga de actualizar el campo upload de la entidad venta_cancelada 
        /// despues de enviarla a rabbitMQ
        /// </summary>
        /// <param name="IdVenta">el identificador de la venta cancelada</param>
        /// <exception cref="BusinessException">excepcion en caso de no estar disponible el contexto</exception>
        public void UpdateUploadField(Guid IdVentaCancelada)
        {
            try
            {
                venta_cancelada venta_cancelada = this.repository.SingleOrDefault(v => v.id_venta_cancel == IdVentaCancelada);
                venta_cancelada.upload = true;
                this.repository.Update(venta_cancelada);
            }
            catch (Exception ex) when (ex is DataException || ex is SqlException)
            {

                throw new BusinessException(DsiCodeConst.RESULT_WITHEXCPETION_ID, DsiCodeConst.RESULT_WITHEXCPETION, ex);
            }
        }



    }
}
