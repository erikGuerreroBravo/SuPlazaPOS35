using System;
using System.Collections.Generic;
using System.Data.Linq.SqlClient;
using System.Linq;
using SuPlazaPOS35.DAO;
using SuPlazaPOS35.model;
using DsiCodeTech.Common.Util;
using DsiCodetech.RabbitMQ.BusRabbit;
using DsiCodetech.RabbitMQ.EventQueue;
using DsiCodetech.RabbitMQ.Implement;
using System.Security.Cryptography;
using DsiCodeTech.Common.DataAccess.Domain;

namespace SuPlazaPOS35.controller
{
    public class POS
    {
        private readonly SuPlazaPosUtil posUtil = new SuPlazaPosUtil();

        public static venta_cancelada SuspendedSale;

        private List<SuPlazaPOS35.domain.venta_articulo> saleItems;

        public static bool SaleRecovery { get; set; }

        public static SuPlazaPOS35.domain.usuario user { get; set; }

        public static SuPlazaPOS35.domain.usuario_permiso supervisor { get; set; }

        public static SuPlazaPOS35.domain.pos_settings caja { get; set; }

        public static SuPlazaPOS35.domain.venta sale { get; set; }

        public static SuPlazaPOS35.domain.venta_devolucion devolucion { get; set; }

        public static decimal tp_efectivo { get; set; }


        public static decimal tp_cheque { get; set; }

        public static decimal tp_debitoCard { get; set; }

        public static decimal tp_spei { get; set; }
        
        public static decimal tp_vales { get; set; }

        public static decimal tp_creditCard { get; set; }

        public static decimal totalVenta { get; set; }

        private SuPlazaPOS35.domain.venta_articulo saleItem { get; set; }

        private short noItemSale { get; set; }

        public int articulosVendidos { get; set; }

        public decimal subTotal { get; set; }

        public decimal descuento { get; set; }

        public decimal iva { get; set; }

        public decimal ieps { get; set; }

        public decimal impuestos { get; set; }

        public decimal total { get; set; }

        public bool deleteItemLast { get; set; }

        public bool changeItemPrice { get; set; }

        public POS()
        {
            newSale();
            deleteItemLast = false;
            changeItemPrice = true;
        }

        public POS(SuPlazaPOS35.domain.pos_settings conf)
        {
            saveConfig(conf);
        }

        public void newSale()
        {
            saleItems = new List<SuPlazaPOS35.domain.venta_articulo>();
            noItemSale = 1;
        }

        public void setSuspendedSales()
        {
            saleCancelOrSuspend("suspendida");
        }

        public SuPlazaPOS35.domain.venta_articulo setItemSale(string codeInput, int count)
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            SuPlazaPOS35.domain.articulo articulo = dataClassesPOSDataContext.articulo.FirstOrDefault(a => a.cod_barras.Equals(codeInput) || a.cod_interno.Equals(codeInput));
            if (articulo != null && articulo.kit)
            {
                DateTime? kit_fecha_ini = articulo.kit_fecha_ini;
                DateTime now = DateTime.Now;
                if (!kit_fecha_ini.HasValue || !(kit_fecha_ini.GetValueOrDefault() < now) || !(articulo.kit_fecha_fin > DateTime.Now))
                {
                    throw new Exception("KIT");
                }
            }
            if (articulo == null && codeInput.Length > 13)
            {
                articulo = dataClassesPOSDataContext.articulo.FirstOrDefault(a => a.cod_barras.Equals(codeInput.Substring(2, 12)));
            }
            else if (articulo == null && codeInput.Length > 12)
            {
                articulo = dataClassesPOSDataContext.articulo.FirstOrDefault(a => a.cod_barras.Equals(codeInput.Substring(1, 12)));
            }
            if (articulo == null)
            {
                if (codeInput.Length <= 12 || (!codeInput.Substring(0, 2).Equals("28") && !codeInput.Substring(0, 2).Equals("26")))
                {
                    throw new Exception("El articulo no existe");
                }
                if (!codeInput.Substring(0, 2).Equals("28") && !codeInput.Substring(0, 2).Equals("26"))
                {
                    throw new Exception("El articulo no existe");
                }
                string s = codeInput.Substring(7, 5);
                string codPesable = codeInput.Substring(0, 7);
                articulo = dataClassesPOSDataContext.articulo.FirstOrDefault(a => SqlMethods.Like(a.cod_barras, $"{codPesable}%"));
                if (articulo == null)
                {
                    if (count != 0)
                    {
                        return setItemSale($"0{codeInput}", --count);
                    }
                    throw new Exception("El artículo no existe");
                }
                saleItem = new SuPlazaPOS35.domain.venta_articulo();
                saleItem.cantidad = ((decimal.Parse(s) != 0.0m) ? (decimal.Parse(s) / 1000m) : 1.0m);
            }
            else
            {
                saleItem = new SuPlazaPOS35.domain.venta_articulo();
                saleItem.cantidad = 1m;
            }

