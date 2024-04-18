using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Microsoft.PointOfService;
using SuPlazaPOS35.controller;
using SuPlazaPOS35.DAO;
using SuPlazaPOS35.domain;
using SuPlazaPOS35.model;

using DsiCodeTech.Business.Interface;
using DsiCodeTech.Business;
using DsiCodeTech.Common.DataAccess.Domain;
using DsiCodetech.RabbitMQ.BusRabbit;
using DsiCodetech.RabbitMQ.Implement;
using DsiCodetech.RabbitMQ.EventQueue;
using System.Collections.Generic;
using DsiCodeTech.Common.Util;

using NLog;

using Newtonsoft;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace SuPlazaPOS35.view
{
    public class SuPlazaPOS : Form
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public enum StatusItem
        {
            Vendido,
            Eliminado,
            ConDescuento,
            PrecioModificado
        }

        public enum modeOperation
        {
            Sale,
            Devolution,
            Verify
        }

        private FunctionsPOS functions;

        private POS sale;

        private DevicesOPOS devOpos;

        private Thread threadDownload;

        private Thread threadPOS;

        private corte corte;

        public static modeOperation statusModeOperation { get; set; }

        public string msgDownload { get; set; }

        public static string codigoBarras { get; set; }

        public static long ticketNumber { get; set; }

        public static decimal changeQuality { get; set; }

        public static decimal changePrice { get; set; }

        public static decimal changePorcent { get; set; }

        public static string statusSync { get; set; }

        public bool cobro { get; set; }

        public bool cancelPOS { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public SuPlazaPOS()
        {
            InitializeComponent();
            try
            {
                POSCaja.getConnectionLocal();
            }
            catch
            {
                MessageBox.Show("Fallo al conectarse a la BD del POS", "Conexión a la BD", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            try
            {
                devOpos = new DevicesOPOS();
                if (devOpos.getScanner() != null)
                {
                    devOpos.getScanner().DataEvent += scannerOnEventInfo;
                    devOpos.getScanner().ErrorEvent += scannerOnErrorEvent;
                }
                if (devOpos.getCashDrawer() != null)
                {
                    devOpos.getCashDrawer().StatusUpdateEvent += cashDrawerOnStatusInfo;
                }
                if (devOpos.getPosPrinter() != null)
                {
                    devOpos.printInitalMessage();
                }
                else
                {
                    MessageBox.Show("No se detectó la impresora", "Configuración de OPOS", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }
            catch (Exception e)
            {
                using (DataClassesPOSDataContext dataClassesPOSDataContext = new DataClassesPOSDataContext())
                {
                    pos_settings pos_settings = dataClassesPOSDataContext.pos_settings.FirstOrDefault();
                    pos_settings.pos_csh_enable = false;
                    pos_settings.pos_csh_name = null;
                    pos_settings.pos_dsp_enable = false;
                    pos_settings.pos_dsp_name = null;
                    pos_settings.pos_ptr_enable = false;
                    pos_settings.pos_ptr_name = null;
                    pos_settings.pos_scn_enable = false;
                    pos_settings.pos_scn_name = null;
                    pos_settings.pos_log_enable = false;
                    //dataClassesPOSDataContext.SubmitChanges();
                }
                MessageBox.Show("Fallo al cargar los dispositos OPOS, se restauró la configuración y se cerrará el programa. Favor de reconfigurar las opciones: " + e.Message, "Configuración de OPOS", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                Application.Exit();
            }
            try
            {
                codigoBarras = null;
                listProducts.Dock = DockStyle.Fill;
                listDevolutionItems.Visible = false;
                listDevolutionItems.Dock = DockStyle.Fill;
                tlsCaja.Text = "Caja " + POS.caja.id_pos;
                tlsFecha.Text = DateTime.Now.ToLongDateString().ToString();
                tmrHour.Start();
                using DataClassesPOSDataContext dataClassesPOSDataContext2 = new DataClassesPOSDataContext();
                if (dataClassesPOSDataContext2.empleado.FirstOrDefault() != null)
                {
                    tlsUsuario.Text = " Cajer@: " + dataClassesPOSDataContext2.empleado.FirstOrDefault((SuPlazaPOS35.domain.empleado e) => e.usuario.Equals(POS.user)).fullName() + " ";
                }
            }
            catch
            {
                MessageBox.Show("Es probable que la base de datos no se haya sincronizada aún, consulte a su administrador del sistema", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void SuPlazaPOS_Load(object sender, EventArgs e)
        {
            try
            {
                newSale();
                txtCodigoBarras.Select();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void newSale()
        {
            try
            {
                sale = null;
                GC.Collect();
                sale = new POS();
                cancelPOS = true;
                statusModeOperation = modeOperation.Sale;
                POS.sale = null;
                POS.SuspendedSale = null;
                POS.tp_efectivo = 0.0m;
                POS.tp_cheque = 0.0m;
                POS.tp_vales = 0.0m;
                POS.tp_creditCard = 0.0m;
                POS.tp_debitoCard = 0.0m;
                POS.tp_spei = 0.0m;
             
                Invoke(new EventHandler(ResetControls));
                welcomeMessage();

                if (devOpos.getScanner() != null)
                {
                    devOpos.getScanner().DeviceEnabled = true;
                    devOpos.getScanner().DataEventEnabled = true;
                }
                GC.Collect();
            }
            catch
            {
                MessageBox.Show("Incovenientes al cerrar la Caja", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        public void ResetControls(object sender, EventArgs e)
        {
            panelCambio.Visible = false;
            label1.Text = "Su cambio:";
            lblCambio.Text = "0.00";
            listProducts.Items.Clear();
            listProducts.Visible = true;
            listDevolutionItems.Visible = false;
            resetTotales();
            cleanTotales();
            lblTitleNoArticulos.Text = "# Articulos vendidos:";
            lblTitleTotal.Text = "Total a Pagar: $";
            txtCodigoBarras.Enabled = true;
            txtCodigoBarras.Text = "";
            txtCodigoBarras.Focus();
            txtCodigoBarras.SelectAll();
        }

        private void welcomeMessage()
        {
            using DataClassesPOSDataContext dataClassesPOSDataContext = new DataClassesPOSDataContext();
            DevicesOPOS.showMessageDisplay(((DateTime.Now.Hour < 12) ? "Buenos días" : ((DateTime.Now.Hour < 20) ? "Buenas tardes" : "Buenas noches")) + " Atde:", dataClassesPOSDataContext.empleado.FirstOrDefault((SuPlazaPOS35.domain.empleado e) => e.usuario.Equals(POS.user)).shortName());
            

        }

        private void scannerOnEventInfo(object sender, DataEventArgs e)
        {
            Invoke((MethodInvoker)delegate
            {
                try
                {
                    string @string = new ASCIIEncoding().GetString(devOpos.getScanner().ScanDataLabel);
                    switch (statusModeOperation)
                    {
                        case modeOperation.Sale:
                            saleItem(@string);
                            break;
                        case modeOperation.Verify:
                            functions.VerificarArticulo = @string;
                            break;
                        case modeOperation.Devolution:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    logger.Error("Fue imposible procesar la información del scanner: ", ex);
                    MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                finally
                {
                    devOpos.getScanner().ClearInput();
                    devOpos.getScanner().DataEventEnabled = true;
                }
            });
        }

        private void scannerOnErrorEvent(object source, DeviceErrorEventArgs d)
        {
            devOpos.getScanner().DataEventEnabled = true;
        }

        private void cashDrawerOnStatusInfo(object sender, StatusUpdateEventArgs e)
        {
            try
            {
                if (!devOpos.getCashDrawer().DrawerOpened)
                {
                    newSale();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Fue imposible establecer conexión con el CashDrawer: ", ex);
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void MyClock_Tick(object sender, EventArgs e)
        {
            tlsHour.Text = DateTime.Now.ToLongTimeString().ToString();
        }

        private void functionsOfPOS(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    if (statusModeOperation.CompareTo(modeOperation.Sale) == 0)
                    {
                        if (txtCodigoBarras.Text.Trim().Length <= 0)
                        {
                            return;
                        }
                        saleItem(txtCodigoBarras.Text.Trim());
                    }
                    break;
                case Keys.F1:
                    txtCodigoBarras.Select();
                    break;
                case Keys.F2:
                    btnBuscar_Click(sender, e);
                    break;
                case Keys.F3:
                    btnCobrar_Click(sender, e);
                    break;
                case Keys.F4:
                    btnVerificar_Click(sender, e);
                    break;
                case Keys.F5:
                    btnCambiarPrecio_Click(sender, e);
                    break;
                case Keys.F6:
                    btnCantidad_Click(sender, e);
                    break;
                case Keys.F7:
                    btnEliminar_Click(sender, e);
                    break;
                case Keys.F8:
                    btnDescProduct_Click(sender, e);
                    break;
                case Keys.F9:
                    btnCancelarVta_Click(sender, e);
                    break;
                case Keys.F10:
                    btnReimprimir_Click(sender, e);
                    break;
                case Keys.F11:
                    btnSuspender_Click(sender, e);
                    break;
                case Keys.F12:
                    btnRecuperar_Click(sender, e);
                    break;
            }
            showCalculate();
        }

        #region SaleItem
        private void saleItem(string barCode)
        {
            try
            {
                if (devOpos.getCashDrawer() != null && devOpos.getCashDrawer().DrawerOpened)
                {
                    MessageBox.Show("Para continuar con una venta, es necesario que cierre el cajón", "Cajón abierto", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    return;
                }
                SuPlazaPOS35.domain.venta_articulo venta_articulo = sale.setItemSale(barCode, 1);
                if (venta_articulo == null)
                {
                    throw new Exception("Articulo no encontrado");
                }
                DevicesOPOS.showMessageDisplay((venta_articulo.articulo.descripcion_corta.Length > 20) ? venta_articulo.articulo.descripcion_corta.Substring(0, 20) : venta_articulo.articulo.descripcion_corta, "Precio: " + DsiCodeUtil.CurrencyFormat(venta_articulo.precio_vta));
                ListViewItem listViewItem = new ListViewItem(new string[7]
                {
                venta_articulo.cod_barras,
                venta_articulo.articulo.descripcion,
                venta_articulo.articulo.unidad_medida.descripcion,
                DsiCodeUtil.CurrencyFormat(venta_articulo.precio_vta),
                venta_articulo.cantidad.ToString("G"),
                DsiCodeUtil.CurrencyFormat(venta_articulo.descuento()),
                DsiCodeUtil.CurrencyFormat(venta_articulo.total())
                });
                listViewItem.Tag = StatusItem.Vendido;
                listProducts.Items.Add(listViewItem).ForeColor = (venta_articulo.articulo_ofertado ? Color.DarkMagenta : Color.Blue);
                calcularTotales();
                sale.deleteItemLast = false;
            }
            catch (Exception ex)
            {
                try
                {
                    if (ex.Message.Equals("KIT"))
                    {
                        throw new Exception("La fecha del KIT ha expirado.");
                    }
                    FunctionsPOS functionsPOS = new FunctionsPOS(FunctionsPOS.Options.Buscar);
                    functionsPOS.txtFindItem.Text = txtCodigoBarras.Text;
                    functionsPOS.ShowDialog();
                    txtCodigoBarras.SelectAll();
                    if (codigoBarras != null)
                    {
                        txtCodigoBarras.Text = codigoBarras;
                        SendKeys.Send("{ENTER}");
                    }
                }
                catch (Exception ex2)
                {
                    MessageBox.Show(ex2.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }
        #endregion

        #region SaleItemRecovery
        private void saleItemRecovery()
        {
            try
            {
                foreach (SuPlazaPOS35.model.venta_cancelada_articulo item in POS.SuspendedSale.venta_cancelada_articulo)
                {
                    sale.setItemSaleRecovery(item);
                    ListViewItem listViewItem = new ListViewItem(new string[7]
                    {
                    item.cod_barras,
                    item.descripcion,
                    item.unidad_medida,
                    DsiCodeUtil.CurrencyFormat(item.precio_vta),
                    item.cantidad.ToString("G"),
                    DsiCodeUtil.CurrencyFormat(item.descuento()),
                    DsiCodeUtil.CurrencyFormat(item.total())
                    });
                    if (item.cambio_precio)
                    {
                        listViewItem.ForeColor = Color.DarkGreen;
                        listViewItem.Tag = StatusItem.PrecioModificado;
                    }
                    else
                    {
                        listViewItem.Tag = StatusItem.Vendido;
                        listViewItem.ForeColor = (item.porcent_desc != 0.0m) ? Color.DarkGreen : (item.articulo_ofertado ? Color.DarkMagenta : Color.Blue);
                    }
                    listProducts.Items.Add(listViewItem);
                }
                calcularTotales();
                sale.deleteItemLast = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        #endregion

        #region Limpiar Focus
        private void cleanAndFocus()
        {
            txtCodigoBarras.Text = "";
            txtCodigoBarras.Focus();
        }
        #endregion

        #region Evento Buscar
        private void btnBuscar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && new FunctionsPOS(FunctionsPOS.Options.Buscar).ShowDialog() == DialogResult.OK)
                {
                    txtCodigoBarras.Select();
                    if (codigoBarras.Length > 0)
                    {
                        txtCodigoBarras.Text = codigoBarras;


                        SendKeys.Send("{ENTER}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Búsqueda de Artículos", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Cobrar

        private void btnCobrar_Click(object sender, EventArgs e)
        {
            try
            {
                ///aqui estaba comentado
                if (devOpos.getScanner() != null)
                {
                    devOpos.getScanner().DeviceEnabled = false;
                    devOpos.getScanner().DataEventEnabled = false;
                }
                
                if (!panelCambio.Visible && sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && sale.articulosVendidos > 0 && !devOpos.isOpenCashDrawer())
                {
                    string venta = DsiCodeUtil.CurrencyFormat(POS.totalVenta);
                    
                    DevicesOPOS.showMessageDisplay("Total a pagar:", venta);
                    if (new FunctionsPOS(FunctionsPOS.Options.Cobrar).ShowDialog() == DialogResult.OK)
                    {
                        string cambio = DsiCodeUtil.CurrencyFormat(POS.getCambio(decimal.Parse(lblTotal.Text.Replace("$", ""))));
                        panelCambio.Visible = int.Parse(lblNoArticulosVendidos.Text) != 0;
                        label1.Text = "Su cambio:";
                        lblCambio.Text = cambio;
                        DevicesOPOS.showMessageDisplay("Total: " + venta, "Cambio: " + cambio);
                        //esto estaba comentado
                        devOpos.openNowCashDrawer();
                        threadPOS = new Thread(cobrar);
                        threadPOS.Start();
                    }
                    //esto estaba comentado esta linea del scanner
                    else if (devOpos.getScanner() != null)
                    {
                        devOpos.getScanner().DeviceEnabled = true;
                        devOpos.getScanner().DataEventEnabled = true;
                    }
                }
                else if (statusModeOperation.CompareTo(modeOperation.Devolution) == 0 && sale.total > 0m)
                {
                    SuPlazaPOS35.domain.venta_devolucion venta_devolucion = sale.setDevolution();
                    devOpos.printTicketDevolutionOnPosPrinter(venta_devolucion.id_devolucion);
                    newSale();
                    Task work = Task.Run(() =>
                    {
                        try
                        {
                            IRabbitEventBus rabbitEvent = new RabbitEventBus();
                            rabbitEvent.Producer(new VentaDevolucionQueue() { CorrelationId = Guid.NewGuid().ToString(), Body = this.GetVentaDevolucion(venta_devolucion) });
                            IVentaDevolucionBusiness ventaDBusiness = new VentaDevolucionBusiness();
                            ventaDBusiness.UpdateUploadField(venta_devolucion.id_devolucion);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Fue imposible realizar conexión a traves de RabbitMQ, la devolucion se realizo en la caja: ", ex.Message);
                            
                        }                       
                    });
                    work.Wait();
                }
            }
            catch (Exception ex)
            {
                logger.Error("Fue imposible realizar el cobro, se presento la siguiente excepción: ", ex);
                MessageBox.Show(ex.Message, "Cobrar Venta", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            finally
            {
                txtCodigoBarras.SelectAll();
                txtCodigoBarras.Focus();
            }
        }
        #endregion



        #region SetCambio
        public void setCambio(object sender, EventArgs e)
        {
            panelCambio.Visible = int.Parse(lblNoArticulosVendidos.Text) != 0;
            label1.Text = "Su cambio:";
            lblCambio.Text = DsiCodeUtil.CurrencyFormat(POS.getCambio(decimal.Parse(lblTotal.Text.Replace("$", ""))));
        }
        #endregion

        #region Funcion Cobrar
        private void cobrar()
        {
            try
            {
                SuPlazaPOS35.domain.venta venta = sale.saleOut();
                model.venta entidad = new model.venta
                {
                    fecha_venta = venta.fecha_venta,

                    folio = venta.folio,
                    id_pos = venta.id_pos,
                    id_venta = venta.id_venta,
                    pago_spei = venta.pago_spei,
                    pago_tc = venta.pago_tc,
                    pago_efectivo = venta.pago_efectivo,
                    pago_td = venta.pago_td,
                    pago_vales = venta.pago_vales,
                    supervisor = venta.supervisor,
                    total_vendido = venta.total_vendido,
                    vendedor = venta.vendedor,
                    no_articulo = venta.num_registros,

                };

                devOpos.printTicketSaleOnPosPrinter(entidad);

                Task work = Task.Run(() =>
                {
                    try
                    {
                        IVentaBusiness IventaBusiness = new VentaBusiness();
                        VentaDM ventaDM = IventaBusiness.GetVentaByFolio(venta.folio);
                        IRabbitEventBus rabbitEvent = new RabbitEventBus();
                        rabbitEvent.Producer(new VentaQueue() { CorrelationId = Guid.NewGuid().ToString(), Body = ventaDM });
                        IventaBusiness.UpdateUploadField(venta.id_venta);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Fue imposible realizar conexión a traves de RabbitMQ, el cobro se realizo en la caja: ", ex.Message);

                    }
                               
                });
                work.Wait();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cobro de Venta", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                newSale();
            }
        }
        #endregion

        #region Boton Cambiar Precio

        private void btnCambiarPrecio_Click(object sender, EventArgs e)
        {
            try
            {
                if (panelCambio.Visible || !sale.saleStarted() || statusModeOperation.CompareTo(modeOperation.Sale) != 0 || sale.articulosVendidos <= 0)
                {
                    return;
                }
                int indexItem = getIndexItem();
                if (StatusItem.Eliminado.CompareTo(listProducts.Items[indexItem].Tag) == 0)
                {
                    throw new Exception("Elija el articulo para aplicar el cambio de precio");
                }
                if (new ValidateUser("cambiar_precio").ShowDialog() == DialogResult.OK && new FunctionsPOS(FunctionsPOS.Options.CambiarPrecio).ShowDialog() == DialogResult.OK)
                {
                    if (listProducts.Items.Count > 0 && changePrice > 0.0m)
                    {
                        SuPlazaPOS35.domain.venta_articulo venta_articulo = sale.setPrice(indexItem, changePrice);
                        listProducts.Items[indexItem].SubItems[3].Text = DsiCodeUtil.CurrencyFormat(venta_articulo.precio_vta);
                        listProducts.Items[indexItem].SubItems[6].Text = DsiCodeUtil.CurrencyFormat(venta_articulo.total());
                        listProducts.Items[indexItem].ForeColor = Color.DarkGreen;
                        listProducts.Items[indexItem].Tag = StatusItem.PrecioModificado;
                        listProducts.Items[indexItem].Selected = false;
                    }
                    showCalculate();
                }
                cleanAndFocus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cambio de precio", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion 

        #region Verificar
        private void btnVerificar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && statusModeOperation.CompareTo(modeOperation.Sale) == 0)
                {
                    statusModeOperation = modeOperation.Verify;
                    functions = new FunctionsPOS(FunctionsPOS.Options.Verificar);
                    if (functions.ShowDialog() == DialogResult.Cancel)
                    {
                        txtCodigoBarras.Select();
                        statusModeOperation = modeOperation.Sale;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Verificado de Precios", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Cantidad
        private void btnCantidad_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && sale.articulosVendidos > 0)
                {
                    int indexItem = getIndexItem();
                    if (StatusItem.Eliminado.CompareTo(listProducts.Items[indexItem].Tag) != 0 && new FunctionsPOS(FunctionsPOS.Options.Cantidad, listProducts.Items[indexItem].SubItems[2].Text.Trim()).ShowDialog() == DialogResult.OK)
                    {
                        SuPlazaPOS35.domain.venta_articulo venta_articulo = sale.setQuality(indexItem, changeQuality);
                        listProducts.Items[indexItem].SubItems[4].Text = venta_articulo.cantidad.ToString("G");
                        listProducts.Items[indexItem].SubItems[5].Text = DsiCodeUtil.CurrencyFormat(venta_articulo.descuento());
                        listProducts.Items[indexItem].SubItems[6].Text = DsiCodeUtil.CurrencyFormat(venta_articulo.total());
                        listProducts.Items[indexItem].Selected = false;
                        listProducts.EnsureVisible(listProducts.Items.Count - 1);
                        calcularTotales();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cantidad del Artículo", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Desc Producto
        private void btnDescProduct_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && sale.articulosVendidos > 0 && new ValidateUser("desc_online").ShowDialog() == DialogResult.OK)
                {
                    int indexItem = getIndexItem();
                    if (StatusItem.Eliminado.CompareTo(listProducts.Items[indexItem].Tag) != 0 && new FunctionsPOS(FunctionsPOS.Options.PorcentLine).ShowDialog() == DialogResult.OK)
                    {
                        SuPlazaPOS35.domain.venta_articulo venta_articulo = sale.setPorcentLine(indexItem, changePorcent);
                        listProducts.Items[indexItem].SubItems[5].Text = DsiCodeUtil.CurrencyFormat(venta_articulo.descuento());
                        listProducts.Items[indexItem].SubItems[6].Text = DsiCodeUtil.CurrencyFormat(venta_articulo.total());
                        listProducts.Items[indexItem].ForeColor = ((venta_articulo.porcent_desc != 0.0m) ? Color.DarkGreen : Color.Blue);
                        listProducts.Items[indexItem].Selected = false;
                        listProducts.EnsureVisible(listProducts.Items.Count - 1);
                        calcularTotales();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Descuento en Línea", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Descuento Global
        private void btnDescGlobal_Click(object sender, EventArgs e)
        {
            try
            {
                if (panelCambio.Visible || !sale.saleStarted() || statusModeOperation.CompareTo(modeOperation.Sale) != 0 || sale.articulosVendidos <= 0 || new ValidateUser("desc_global").ShowDialog() != DialogResult.OK || new FunctionsPOS(FunctionsPOS.Options.PorcentGlobal).ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                for (int i = 0; i < listProducts.Items.Count; i++)
                {
                    if (StatusItem.Eliminado.CompareTo(listProducts.Items[i].Tag) != 0)
                    {
                        SuPlazaPOS35.domain.venta_articulo venta_articulo = sale.setPorcentGlobal(i, changePorcent);
                        listProducts.Items[i].SubItems[5].Text = DsiCodeUtil.CurrencyFormat(venta_articulo.descuento());
                        listProducts.Items[i].SubItems[6].Text = DsiCodeUtil.CurrencyFormat(venta_articulo.total());
                        listProducts.Items[i].ForeColor = ((venta_articulo.porcent_desc != 0.0m) ? Color.DarkGreen : Color.Blue);
                        listProducts.Items[i].Selected = false;
                    }
                }
                listProducts.EnsureVisible(listProducts.Items.Count - 1);
                calcularTotales();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Descuento Global", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Cargar Items List Products
        private void loadItemsToListProducts()
        {
            listProducts.Items.Clear();
            foreach (SuPlazaPOS35.domain.venta_articulo itemsSale in sale.getItemsSales())
            {
                listProducts.Items.Add(new ListViewItem(new string[7]
                {
                itemsSale.cod_barras,
                itemsSale.articulo.descripcion,
                itemsSale.articulo.unidad_medida.descripcion,
                DsiCodeUtil.CurrencyFormat(itemsSale.precio_vta),
                itemsSale.cantidad.ToString("G"),
                DsiCodeUtil.CurrencyFormat(itemsSale.descuento()),
                DsiCodeUtil.CurrencyFormat(itemsSale.total())
                })).ForeColor = Color.Blue;
            }
        }
        #endregion

        #region  GetIndexLastItem
        private int getIndexLastItem()
        {
            if (listProducts.Items.Count > 0)
            {
                return listProducts.Items.Count - 1;
            }
            throw new Exception("No hay articulos registrados para esta operación");
        }
        #endregion

        #region GetIndexSelected
        private int getIndexSelectedItem()
        {
            if (listProducts.Items.Count > 0)
            {
                return listProducts.Items.IndexOf(listProducts.SelectedItems[0]);
            }
            throw new Exception("No hay articulos registrados para esta operación");
        }
        #endregion

        #region GetIndexItem
        private int getIndexItem()
        {
            if (listProducts.SelectedIndices.Count <= 0)
            {
                return listProducts.Items.Count - 1;
            }
            return listProducts.Items.IndexOf(listProducts.SelectedItems[0]);
        }
        #endregion

        #region ReImprimir
        private void btnReimprimir_Click(object sender, EventArgs e)
        {
            try
            {
                if (panelCambio.Visible || sale.saleStarted() || statusModeOperation.CompareTo(modeOperation.Sale) != 0)
                {
                    return;
                }
                switch (MessageBox.Show("Si para Reimprimir el último ticket.\r\nNo para indicar un número de ticket.\r\nCancelar para salir de la operación.", "Reimpresión de Ticket", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
                {
                    case DialogResult.Yes:
                        devOpos.printLastOneTicketOnPosPrinter();
                        break;
                    case DialogResult.No:
                        if (new ValidateUser("reprint_tck").ShowDialog() == DialogResult.OK && new FunctionsPOS(FunctionsPOS.Options.Reimprimir).ShowDialog() == DialogResult.OK)
                        {
                            devOpos.printLastTicketOnPosPrinter(ticketNumber);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Fue imposible realizar una reimpresión de ticket: {ticketNumber}", ex);
                MessageBox.Show(ex.Message, "Reimpresión de Ticket", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }
        #endregion

        #region Suspender
        private void btnSuspender_Click(object sender, EventArgs e)
        {
        }
        #endregion

        #region Cancelar Venta
        private void btnCancelarVta_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && sale.articulosVendidos > 0 && new ValidateUser("cambiar_precio").ShowDialog() == DialogResult.OK)
                {
                    ///modifcamos este apartado para  acoplarlo a rabbitMQ
                    Guid idVentaCancelada = sale.saleCancelOrSuspend(POS.SaleRecovery ? "suspecancel" : "cancelada");
                    #region Envio de Venta Cancelada Rabbit
                    Task work = Task.Run(() =>
                    {
                        try
                        {
                            IVentaCanceladaBusiness ventaCancelada = new VentaCanceladaBusiness();
                            VentaCanceladaDM ventaCanceladaDM = ventaCancelada.GetCancelSaleByIdCancelSale(idVentaCancelada);
                            IRabbitEventBus rabbitEvent = new RabbitEventBus();
                            rabbitEvent.Producer(new VentaCanceladaQueue() { CorrelationId = Guid.NewGuid().ToString(), Body = ventaCanceladaDM });
                            ventaCancelada.UpdateUploadField(idVentaCancelada);
                        }
                        catch (Exception ex)
                        {
                            logger.Error("Fue imposible realizar conexión a traves de RabbotMQ, se realizo la cancelacion en la caja: ", ex.Message);

                        }
                                     
                    });
                    work.Wait();
                    #endregion
                    newSale();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Suspender Venta", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        #endregion

        #region Recuperar
        private void btnRecuperar_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && listProducts.Items.Count == 0 && new FunctionsPOS(FunctionsPOS.Options.Recuperar).ShowDialog() == DialogResult.OK && POS.SaleRecovery)
                {
                    saleItemRecovery();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Recuperación de ventas suspendidas", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        #endregion

        #region Boton Corte X
        private void btnCorteX_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && !sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0)
                {
                    if (sale.existsSuspendedSales() > 0)
                    {
                        throw new Exception("Hay " + sale.existsSuspendedSales() + " venta(s) suspendida(s).");
                    }
                    if (new ValidateUser("corte_x").ShowDialog() == DialogResult.OK)
                    {
                        CorteX();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Corte X", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        #endregion

        #region Boton Devolucion
        private void btnDevolucion_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && !sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && new ValidateUser("devolver_vta").ShowDialog() == DialogResult.OK && new FunctionsPOS(FunctionsPOS.Options.Devolucion).ShowDialog() == DialogResult.OK)
                {
                    statusModeOperation = modeOperation.Devolution;
                    listProducts.Visible = false;
                    listDevolutionItems.Visible = true;
                    lblTitleNoArticulos.Text = "# Articulos devueltos:";
                    lblTitleTotal.Text = "Total a Regresar: $";
                    showAllItemsDevolutions();
                    txtCodigoBarras.Enabled = false;
                    cleanAndFocus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Devoluciones", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        #endregion

        #region  Boton Cajon
        private void btnCajon_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && !sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && new ValidateUser("abrir_caja").ShowDialog() == DialogResult.OK)
                {
                    devOpos.openNowCashDrawer();
                }
            }
            catch (Exception ex)
            {
                logger.Error($"Se produjo la siguiente excepción: {ex}");
                logger.Error($"Se produjo la siguiente excepción erik: {ex.Message}");
                MessageBox.Show(ex.Message, "Apertura de Cajón", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        #endregion

        #region Boton de  Configuracion
        private void btnConfiguracion_Click(object sender, EventArgs e)
        {
            try
            {
                if (!panelCambio.Visible && !sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0 && new ValidateUser("config_pos").ShowDialog() == DialogResult.OK)
                {
                    new Config().ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Impresión de Corte X", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        #endregion

        #region Boton ExitPOS
        private void btnExitPOS_Click(object sender, EventArgs e)
        {
            cancelPOS = false;
        }
        #endregion

        #region FormClosing
        private void SuPlazaPOS_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!cancelPOS)
            {
                e.Cancel = closePOS();
            }
            else
            {
                e.Cancel = true;
            }
        }
        #endregion

        #region ClosePos
        private bool closePOS()
        {
            try
            {
                if (!sale.saleStarted() && statusModeOperation.CompareTo(modeOperation.Sale) == 0)
                {
                    if (devOpos.isOpenCashDrawer())
                    {
                        throw new Exception("El cajón se encuentra abierto");
                    }
                    switch (MessageBox.Show("¿Desea salir completamente del sistema?\nSi para cerrar definitivamente.\nNo para cerrar temporalmente.\nCancelar para anular ésta operación", "Salir de " + Application.ProductName, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
                    {
                        case DialogResult.Yes:
                            if (sale.existsSuspendedSales() > 0)
                            {
                                MessageBox.Show(string.Concat("Hay " + sale.existsSuspendedSales() + " venta(s) suspendida(s)."), "Ventas suspendidas en el POS", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                                return true;
                            }
                            if (new ValidateUser("corte_z").ShowDialog() == DialogResult.OK)
                            {
                                CorteZ();
                                break;
                            }
                            return true;
                        case DialogResult.No:
                            if (sale.existsSuspendedSales() > 0)
                            {
                                throw new Exception("Hay " + sale.existsSuspendedSales() + " venta(s) suspendida(s).");
                            }
                            break;
                        case DialogResult.Cancel:
                            corte = null;
                            txtCodigoBarras.SelectAll();
                            return true;
                    }
                    devOpos.closeDevicesOpos();

                    return false;
                }
                if (statusModeOperation.CompareTo(modeOperation.Devolution) == 0)
                {
                    if (MessageBox.Show("¿Desea salir cancelar la operación?", "Devoluciones", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        newSale();
                    }
                    return true;
                }
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error al salir de POS", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return true;
            }
        }
        #endregion

        #region Corte X
        private void CorteX()
        {
            try
            {
                SuPlazaPosUtil suPlazaPosUtil = new SuPlazaPosUtil();
                corte corteX = suPlazaPosUtil.GetCorteByDate(DateTime.Now);
                logger.Info("Info del JSON: {0}", JsonConvert.SerializeObject(corteX));

                if (corteX == null)
                {
                    throw new Exception("No hay ventas registradas para realizar el corte solicitado");
                }
                devOpos.imprimirCorte(DevicesOPOS.PrintTicket.corte_x, corteX);
                GC.Collect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error en Corte X", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        #endregion

        #region Corte Z
        private void CorteZ()
        {
            try
            {
                DateTime now = DateTime.Now;
                //corte corte = new CorteDAO().getCorte(now);
                SuPlazaPosUtil suPlazaPosUtil = new SuPlazaPosUtil();
                corte corteZ = suPlazaPosUtil.GetCorteByDate(now);
                if (corteZ == null)
                {
                    throw new Exception("No hay ventas registradas para realizar el corte solicitado");
                }
                devOpos.imprimirCorte(DevicesOPOS.PrintTicket.corte_z, corteZ);  //corte
                GC.Collect();
                devOpos.openNowCashDrawer();
                new CorteDAO().setLastCut(now);
            }
            catch (Exception ex)
            {
                logger.Error($"Se produjo la siguiente excepcion corte Z: {ex}");
                MessageBox.Show(ex.Message, "Error en Corte Z", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        #endregion

        #region Boton Eliminar
        private void btnEliminar_Click(object sender, EventArgs e)
        {
            try
            {
                if (panelCambio.Visible || !sale.saleStarted() || statusModeOperation.CompareTo(modeOperation.Sale) != 0)
                {
                    return;
                }
                int index;
                if (listProducts.SelectedItems.Count > 0)
                {
                    index = getIndexSelectedItem();
                    if (new ValidateUser("eliminar_art").ShowDialog() != DialogResult.OK || StatusItem.Eliminado.CompareTo(listProducts.Items[index].Tag) == 0)
                    {
                        return;
                    }
                }
                else
                {
                    index = getIndexLastItem();
                    if (sale.deleteItemLast || StatusItem.Eliminado.CompareTo(listProducts.Items[index].Tag) == 0 || (POS.SaleRecovery && new ValidateUser("eliminar_art").ShowDialog() != DialogResult.OK))
                    {
                        return;
                    }
                    sale.deleteItemLast = true;
                }
                using (DataClassesPOSDataContext dataClassesPOSDataContext = new DataClassesPOSDataContext())
                {
                    SuPlazaPOS35.domain.venta_articulo itemDeleted = sale.deleteItem(index, sale.deleteItemLast);
                    itemDeleted.articulo = dataClassesPOSDataContext.articulo.FirstOrDefault((articulo i) => i.cod_barras.Equals(itemDeleted.cod_barras));
                    ListViewItem listViewItem = new ListViewItem(new string[7]
                    {
                        itemDeleted.cod_barras,
                        itemDeleted.articulo.descripcion,
                        itemDeleted.articulo.unidad_medida.descripcion,
                        DsiCodeUtil.CurrencyFormat(itemDeleted.precio_compra),
                        itemDeleted.cantidad.ToString("G"),
                        DsiCodeUtil.CurrencyFormat(itemDeleted.descuento()),
                        DsiCodeUtil.CurrencyFormat(itemDeleted.total())
                    });
                    listViewItem.Tag = StatusItem.Eliminado;
                    listProducts.Items.Add(listViewItem).ForeColor = Color.Red;
                    listProducts.Items[index].ForeColor = Color.DarkRed;
                    listProducts.Items[index].Tag = StatusItem.Eliminado;
                    listProducts.Items[index].Selected = false;
                    showCalculate();
                }
                cleanAndFocus();
                listProducts.EnsureVisible(listProducts.Items.Count - 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
            }
        }
        #endregion

        #region Reset Totales
        public void resetTotales()
        {
            sale.articulosVendidos = 0;
            sale.subTotal = 0.0m;
            sale.descuento = 0.0m;
            sale.iva = 0.0m;
            sale.impuestos = 0.0m;
            sale.ieps = 0.0m;
            sale.total = 0.0m;
        }
        #endregion

        #region Calcular Totales
        private void calcularTotales()
        {
            showCalculate();
            if (statusModeOperation.CompareTo(modeOperation.Sale) == 0)
            {
                cleanAndFocus();
                if (listProducts.Items.Count > 0)
                {
                    listProducts.EnsureVisible(listProducts.Items.Count - 1);
                }
            }
        }
        #endregion

        #region Mostrar Calcular
        private void showCalculate()
        {
            if (statusModeOperation.CompareTo(modeOperation.Sale) == 0)
            {
                sale.calculate();
            }
            else if (statusModeOperation.CompareTo(modeOperation.Devolution) == 0)
            {
                sale.calculateDevolution();
            }
            lblNoArticulosVendidos.Text = sale.articulosVendidos.ToString();
            lblSubtotal.Text = DsiCodeUtil.CurrencyFormat(sale.subTotal);
            lblDescuento.Text = DsiCodeUtil.CurrencyFormat(sale.descuento);
            lblIVA.Text = DsiCodeUtil.CurrencyFormat(sale.impuestos);
            lblTotal.Text = DsiCodeUtil.CurrencyFormat(sale.total).Replace("$", "");
            POS.totalVenta = sale.total;
        }
        #endregion

        #region ListDevolutionItems
        private void listDevolutionItems_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.F3)
                {
                    btnCobrar_Click(sender, e);
                }
                if (e.KeyCode != Keys.Return || listDevolutionItems.SelectedItems.Count <= 0)
                {
                    return;
                }
                foreach (ListViewItem selectedItem in listDevolutionItems.SelectedItems)
                {
                    if (POS.sale.venta_articulo[selectedItem.Index].cantidad <= 1m || selectedItem.SubItems[2].Text.Equals("Kg"))
                    {
                        POS.sale.venta_articulo[selectedItem.Index].cant_devuelta = POS.sale.venta_articulo[selectedItem.Index].cantidad;
                        POS.sale.venta_articulo[selectedItem.Index].cantidad_a_devolver = POS.sale.venta_articulo[selectedItem.Index].cantidad;
                        continue;
                    }
                    changeQuality = POS.sale.venta_articulo[selectedItem.Index].cantidad_por_devolver();
                    if (new FunctionsPOS(FunctionsPOS.Options.Cantidad, selectedItem.SubItems[2].Text).ShowDialog() == DialogResult.OK)
                    {
                        POS.sale.venta_articulo[selectedItem.Index].cant_devuelta += changeQuality;
                        POS.sale.venta_articulo[selectedItem.Index].cantidad_a_devolver = changeQuality;
                        continue;
                    }
                    break;
                }
                showAllItemsDevolutions();
            }
            catch (Exception ex)
            {
                logger.Error("Fue imposible realizar una devolución: ", ex);
                logger.Error("Fue imposible realizar una devolución dsi_code: " + ex.Message);
                MessageBox.Show(ex.Message, "Devoluciones", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }
        #endregion

        #region Mostrar todos los items
        private void showAllItemsDevolutions()
        {
            try
            {
                if (POS.sale.venta_articulo.Count() <= 0)
                {
                    return;
                }
                listDevolutionItems.Items.Clear();
                foreach (SuPlazaPOS35.domain.venta_articulo item in POS.sale.venta_articulo)
                {
                    ListViewItem value = new ListViewItem(new string[7]


                    {
                    item.cod_barras,
                    item.articulo.descripcion,
                    item.articulo.unidad_medida.descripcion,
                    DsiCodeUtil.CurrencyFormat(item.precio_vta),
                    item.cantidad_por_devolver().ToString("G"),
                    item.cantidad_a_devolver.ToString("G"),
                    DsiCodeUtil.CurrencyFormat(item.totalDevolucion())
                    });
                    listDevolutionItems.Items.Add(value).ForeColor = ((item.cantidad_a_devolver > 0m) ? Color.Red : Color.Black);
                }
                calcularTotales();
                listDevolutionItems.Select();
            }
            catch (Exception ex)
            {
                logger.Error("Fue imposible realizar una lista de devolución dsi_code: " + ex.Message);
                logger.Error("Fue imposible mostrar una lista de devolución: ", ex);
            }
        }
        #endregion

        #region Limpiar Totales
        private void cleanTotales()
        {
            lblNoArticulosVendidos.Text = "0";
            lblSubtotal.Text = "$0.00";
            lblIVA.Text = "$0.00";
            lblDescuento.Text = "$0.00";
            lblTotal.Text = "$0.00";
        }
        #endregion

        #region  F1 Tecla
        private void F1_KeyPress(object sender, KeyPressEventArgs e)
        {
            txtCodigoBarras.Focus();
            txtCodigoBarras.SelectAll();
        }
        #endregion

        #region Lista de Productos
        private void listProducts_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
        #endregion

        #region Panel Paint
        private void panel1_Paint(object sender, PaintEventArgs e)
        {
        }
        #endregion

        #region Componentes
        private IContainer components;

        private StatusStrip statusBar;

        private ToolStripStatusLabel tlsCaja;

        private ToolStripStatusLabel tlsUsuario;

        private ToolStripStatusLabel tlsFecha;

        private ToolStripStatusLabel tlsHour;

        private System.Windows.Forms.Timer tmrHour;

        private SplitContainer splitContainer1;

        private SplitContainer splitContainer2;

        private SplitContainer splitContainer3;

        private Panel panel1;

        private Label lblTotal;

        private Label lblIVA;

        private Label lblDescuento;

        private Label lblTitleTotal;

        private Label lblSubtotal;

        private Label lblTitleIVA;

        private Label lblTitleDescuento;

        private Label lblTitleSubtotal;

        private Panel panel2;

        private ListView listProducts;

        private ColumnHeader codigo;

        private ColumnHeader descripcion;

        private ColumnHeader unidad;

        private ColumnHeader precio;

        private ColumnHeader cantidad;

        private ColumnHeader total;

        private ColumnHeader descuento;

        private Label lblNoArticulosVendidos;

        private Label lblTitleNoArticulos;

        private Panel panel3;

        private Panel panel4;

        private Button btnBuscar;

        private TextBox txtCodigoBarras;

        private Button btnVerificar;

        private Button btnCobrar;

        private Button btnCantidad;

        private Button btnEliminar;

        private Button btnCambiarPrecio;

        private Button btnDescProduct;

        private Button btnCancelarVta;

        private Button btnReimprimir;

        private Button btnDevolucion;

        private Button btnCorteX;

        private Button btnDescGlobal;

        private Button btnCajon;

        private Button btnConfiguracion;

        private Button btnExitPOS;

        private Panel panelCambio;

        private Label lblCambio;

        private Label label1;

        private ListView listDevolutionItems;

        private ColumnHeader columnHeader1;

        private ColumnHeader columnHeader2;

        private ColumnHeader columnHeader3;

        private ColumnHeader columnHeader4;

        private ColumnHeader columnHeader5;

        private ColumnHeader columnHeader6;

        private ColumnHeader columnHeader7;

        private ToolStripStatusLabel tlsStatusSync;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.statusBar = new System.Windows.Forms.StatusStrip();
            this.tlsCaja = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlsUsuario = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlsStatusSync = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlsFecha = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlsHour = new System.Windows.Forms.ToolStripStatusLabel();
            this.tmrHour = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panelCambio = new System.Windows.Forms.Panel();
            this.lblCambio = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.listProducts = new System.Windows.Forms.ListView();
            this.codigo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descripcion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.unidad = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.precio = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.cantidad = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descuento = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.total = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listDevolutionItems = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer3 = new System.Windows.Forms.SplitContainer();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtCodigoBarras = new System.Windows.Forms.TextBox();
            this.btnCambiarPrecio = new System.Windows.Forms.Button();
            this.btnVerificar = new System.Windows.Forms.Button();
            this.btnBuscar = new System.Windows.Forms.Button();
            this.btnCobrar = new System.Windows.Forms.Button();
            this.btnCantidad = new System.Windows.Forms.Button();
            this.btnEliminar = new System.Windows.Forms.Button();
            this.btnCancelarVta = new System.Windows.Forms.Button();
            this.btnDescProduct = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.lblTotal = new System.Windows.Forms.Label();
            this.lblIVA = new System.Windows.Forms.Label();
            this.lblDescuento = new System.Windows.Forms.Label();
            this.lblTitleTotal = new System.Windows.Forms.Label();
            this.lblNoArticulosVendidos = new System.Windows.Forms.Label();
            this.lblSubtotal = new System.Windows.Forms.Label();
            this.lblTitleIVA = new System.Windows.Forms.Label();
            this.lblTitleNoArticulos = new System.Windows.Forms.Label();
            this.lblTitleDescuento = new System.Windows.Forms.Label();
            this.lblTitleSubtotal = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnExitPOS = new System.Windows.Forms.Button();
            this.btnConfiguracion = new System.Windows.Forms.Button();
            this.btnCajon = new System.Windows.Forms.Button();
            this.btnDevolucion = new System.Windows.Forms.Button();
            this.btnCorteX = new System.Windows.Forms.Button();
            this.btnDescGlobal = new System.Windows.Forms.Button();
            this.btnReimprimir = new System.Windows.Forms.Button();
            this.statusBar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panelCambio.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).BeginInit();
            this.splitContainer3.Panel1.SuspendLayout();
            this.splitContainer3.Panel2.SuspendLayout();
            this.splitContainer3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusBar
            // 
            this.statusBar.ImageScalingSize = new System.Drawing.Size(28, 28);
            this.statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tlsCaja,
            this.tlsUsuario,
            this.tlsStatusSync,
            this.tlsFecha,
            this.tlsHour});
            this.statusBar.Location = new System.Drawing.Point(0, 706);
            this.statusBar.Name = "statusBar";
            this.statusBar.Size = new System.Drawing.Size(1008, 24);
            this.statusBar.TabIndex = 0;
            this.statusBar.Text = "statusStrip1";
            this.statusBar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // tlsCaja
            // 
            this.tlsCaja.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tlsCaja.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.tlsCaja.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tlsCaja.Name = "tlsCaja";
            this.tlsCaja.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.tlsCaja.Size = new System.Drawing.Size(68, 19);
            this.tlsCaja.Text = "Caja: 01";
            // 
            // tlsUsuario
            // 
            this.tlsUsuario.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tlsUsuario.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.tlsUsuario.Name = "tlsUsuario";
            this.tlsUsuario.Size = new System.Drawing.Size(177, 19);
            this.tlsUsuario.Text = " Cajer@: Oscar Jiménez Aguilar ";
            // 
            // tlsStatusSync
            // 
            this.tlsStatusSync.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tlsStatusSync.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.tlsStatusSync.Name = "tlsStatusSync";
            this.tlsStatusSync.Size = new System.Drawing.Size(576, 19);
            this.tlsStatusSync.Spring = true;
            this.tlsStatusSync.Text = "Estatus de Sincronización";
            // 
            // tlsFecha
            // 
            this.tlsFecha.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tlsFecha.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.tlsFecha.Name = "tlsFecha";
            this.tlsFecha.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.tlsFecha.Size = new System.Drawing.Size(103, 19);
            this.tlsFecha.Text = "01/Enero/2015";
            // 
            // tlsHour
            // 
            this.tlsHour.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Right) 
            | System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
            this.tlsHour.BorderStyle = System.Windows.Forms.Border3DStyle.Sunken;
            this.tlsHour.Name = "tlsHour";
            this.tlsHour.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.tlsHour.Size = new System.Drawing.Size(69, 19);
            this.tlsHour.Text = "00:00:00";
            // 
            // tmrHour
            // 
            this.tmrHour.Interval = 1000;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.splitContainer2);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panel4);
            this.splitContainer1.Size = new System.Drawing.Size(1008, 706);
            this.splitContainer1.SplitterDistance = 640;
            this.splitContainer1.TabIndex = 1;
            this.splitContainer1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panel2);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.splitContainer3);
            this.splitContainer2.Size = new System.Drawing.Size(1008, 640);
            this.splitContainer2.SplitterDistance = 494;
            this.splitContainer2.TabIndex = 0;
            this.splitContainer2.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.panelCambio);
            this.panel2.Controls.Add(this.listProducts);
            this.panel2.Controls.Add(this.listDevolutionItems);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1008, 494);
            this.panel2.TabIndex = 1;
            // 
            // panelCambio
            // 
            this.panelCambio.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.panelCambio.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelCambio.Controls.Add(this.lblCambio);
            this.panelCambio.Controls.Add(this.label1);
            this.panelCambio.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panelCambio.Location = new System.Drawing.Point(159, 130);
            this.panelCambio.Name = "panelCambio";
            this.panelCambio.Size = new System.Drawing.Size(525, 80);
            this.panelCambio.TabIndex = 2;
            // 
            // lblCambio
            // 
            this.lblCambio.BackColor = System.Drawing.Color.White;
            this.lblCambio.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCambio.ForeColor = System.Drawing.Color.Navy;
            this.lblCambio.Location = new System.Drawing.Point(212, 19);
            this.lblCambio.Name = "lblCambio";
            this.lblCambio.Size = new System.Drawing.Size(288, 49);
            this.lblCambio.TabIndex = 0;
            this.lblCambio.Text = "$0.00";
            this.lblCambio.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(3, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(213, 29);
            this.label1.TabIndex = 0;
            this.label1.Text = "Su cambio:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // listProducts
            // 
            this.listProducts.BackColor = System.Drawing.Color.White;
            this.listProducts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.codigo,
            this.descripcion,
            this.unidad,
            this.precio,
            this.cantidad,
            this.descuento,
            this.total});
            this.listProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listProducts.FullRowSelect = true;
            this.listProducts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listProducts.HideSelection = false;
            this.listProducts.Location = new System.Drawing.Point(-1, -1);
            this.listProducts.MultiSelect = false;
            this.listProducts.Name = "listProducts";
            this.listProducts.Size = new System.Drawing.Size(1017, 768);
            this.listProducts.TabIndex = 1;
            this.listProducts.UseCompatibleStateImageBehavior = false;
            this.listProducts.View = System.Windows.Forms.View.Details;
            this.listProducts.SelectedIndexChanged += new System.EventHandler(this.listProducts_SelectedIndexChanged);
            this.listProducts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            this.listProducts.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.F1_KeyPress);
            // 
            // codigo
            // 
            this.codigo.Text = "      Código";
            this.codigo.Width = 148;
            // 
            // descripcion
            // 
            this.descripcion.Text = "Descripción";
            this.descripcion.Width = 447;
            // 
            // unidad
            // 
            this.unidad.Text = "Unidad";
            this.unidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.unidad.Width = 71;
            // 
            // precio
            // 
            this.precio.Text = "Precio";
            this.precio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.precio.Width = 82;
            // 
            // cantidad
            // 
            this.cantidad.Text = "Cant.";
            this.cantidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cantidad.Width = 63;
            // 
            // descuento
            // 
            this.descuento.Text = "$ Desc";
            this.descuento.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.descuento.Width = 70;
            // 
            // total
            // 
            this.total.Text = "Total";
            this.total.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.total.Width = 119;
            // 
            // listDevolutionItems
            // 
            this.listDevolutionItems.BackColor = System.Drawing.Color.LightBlue;
            this.listDevolutionItems.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7});
            this.listDevolutionItems.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listDevolutionItems.ForeColor = System.Drawing.SystemColors.WindowText;
            this.listDevolutionItems.FullRowSelect = true;
            this.listDevolutionItems.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listDevolutionItems.HideSelection = false;
            this.listDevolutionItems.Location = new System.Drawing.Point(382, -1);
            this.listDevolutionItems.Name = "listDevolutionItems";
            this.listDevolutionItems.Size = new System.Drawing.Size(445, 211);
            this.listDevolutionItems.TabIndex = 4;
            this.listDevolutionItems.UseCompatibleStateImageBehavior = false;
            this.listDevolutionItems.View = System.Windows.Forms.View.Details;
            this.listDevolutionItems.Visible = false;
            this.listDevolutionItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listDevolutionItems_KeyDown);
            this.listDevolutionItems.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.F1_KeyPress);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "      Código";
            this.columnHeader1.Width = 120;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Descripción";
            this.columnHeader2.Width = 292;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Unidad";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 70;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Precio";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader4.Width = 70;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Cant.";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader5.Width = 70;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Devol.";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader6.Width = 70;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Total";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader7.Width = 80;
            // 
            // splitContainer3
            // 
            this.splitContainer3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer3.Location = new System.Drawing.Point(0, 0);
            this.splitContainer3.Name = "splitContainer3";
            // 
            // splitContainer3.Panel1
            // 
            this.splitContainer3.Panel1.Controls.Add(this.panel3);
            // 
            // splitContainer3.Panel2
            // 
            this.splitContainer3.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer3.Panel2.Controls.Add(this.panel1);
            this.splitContainer3.Size = new System.Drawing.Size(1008, 142);
            this.splitContainer3.SplitterDistance = 356;
            this.splitContainer3.TabIndex = 0;
            this.splitContainer3.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(35)))), ((int)(((byte)(242)))));
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.txtCodigoBarras);
            this.panel3.Controls.Add(this.btnCambiarPrecio);
            this.panel3.Controls.Add(this.btnVerificar);
            this.panel3.Controls.Add(this.btnBuscar);
            this.panel3.Controls.Add(this.btnCobrar);
            this.panel3.Controls.Add(this.btnCantidad);
            this.panel3.Controls.Add(this.btnEliminar);
            this.panel3.Controls.Add(this.btnCancelarVta);
            this.panel3.Controls.Add(this.btnDescProduct);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.panel3.Size = new System.Drawing.Size(356, 142);
            this.panel3.TabIndex = 0;
            // 
            // txtCodigoBarras
            // 
            this.txtCodigoBarras.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.txtCodigoBarras.Location = new System.Drawing.Point(12, 14);
            this.txtCodigoBarras.Name = "txtCodigoBarras";
            this.txtCodigoBarras.Size = new System.Drawing.Size(327, 26);
            this.txtCodigoBarras.TabIndex = 0;
            this.txtCodigoBarras.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCodigoBarras.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnCambiarPrecio
            // 
            this.btnCambiarPrecio.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCambiarPrecio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCambiarPrecio.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCambiarPrecio.Location = new System.Drawing.Point(256, 50);
            this.btnCambiarPrecio.Name = "btnCambiarPrecio";
            this.btnCambiarPrecio.Size = new System.Drawing.Size(75, 34);
            this.btnCambiarPrecio.TabIndex = 0;
            this.btnCambiarPrecio.Text = "[F5]\r\nPrecio";
            this.btnCambiarPrecio.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCambiarPrecio.UseVisualStyleBackColor = false;
            this.btnCambiarPrecio.Click += new System.EventHandler(this.btnCambiarPrecio_Click);
            this.btnCambiarPrecio.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnVerificar
            // 
            this.btnVerificar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnVerificar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnVerificar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnVerificar.Location = new System.Drawing.Point(174, 50);
            this.btnVerificar.Name = "btnVerificar";
            this.btnVerificar.Size = new System.Drawing.Size(75, 34);
            this.btnVerificar.TabIndex = 0;
            this.btnVerificar.Text = "[F4]\r\nVerificar";
            this.btnVerificar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnVerificar.UseVisualStyleBackColor = false;
            this.btnVerificar.Click += new System.EventHandler(this.btnVerificar_Click);
            this.btnVerificar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnBuscar
            // 
            this.btnBuscar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnBuscar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnBuscar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnBuscar.Location = new System.Drawing.Point(10, 50);
            this.btnBuscar.Name = "btnBuscar";
            this.btnBuscar.Size = new System.Drawing.Size(75, 34);
            this.btnBuscar.TabIndex = 0;
            this.btnBuscar.Text = "[F2]\r\nBuscar";
            this.btnBuscar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnBuscar.UseVisualStyleBackColor = false;
            this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);
            this.btnBuscar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnCobrar
            // 
            this.btnCobrar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCobrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCobrar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCobrar.Location = new System.Drawing.Point(92, 50);
            this.btnCobrar.Name = "btnCobrar";
            this.btnCobrar.Size = new System.Drawing.Size(75, 34);
            this.btnCobrar.TabIndex = 0;
            this.btnCobrar.Text = "[F3]\r\nCobrar";
            this.btnCobrar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCobrar.UseVisualStyleBackColor = false;
            this.btnCobrar.Click += new System.EventHandler(this.btnCobrar_Click);
            this.btnCobrar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnCantidad
            // 
            this.btnCantidad.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCantidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCantidad.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCantidad.Location = new System.Drawing.Point(10, 89);
            this.btnCantidad.Name = "btnCantidad";
            this.btnCantidad.Size = new System.Drawing.Size(75, 34);
            this.btnCantidad.TabIndex = 0;
            this.btnCantidad.Text = "[F6]\r\nCantidad";
            this.btnCantidad.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCantidad.UseVisualStyleBackColor = false;
            this.btnCantidad.Click += new System.EventHandler(this.btnCantidad_Click);
            this.btnCantidad.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnEliminar
            // 
            this.btnEliminar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnEliminar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnEliminar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEliminar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnEliminar.Location = new System.Drawing.Point(92, 89);
            this.btnEliminar.Name = "btnEliminar";
            this.btnEliminar.Size = new System.Drawing.Size(75, 34);
            this.btnEliminar.TabIndex = 0;
            this.btnEliminar.Text = "[F7]\r\nEliminar";
            this.btnEliminar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnEliminar.UseVisualStyleBackColor = false;
            this.btnEliminar.Click += new System.EventHandler(this.btnEliminar_Click);
            this.btnEliminar.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnCancelarVta
            // 
            this.btnCancelarVta.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancelarVta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelarVta.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelarVta.Location = new System.Drawing.Point(256, 89);
            this.btnCancelarVta.Name = "btnCancelarVta";
            this.btnCancelarVta.Size = new System.Drawing.Size(75, 34);
            this.btnCancelarVta.TabIndex = 0;
            this.btnCancelarVta.Text = "[F9]\r\nCancelar";
            this.btnCancelarVta.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelarVta.UseVisualStyleBackColor = false;
            this.btnCancelarVta.Click += new System.EventHandler(this.btnCancelarVta_Click);
            this.btnCancelarVta.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnDescProduct
            // 
            this.btnDescProduct.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDescProduct.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDescProduct.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDescProduct.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDescProduct.Location = new System.Drawing.Point(174, 89);
            this.btnDescProduct.Name = "btnDescProduct";
            this.btnDescProduct.Size = new System.Drawing.Size(75, 34);
            this.btnDescProduct.TabIndex = 0;
            this.btnDescProduct.Text = "[F8]\r\n% Línea";
            this.btnDescProduct.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDescProduct.UseVisualStyleBackColor = false;
            this.btnDescProduct.Click += new System.EventHandler(this.btnDescProduct_Click);
            this.btnDescProduct.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.lblTotal);
            this.panel1.Controls.Add(this.lblIVA);
            this.panel1.Controls.Add(this.lblDescuento);
            this.panel1.Controls.Add(this.lblTitleTotal);
            this.panel1.Controls.Add(this.lblNoArticulosVendidos);
            this.panel1.Controls.Add(this.lblSubtotal);
            this.panel1.Controls.Add(this.lblTitleIVA);
            this.panel1.Controls.Add(this.lblTitleNoArticulos);
            this.panel1.Controls.Add(this.lblTitleDescuento);
            this.panel1.Controls.Add(this.lblTitleSubtotal);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(648, 142);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.Navy;
            this.lblTotal.Location = new System.Drawing.Point(431, 99);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(190, 37);
            this.lblTotal.TabIndex = 1;
            this.lblTotal.Text = "$0.00";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblIVA
            // 
            this.lblIVA.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblIVA.Location = new System.Drawing.Point(431, 47);
            this.lblIVA.Name = "lblIVA";
            this.lblIVA.Size = new System.Drawing.Size(179, 29);
            this.lblIVA.TabIndex = 2;
            this.lblIVA.Text = "$0.00";
            this.lblIVA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDescuento
            // 
            this.lblDescuento.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblDescuento.Location = new System.Drawing.Point(431, 73);
            this.lblDescuento.Name = "lblDescuento";
            this.lblDescuento.Size = new System.Drawing.Size(179, 29);
            this.lblDescuento.TabIndex = 3;
            this.lblDescuento.Text = "$0.00";
            this.lblDescuento.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitleTotal
            // 
            this.lblTitleTotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F, ((System.Drawing.FontStyle)((System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline))), System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitleTotal.ForeColor = System.Drawing.Color.Navy;
            this.lblTitleTotal.Location = new System.Drawing.Point(118, 103);
            this.lblTitleTotal.Name = "lblTitleTotal";
            this.lblTitleTotal.Size = new System.Drawing.Size(310, 34);
            this.lblTitleTotal.TabIndex = 4;
            this.lblTitleTotal.Text = "Total a Pagar: $";
            this.lblTitleTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblNoArticulosVendidos
            // 
            this.lblNoArticulosVendidos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblNoArticulosVendidos.Location = new System.Drawing.Point(463, 0);
            this.lblNoArticulosVendidos.Name = "lblNoArticulosVendidos";
            this.lblNoArticulosVendidos.Size = new System.Drawing.Size(179, 29);
            this.lblNoArticulosVendidos.TabIndex = 5;
            this.lblNoArticulosVendidos.Text = "0";
            this.lblNoArticulosVendidos.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSubtotal
            // 
            this.lblSubtotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblSubtotal.Location = new System.Drawing.Point(431, 21);
            this.lblSubtotal.Name = "lblSubtotal";
            this.lblSubtotal.Size = new System.Drawing.Size(179, 29);
            this.lblSubtotal.TabIndex = 5;
            this.lblSubtotal.Text = "$0.00";
            this.lblSubtotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitleIVA
            // 
            this.lblTitleIVA.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblTitleIVA.Location = new System.Drawing.Point(262, 51);
            this.lblTitleIVA.Name = "lblTitleIVA";
            this.lblTitleIVA.Size = new System.Drawing.Size(166, 26);
            this.lblTitleIVA.TabIndex = 6;
            this.lblTitleIVA.Text = "Impuestos:";
            this.lblTitleIVA.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitleNoArticulos
            // 
            this.lblTitleNoArticulos.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblTitleNoArticulos.Location = new System.Drawing.Point(206, 0);
            this.lblTitleNoArticulos.Name = "lblTitleNoArticulos";
            this.lblTitleNoArticulos.Size = new System.Drawing.Size(208, 26);
            this.lblTitleNoArticulos.TabIndex = 8;
            this.lblTitleNoArticulos.Text = "# Articulos vendidos:";
            this.lblTitleNoArticulos.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitleDescuento
            // 
            this.lblTitleDescuento.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblTitleDescuento.Location = new System.Drawing.Point(262, 77);
            this.lblTitleDescuento.Name = "lblTitleDescuento";
            this.lblTitleDescuento.Size = new System.Drawing.Size(166, 26);
            this.lblTitleDescuento.TabIndex = 7;
            this.lblTitleDescuento.Text = "Descuento:";
            this.lblTitleDescuento.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTitleSubtotal
            // 
            this.lblTitleSubtotal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F);
            this.lblTitleSubtotal.Location = new System.Drawing.Point(262, 25);
            this.lblTitleSubtotal.Name = "lblTitleSubtotal";
            this.lblTitleSubtotal.Size = new System.Drawing.Size(166, 26);
            this.lblTitleSubtotal.TabIndex = 8;
            this.lblTitleSubtotal.Text = "Sub Total:";
            this.lblTitleSubtotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.panel4.Controls.Add(this.btnExitPOS);
            this.panel4.Controls.Add(this.btnConfiguracion);
            this.panel4.Controls.Add(this.btnCajon);
            this.panel4.Controls.Add(this.btnDevolucion);
            this.panel4.Controls.Add(this.btnCorteX);
            this.panel4.Controls.Add(this.btnDescGlobal);
            this.panel4.Controls.Add(this.btnReimprimir);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(1008, 62);
            this.panel4.TabIndex = 0;
            // 
            // btnExitPOS
            // 
            this.btnExitPOS.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnExitPOS.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExitPOS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExitPOS.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnExitPOS.Location = new System.Drawing.Point(605, 4);
            this.btnExitPOS.Name = "btnExitPOS";
            this.btnExitPOS.Size = new System.Drawing.Size(80, 34);
            this.btnExitPOS.TabIndex = 0;
            this.btnExitPOS.Text = "[ESC]\r\nSalir";
            this.btnExitPOS.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExitPOS.UseVisualStyleBackColor = false;
            this.btnExitPOS.Click += new System.EventHandler(this.btnExitPOS_Click);
            this.btnExitPOS.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnConfiguracion
            // 
            this.btnConfiguracion.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnConfiguracion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnConfiguracion.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnConfiguracion.Location = new System.Drawing.Point(502, 4);
            this.btnConfiguracion.Name = "btnConfiguracion";
            this.btnConfiguracion.Size = new System.Drawing.Size(80, 34);
            this.btnConfiguracion.TabIndex = 0;
            this.btnConfiguracion.Text = "[ALT + O]\r\nC&onfig.";
            this.btnConfiguracion.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnConfiguracion.UseVisualStyleBackColor = false;
            this.btnConfiguracion.Click += new System.EventHandler(this.btnConfiguracion_Click);
            this.btnConfiguracion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnCajon
            // 
            this.btnCajon.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCajon.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCajon.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCajon.Location = new System.Drawing.Point(409, 4);
            this.btnCajon.Name = "btnCajon";
            this.btnCajon.Size = new System.Drawing.Size(80, 34);
            this.btnCajon.TabIndex = 0;
            this.btnCajon.Text = "[ALT + J]\r\nCa&jón";
            this.btnCajon.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCajon.UseVisualStyleBackColor = false;
            this.btnCajon.Click += new System.EventHandler(this.btnCajon_Click);
            this.btnCajon.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnDevolucion
            // 
            this.btnDevolucion.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDevolucion.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDevolucion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDevolucion.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDevolucion.Location = new System.Drawing.Point(308, 4);
            this.btnDevolucion.Name = "btnDevolucion";
            this.btnDevolucion.Size = new System.Drawing.Size(80, 34);
            this.btnDevolucion.TabIndex = 0;
            this.btnDevolucion.Text = "[ALT + D]\r\n&Devolución";
            this.btnDevolucion.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDevolucion.UseVisualStyleBackColor = false;
            this.btnDevolucion.Click += new System.EventHandler(this.btnDevolucion_Click);
            this.btnDevolucion.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnCorteX
            // 
            this.btnCorteX.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCorteX.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCorteX.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCorteX.Location = new System.Drawing.Point(210, 4);
            this.btnCorteX.Name = "btnCorteX";
            this.btnCorteX.Size = new System.Drawing.Size(80, 34);
            this.btnCorteX.TabIndex = 0;
            this.btnCorteX.Text = "[ALT + X]\r\nCorte &X";
            this.btnCorteX.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCorteX.UseVisualStyleBackColor = false;
            this.btnCorteX.Click += new System.EventHandler(this.btnCorteX_Click);
            this.btnCorteX.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnDescGlobal
            // 
            this.btnDescGlobal.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDescGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDescGlobal.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDescGlobal.Location = new System.Drawing.Point(112, 4);
            this.btnDescGlobal.Name = "btnDescGlobal";
            this.btnDescGlobal.Size = new System.Drawing.Size(80, 34);
            this.btnDescGlobal.TabIndex = 0;
            this.btnDescGlobal.Text = "[ALT + G]\r\n% &Global";
            this.btnDescGlobal.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDescGlobal.UseVisualStyleBackColor = false;
            this.btnDescGlobal.Click += new System.EventHandler(this.btnDescGlobal_Click);
            this.btnDescGlobal.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // btnReimprimir
            // 
            this.btnReimprimir.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnReimprimir.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReimprimir.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReimprimir.Location = new System.Drawing.Point(12, 4);
            this.btnReimprimir.Name = "btnReimprimir";
            this.btnReimprimir.Size = new System.Drawing.Size(80, 34);
            this.btnReimprimir.TabIndex = 0;
            this.btnReimprimir.Text = "[F10]\r\nReimprimir";
            this.btnReimprimir.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReimprimir.UseVisualStyleBackColor = false;
            this.btnReimprimir.Click += new System.EventHandler(this.btnReimprimir_Click);
            this.btnReimprimir.KeyDown += new System.Windows.Forms.KeyEventHandler(this.functionsOfPOS);
            // 
            // SuPlazaPOS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExitPOS;
            this.ClientSize = new System.Drawing.Size(1008, 730);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.statusBar);
            this.Name = "SuPlazaPOS";
            this.Text = "Super Plaza Reforma de Actopan, S.A. De C.V. :: POS v1.0";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SuPlazaPOS_FormClosing);
            this.Load += new System.EventHandler(this.SuPlazaPOS_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.F1_KeyPress);
            this.statusBar.ResumeLayout(false);
            this.statusBar.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panelCambio.ResumeLayout(false);
            this.splitContainer3.Panel1.ResumeLayout(false);
            this.splitContainer3.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer3)).EndInit();
            this.splitContainer3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        private readonly IVentaBusiness IventaBusiness;
        private readonly VentaBusiness ventaBusiness;
        #endregion



        #region Metodo utilitario para Rabbit MQ Venta Devolucion
        public VentaDevolucionDM GetVentaDevolucion(SuPlazaPOS35.domain.venta_devolucion venta_Devolucion)
        {
            IVentaDevolucionArticuloBusiness devolucionArticuloBusiness = new VentaDevolucionArticuloBusiness();
            List<VentaDevolucionArticuloDM> Articulos = new List<VentaDevolucionArticuloDM> { };

            foreach (var a in devolucionArticuloBusiness.GetSalePurseByIdSale(venta_Devolucion.id_devolucion))
            {
                VentaDevolucionArticuloDM ventaDevolucion = new VentaDevolucionArticuloDM();
                ventaDevolucion.IdDevolucion = a.IdDevolucion != null ? a.IdDevolucion : Guid.Empty;
                ventaDevolucion.NoArticulo = a.NoArticulo;
                ventaDevolucion.CodBarras = a.CodBarras;
                ventaDevolucion.Cantidad = a.Cantidad;
                Articulos.Add(ventaDevolucion);
            }

            VentaDevolucionDM ventaDM = new VentaDevolucionDM
            {
                IdDevolucion = venta_Devolucion.id_devolucion != null ? venta_Devolucion.id_devolucion : Guid.Empty,
                Folio = venta_Devolucion.folio,
                IdPos = venta_Devolucion.id_pos,
                IdVenta = venta_Devolucion.id_venta != null ? venta_Devolucion.id_venta : Guid.Empty,
                FechaDevolucion = venta_Devolucion.fecha_dev,
                Vendedor = venta_Devolucion.vendedor,
                Supervisor = venta_Devolucion.supervisor,
                CantidadDevuelta = venta_Devolucion.cant_dev,
                Impuestos = venta_Devolucion.impuestos,
                Descuento = venta_Devolucion.descuento,
                 Upload = true,
                Articulos = Articulos
            };
            return ventaDM;
        }
        #endregion

    }
}