            saleItem.no_articulo = noItemSale++;
            saleItem.articulo = articulo;
            saleItem.unidad_medida = articulo.unidad_medida.descripcion;
            saleItem.precio_regular = articulo.precio_venta;
            saleItem.precio_vta = articulo.precio_venta;
            saleItem = posUtil.GetOffer(saleItem);
            saleItems.Add(saleItem);
            changeItemPrice = true;
            calculate();
            return saleItem;
        }

        public void setItemSaleRecovery(venta_cancelada_articulo itemRecovery)
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            SuPlazaPOS35.domain.vw_oferta vw_oferta = dataClassesPOSDataContext.vw_ofertas.FirstOrDefault(o => o.cod_barras == itemRecovery.cod_barras);
            saleItem = new SuPlazaPOS35.domain.venta_articulo();
            saleItem.no_articulo = noItemSale++;
            saleItem.articulo = dataClassesPOSDataContext.articulo.FirstOrDefault(a => a.cod_barras.Equals(itemRecovery.cod_barras));
            saleItem.unidad_medida = itemRecovery.unidad_medida;
            saleItem.cantidad = itemRecovery.cantidad;
            saleItem.articulo_ofertado = vw_oferta != null;
            saleItem.precio_regular = itemRecovery.precio_regular;
            saleItem.cambio_precio = itemRecovery.cambio_precio;
            saleItem.precio_vta = itemRecovery.precio_vta;
            saleItem.porcent_desc = itemRecovery.porcent_desc;
            saleItems.Add(saleItem);
        }

        public static SuPlazaPOS35.domain.articulo findItemByCode(string code)
        {
            return new SuPlazaPOS35.domain.DataClassesPOSDataContext().articulo.FirstOrDefault(a => a.cod_barras == code);
        }

        public static SuPlazaPOS35.domain.venta saleThere(int folio)
        {
            SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            SuPlazaPOS35.domain.venta sale = dataClassesPOSDataContext.venta.FirstOrDefault((SuPlazaPOS35.domain.venta v) => v.folio == (long)folio);
            if (sale != null)
            {
                SuPlazaPOS35.domain.factura_venta factura_venta = dataClassesPOSDataContext.factura_venta.FirstOrDefault(f => f.id_pos == sale.id_pos && f.id_venta == sale.id_venta);
                if (factura_venta != null)
                {
                    throw new Exception("La venta ya está facturada");
                }
                List<SuPlazaPOS35.domain.venta_articulo> list = sale.venta_articulo.Where((SuPlazaPOS35.domain.venta_articulo va) => va.cantidad <= va.cant_devuelta).ToList();
                foreach (SuPlazaPOS35.domain.venta_articulo item in list)
                {
                    sale.venta_articulo.Remove(item);
                }
            }
            return sale;
        }

        #region  Obtener Los Items de la Venta
        public List<SuPlazaPOS35.domain.venta_articulo> getItemsSales()
        {
            return saleItems;
        }
        #endregion
        public SuPlazaPOS35.domain.venta_articulo getCopyItem(int index)
        {
            return (SuPlazaPOS35.domain.venta_articulo)saleItems[index].Clone();
        }

        public bool saleStarted()
        {
            return saleItems.Count() > 0;
        }

        public bool validateUser(string userName, string password)
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            if (dataClassesPOSDataContext.usuario.FirstOrDefault() != null)
            {
                user = dataClassesPOSDataContext.usuario.FirstOrDefault(u => u.user_name.Equals(userName) && u.password.Equals(password) && u.usuario_permiso.FirstOrDefault(up => up.id_permiso.Equals("pos_caja")) != null);
                return user != null;
            }
            return userName.Equals("admin") && password.Equals("admin");
        }

        public static bool SupervisorAuthorized(string password, string permission)
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            if (dataClassesPOSDataContext.usuario_permiso.FirstOrDefault(up => up.id_permiso == permission) != null)
            {
                return dataClassesPOSDataContext.usuario_permiso.FirstOrDefault(up => up.usuario.password == password && up.id_permiso == permission) != null;
            }
            return password.Equals("admin");
        }

        public static SuPlazaPOS35.domain.usuario_permiso getSupervisorAuthorized(string password, string permission)
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            return dataClassesPOSDataContext.usuario_permiso.FirstOrDefault(up => up.usuario.password == password && up.id_permiso == permission);
        }

        public bool isRegisterPOS()
        {
            caja = new SuPlazaPOS35.domain.DataClassesPOSDataContext().pos_settings.FirstOrDefault();
            return caja != null;
        }

        public int existsSuspendedSales()
        {
            return new SuspencionDAO().existsSaleSuspended();
        }

        public void saveConfig(SuPlazaPOS35.domain.pos_settings kja)
        {
            SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            if (caja == null)
            {
                dataClassesPOSDataContext.pos_settings.InsertOnSubmit(kja);
            }
            else
            {
                SuPlazaPOS35.domain.pos_settings pos_settings = dataClassesPOSDataContext.pos_settings.FirstOrDefault();
                pos_settings.id_pos = kja.id_pos;
                pos_settings.tipo_pago = kja.tipo_pago;
                pos_settings.pos_log_enable = kja.pos_log_enable;
                pos_settings.tck_concentrado = kja.tck_concentrado;
                pos_settings.inventario_online = kja.inventario_online;
                pos_settings.pos_dsp_name = kja.pos_dsp_name;
                pos_settings.pos_dsp_enable = kja.pos_dsp_enable;
                pos_settings.pos_ptr_name = kja.pos_ptr_name;
                pos_settings.pos_ptr_enable = kja.pos_ptr_enable;
                pos_settings.pos_csh_name = kja.pos_csh_name;
                pos_settings.pos_csh_enable = kja.pos_csh_enable;
                pos_settings.pos_scn_name = kja.pos_scn_name;
                pos_settings.pos_scn_enable = kja.pos_scn_enable;
                pos_settings.pos_folio = kja.pos_folio;
                pos_settings.win_ptr_name = kja.win_ptr_name;
                pos_settings.win_ptr_enable = kja.win_ptr_enable;
                pos_settings.com_name = kja.com_name;
                pos_settings.com_rate = kja.com_rate;
                pos_settings.com_bits = kja.com_bits;
                pos_settings.com_parity = kja.com_parity;
                pos_settings.com_stop = kja.com_stop;
                pos_settings.com_enable = kja.com_enable;
            }
            dataClassesPOSDataContext.SubmitChanges();
        }

        #region Metodo para Calcular Descuentos
        public void calculate()
        {
            List<SuPlazaPOS35.domain.venta_articulo> itemsSales = getItemsSales();
            articulosVendidos = 0;
            subTotal = 0.0m;
            descuento = 0.0m;
            impuestos = 0.0m;
            iva = 0.0m;
            ieps = 0.0m;
            total = 0.0m;
            foreach (SuPlazaPOS35.domain.venta_articulo item in itemsSales)
            {
                articulosVendidos += ((item.unidad_medida.CompareTo("Kg") == 0) ? ((item.cantidad > 0m) ? 1 : (-1)) : ((item.unidad_medida.CompareTo("Gms") == 0) ? ((item.cantidad > 0m) ? 1 : (-1)) : ((int)item.cantidad)));
                decimal itemSub = item.subTotal();
                decimal itemIeps = item.getIeps();
                decimal itemIva = item.getIVA();
                decimal itemTotal = item.total();
                decimal itemDescuento = item.descuento();

                subTotal += item.hasIva() && item.hasIeps() ? DsiCodeUtil.Sum(itemSub, itemIeps) : itemSub;
                descuento += itemDescuento;
                iva += itemIva;
                ieps += item.hasIva() && item.hasIeps() ? 0 : itemIeps;
                total += itemTotal;
            }
            impuestos = Math.Round(DsiCodeUtil.Sum(iva, ieps), 2);
            descuento = Math.Round(descuento, 2);
            total = Math.Round(total, 2);
            subTotal = DsiCodeUtil.Sum(impuestos, subTotal) <= total ? Math.Round(subTotal, 2) : DsiCodeUtil.Round2Positions(subTotal);
        }

        #endregion
        public void calculateDevolution()
        {
            articulosVendidos = 0;
            subTotal = 0.0m;
            descuento = 0.0m;
            impuestos = 0.0m;
            iva = 0.0m;
            ieps = 0.0m;
            total = 0.0m;
            foreach (SuPlazaPOS35.domain.venta_articulo item in sale.venta_articulo)
            {
                articulosVendidos += ((item.articulo.unidad_medida.descripcion.CompareTo("Kg") == 0) ? ((item.cantidad_a_devolver > 0m) ? 1 : 0) : ((item.articulo.unidad_medida.descripcion.CompareTo("Gms") == 0) ? ((item.cantidad_a_devolver > 0m) ? 1 : 0) : ((int)item.cantidad_a_devolver)));
                decimal itemSub = item.subTotalDevolucion();
                decimal itemIeps = item.getIepsDevolucion();
                decimal itemIva = item.getIVADevolucion();
                decimal itemTotal = item.totalDevolucion();
                decimal itemDescuento = item.descuentoDevolucion();

                subTotal += item.hasIva() && item.hasIeps() ? DsiCodeUtil.Sum(itemSub, itemIeps) : itemSub;
                descuento += itemDescuento;
                iva += itemIva;
                ieps += item.hasIva() && item.hasIeps() ? 0 : itemIeps;
                total += itemTotal;
            }
            impuestos = Math.Round(DsiCodeUtil.Sum(iva, ieps), 2);
            descuento = Math.Round(descuento, 2);
            total = Math.Round(total, 2);
            subTotal = DsiCodeUtil.Sum(impuestos, subTotal) <= total ? Math.Round(subTotal, 2) : DsiCodeUtil.Round2Positions(subTotal);
        }

        public SuPlazaPOS35.domain.venta_articulo setPrice(int index, decimal newPrice)
        {
            decimal? valor_num = supervisor.valor_num;
            if (valor_num.GetValueOrDefault() == 1m && valor_num.HasValue)
            {
                saleItems[index].user_name = supervisor.user_name;
                saleItems[index].cambio_precio = saleItems[index].precio_regular != newPrice;
                saleItems[index].precio_vta = newPrice;
            }
            else
            {
                decimal? valor_num2 = supervisor.valor_num;
                if (!(valor_num2.GetValueOrDefault() == 0m) || !valor_num2.HasValue)
                {
                    throw new Exception("El precio no se puede aplicar por falta de permisos.\nConsulte al administrador del sistema");
                }
                if (!(saleItems[index].articulo.precio_compra < newPrice))
                {
                    throw new Exception("El precio es superior al costo");
                }
                saleItems[index].user_name = supervisor.user_name;
                saleItems[index].cambio_precio = saleItems[index].precio_regular != newPrice;
                saleItems[index].precio_vta = newPrice;
            }
            return saleItems[index];
        }

        public SuPlazaPOS35.domain.venta_articulo setQuality(int index, decimal quality)
        {
            saleItems[index].cantidad = quality;
            return saleItems[index];
        }

        public SuPlazaPOS35.domain.venta_articulo deleteItem(int index, bool isLast)
        {
            if (supervisor != null || isLast)
            {
                saleItems[index].eliminar = true;
                SuPlazaPOS35.domain.venta_articulo copyItem = getCopyItem(index);
                copyItem.cantidad *= -1m;
                saleItems.Add(copyItem);
                return copyItem;
            }
            throw new Exception("El artículo no pudo ser eliminado.");
        }

        public void deleteSaleSuspended(bool delete)
        {
            if (delete)
            {
                new SuspencionDAO().deleteVentaSuspendidaTemporal(SuspendedSale.id_venta_cancel);
            }
            SuspendedSale = null;
            SaleRecovery = false;
            GC.Collect();
        }


        #region Porcentaje de Descuento
        public SuPlazaPOS35.domain.venta_articulo setPorcentLine(int index, decimal porcent)
        {
            if (!supervisor.valor_num.HasValue)
                if (supervisor != null)
                {
                    throw new Exception("No puede aplicar descuento en línea, consulte al administrador del sistema.");
                }
            SuPlazaPOS35.domain.venta_articulo venta_articulo = saleItems[index];
            decimal? valor_num = supervisor.valor_num;
            venta_articulo.porcent_desc = ((porcent <= valor_num.GetValueOrDefault() && valor_num.HasValue) ? porcent : saleItems[index].porcent_desc);
            saleItems[index].user_name = ((supervisor != null) ? supervisor.user_name : null);
            return saleItems[index];
        }
        #endregion


        public SuPlazaPOS35.domain.venta_articulo setPorcentGlobal(int index, decimal porcent)
        {
            if (supervisor != null)
            {
                if (!supervisor.valor_num.HasValue)
                {
                    throw new Exception("No puede aplicar descuento global, consulte al administrador del sistema.");
                }
                SuPlazaPOS35.domain.venta_articulo venta_articulo = saleItems[index];
                decimal? valor_num = supervisor.valor_num;
                venta_articulo.porcent_desc = ((porcent <= valor_num.GetValueOrDefault() && valor_num.HasValue) ? porcent : saleItems[index].porcent_desc);
                saleItems[index].user_name = ((supervisor != null) ? supervisor.user_name : null);
            }
            return saleItems[index];
        }

        #region Nuevo Metodo GetCambio
        public static decimal getCambio(decimal total)
        {
            return tp_efectivo + tp_spei + tp_vales + tp_creditCard + tp_debitoCard - total;
        }
        #endregion

        #region Venta Final
        /// <summary>
        /// Este metodo se encarga de realizar la venta final del punto de venta
        /// </summary>
        /// <returns></returns>
        public SuPlazaPOS35.domain.venta saleOut()
        {
            SuPlazaPOS35.domain.venta venta = new SuPlazaPOS35.domain.venta();
            venta.id_venta = (SaleRecovery ? SuspendedSale.id_venta_cancel : Guid.NewGuid());
            venta.folio = getLastFolio();
            venta.id_pos = caja.id_pos;
            venta.vendedor = user.user_name;
            venta.pago_efectivo = tp_efectivo;
            venta.pago_td = tp_debitoCard;
            venta.pago_spei = tp_spei;
            venta.pago_vales = tp_vales;
            venta.pago_tc = tp_creditCard;
            venta.fecha_venta = DateTime.Now;

            venta.total_vendido = totalVenta;
            venta.subtotal = subTotal;
            venta.iva_desglosado = iva;
            venta.ieps_desglosado = ieps;
            venta.impuestos = impuestos;
            venta.descuento = descuento;
            venta.upload = false;

            int num = 1;
            List<SuPlazaPOS35.domain.venta_articulo> itemsSales = getItemsSales();
            foreach (SuPlazaPOS35.domain.venta_articulo item in itemsSales)
            {
                if (!item.eliminar)
                {
                    SuPlazaPOS35.domain.venta_articulo venta_articulo = new SuPlazaPOS35.domain.venta_articulo();
                    venta_articulo.venta = venta;
                    venta_articulo.no_articulo = num++;
                    venta_articulo.cod_barras = item.articulo.cod_barras;
                    venta_articulo.cantidad = item.cantidad;
                    venta_articulo.iva = (item.articulo.impuestos[0].iva / 100);
                    venta_articulo.ieps = (item.articulo.impuestos[0].ieps / 100);
                    venta_articulo.articulo_ofertado = item.articulo_ofertado;
                    venta_articulo.precio_regular = item.precio_regular;
                    venta_articulo.cambio_precio = item.cambio_precio;
                    venta_articulo.precio_vta = item.precio_vta;
                    venta_articulo.porcent_desc = item.porcent_desc;
                    venta_articulo.user_name = item.user_name;
                    venta_articulo.precio_compra = item.articulo.precio_compra;
                    venta_articulo.utilidad = item.articulo.utilidad;
                    venta.venta_articulo.Add(venta_articulo);
                }
            }

            new VentaDAO().SaleOut(venta);
            if (SaleRecovery)
            {
                new SuspencionDAO().changeSuspendedStatus(SuspendedSale.id_venta_cancel, SuspencionDAO.SuspendedStatus.Sale);
                SuspendedSale = null;
                SaleRecovery = false;
            }
            itemsSales.Clear();
            itemsSales = null;
            GC.Collect();
            return venta;
        }

        #endregion

        #region Obtencion del Ultimo Folio

        /// <summary>
        /// Este metodo se encarga consultar el ultimo folio entrante de la venta
        /// </summary>
        /// <returns>retorna un valor del tipo long que es el ultimo folio de la venta</returns>
        private long getLastFolio()
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            return (dataClassesPOSDataContext.venta.FirstOrDefault() != null) ? (dataClassesPOSDataContext.venta.Max((SuPlazaPOS35.domain.venta v) => v.folio) + 1) : dataClassesPOSDataContext.pos_settings.FirstOrDefault().pos_folio;
        }
        #endregion

        #region Cancelar Venta Modificado
        public Guid saleCancelOrSuspend(string statusSale)
        {
            Guid idVentaCancelada = Guid.Empty;
            venta_cancelada venta_cancelada = new venta_cancelada();
            venta_cancelada.id_pos = caja.id_pos;

            venta_cancelada.id_venta_cancel = (SaleRecovery ? SuspendedSale.id_venta_cancel : Guid.NewGuid());
            idVentaCancelada = venta_cancelada.id_venta_cancel;

            venta_cancelada.vendedor = user.user_name;
            venta_cancelada.pago_efectivo = tp_efectivo;
            /************se elimina el apgo con cheque *************************/
            venta_cancelada.pago_cheque = tp_cheque;
            /**************** se agrega tarjeta de debito y spei************************/
            venta_cancelada.pago_td = tp_debitoCard;
            venta_cancelada.pago_spei = tp_spei;
            /*************** terminan cambios o modifcaciones*************************/

            venta_cancelada.pago_vales = tp_vales;
            venta_cancelada.pago_tc = tp_creditCard;
            venta_cancelada.status = statusSale;
            venta_cancelada.fecha = DateTime.Now;
            venta_cancelada.total_vendido = totalVenta;
            venta_cancelada.venta_cancelada_articulo = new List<venta_cancelada_articulo>();
            if (statusSale.CompareTo("cancelada") == 0 || statusSale.CompareTo("suspecancel") == 0)
            {
                venta_cancelada.supervisor = supervisor.user_name;
            }
            int num = 1;
            List<SuPlazaPOS35.domain.venta_articulo> itemsSales = getItemsSales();
            foreach (SuPlazaPOS35.domain.venta_articulo item in itemsSales)
            {
                if (!item.eliminar)
                {
                    venta_cancelada_articulo venta_cancelada_articulo = new venta_cancelada_articulo();
                    venta_cancelada_articulo.id_pos = venta_cancelada.id_pos;
                    venta_cancelada_articulo.id_venta_cancel = venta_cancelada.id_venta_cancel;
                    venta_cancelada_articulo.no_articulo = num++;
                    venta_cancelada_articulo.cod_barras = item.articulo.cod_barras;
                    venta_cancelada_articulo.cantidad = item.cantidad;
                    venta_cancelada_articulo.articulo_ofertado = item.articulo_ofertado;
                    venta_cancelada_articulo.precio_regular = item.precio_regular;
                    venta_cancelada_articulo.cambio_precio = item.cambio_precio;
                    venta_cancelada_articulo.precio_vta = item.precio_vta;
                    venta_cancelada_articulo.porcent_desc = item.porcent_desc;
                    venta_cancelada.venta_cancelada_articulo.Add(venta_cancelada_articulo);
                }
            }
            new SuspencionDAO().setSaleSuspended(venta_cancelada);
            SaleRecovery = false;
            venta_cancelada = null;
            itemsSales.Clear();
            itemsSales = null;
            GC.Collect();
            //modifcamos el valor de retorno estava en void
            return idVentaCancelada; // venta_cancelada.id_venta_cancel;

        }
        #endregion



        public static SuPlazaPOS35.model.empresa getEmpresa()
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            SuPlazaPOS35.domain.empresa empresa = dataClassesPOSDataContext.empresa.FirstOrDefault();
            if (empresa != null)
            {
                SuPlazaPOS35.model.empresa empresa2 = new SuPlazaPOS35.model.empresa();
                empresa2.razon_social = empresa.razon_social;
                empresa2.calle = empresa.calle;
                empresa2.colonia = empresa.colonia;
                empresa2.municipio = empresa.municipio;
                empresa2.estado = empresa.estado;
                empresa2.codigo_postal = empresa.codigo_postal;
                empresa2.rfc = empresa.rfc;
                return empresa2;
            }
            return null;
        }

        public SuPlazaPOS35.domain.venta_devolucion setDevolution()
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            SuPlazaPOS35.domain.venta_devolucion venta_devolucion = new SuPlazaPOS35.domain.venta_devolucion();
            venta_devolucion.id_devolucion = Guid.NewGuid();
            venta_devolucion.folio = ((dataClassesPOSDataContext.venta_devolucion.FirstOrDefault() != null) ? dataClassesPOSDataContext.venta_devolucion.Max((SuPlazaPOS35.domain.venta_devolucion v) => v.folio) : 0) + 1;
            venta_devolucion.venta = dataClassesPOSDataContext.venta.FirstOrDefault((SuPlazaPOS35.domain.venta v) => v.id_pos == sale.id_pos && v.id_venta == sale.id_venta);
            venta_devolucion.fecha_dev = DateTime.Now;
            venta_devolucion.vendedor = user.user_name;
            venta_devolucion.supervisor = supervisor.user_name;
            venta_devolucion.cant_dev = total;
            venta_devolucion.descuento = descuento;
            venta_devolucion.impuestos = impuestos;
            venta_devolucion.upload = false;
            dataClassesPOSDataContext.venta_devolucion.InsertOnSubmit(venta_devolucion);
            dataClassesPOSDataContext.SubmitChanges();
            foreach (SuPlazaPOS35.domain.venta_articulo va in sale.venta_articulo)
            {
                if (va.cantidad_a_devolver > 0m)
                {
                    SuPlazaPOS35.domain.venta_articulo venta_articulo =
                        dataClassesPOSDataContext.venta_articulo
                        .SingleOrDefault((SuPlazaPOS35.domain.venta_articulo sva) => sva.id_pos == sale.id_pos && sva.id_venta == sale.id_venta && sva.no_articulo == va.no_articulo);
                    if (venta_articulo != null)
                    {
                        venta_articulo.cant_devuelta += va.cantidad_a_devolver;
                    }
                    SuPlazaPOS35.domain.venta_devolucion_articulo venta_devolucion_articulo = new SuPlazaPOS35.domain.venta_devolucion_articulo();
                    venta_devolucion_articulo.venta_devolucion = venta_devolucion;
                    venta_devolucion_articulo.cantidad = va.cantidad_a_devolver;
                    venta_devolucion_articulo.cod_barras = va.cod_barras;
                    venta_devolucion_articulo.no_articulo = va.no_articulo;
                    dataClassesPOSDataContext.venta_devolucion_articulo.InsertOnSubmit(venta_devolucion_articulo);
                    dataClassesPOSDataContext.SubmitChanges();
                }
            }
            return venta_devolucion;
        }

        public static bool IsConcentredTicket()
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            return dataClassesPOSDataContext.pos_settings.FirstOrDefault().tck_concentrado;
        }

        public static bool IsLogoEnable()
        {
            using SuPlazaPOS35.domain.DataClassesPOSDataContext dataClassesPOSDataContext = new SuPlazaPOS35.domain.DataClassesPOSDataContext();
            return dataClassesPOSDataContext.pos_settings.FirstOrDefault().pos_log_enable;
        }
    }
}
