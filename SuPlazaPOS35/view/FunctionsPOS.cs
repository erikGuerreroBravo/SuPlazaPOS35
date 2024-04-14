using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.SqlClient;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DsiCodeTech.Builder.Generic;
using DsiCodeTech.Business;
using DsiCodeTech.Business.Interface;
using DsiCodeTech.Common.Util;
using SuPlazaPOS35;
using SuPlazaPOS35.controller;
using SuPlazaPOS35.DAO;
using SuPlazaPOS35.domain;
using SuPlazaPOS35.DomainServer;
using SuPlazaPOS35.model;
using SuPlazaPOS35.view;

namespace SuPlazaPOS35.view
{
    public class FunctionsPOS : Form
    {
        public enum Options
        {
            Buscar,
            Cobrar,
            CambiarPrecio,
            Cantidad,
            Eliminar,
            PorcentLine,
            Cancelar,
            Reimprimir,
            Suspender,
            Recuperar,
            PorcentGlobal,
            CorteX,
            Devolucion,
            OpenCashDrawer,
            Verificar
        }

        private string um { get; set; }

        public string VerificarArticulo
        {
            set
            {
                verifyPrice(value);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public FunctionsPOS(Options option)
        {
            InitializeComponent();
            switch (option)
            {
                case Options.Buscar:
                    base.Size = new Size(710, 350);
                    Text = "Buscar artículo";
                    panelBuscar.Dock = DockStyle.Fill;
                    base.CancelButton = btnSalirBuscar;
                    panelBuscar.Visible = true;
                    txtFindItem.Select();
                    break;
                case Options.Cobrar:
                    //***************************************correjir*****/
                    base.Size = new Size(400, 440);  //390
                    
                    Text = "Cobrar venta";
                    panelCobrar.Dock = DockStyle.Fill;
                    base.AcceptButton = btnCobrarVta;
                    base.CancelButton = btnCancelarVta;
                    panelCobrar.Visible = true;
                    txtEfectivo.Text = DsiCodeUtil.CurrencyFormat(POS.totalVenta).Replace("$", "");
                    lblTotalVta.Text = DsiCodeUtil.CurrencyFormat(POS.totalVenta).Replace("$", "");
                    lblCambio.Text = 0.0.ToString("F2");
                    txtEfectivo.SelectAll();
                    break;
                case Options.Verificar:
                    base.Size = new Size(620, 270);
                    Text = "Verificador de precios";
                    panelVerificar.Dock = DockStyle.Fill;
                    base.CancelButton = btnExitVerify;
                    panelVerificar.Visible = true;
                    break;
                case Options.CambiarPrecio:
                    base.Size = new Size(334, 240);
                    Text = "Cambiar de precio";
                    panelCambioPrecio.Dock = DockStyle.Fill;
                    base.AcceptButton = btnCambiarPrecio;
                    base.CancelButton = btnCancelarCambio;
                    panelCambioPrecio.Visible = true;
                    break;
                case Options.Cantidad:
                    base.Size = new Size(260, 200);
                    Text = "Cambiar cantidad";
                    panelCambiarCantidad.Dock = DockStyle.Fill;
                    base.AcceptButton = btnCambiarCantidadOK;
                    base.CancelButton = btnCancelarCantidad;
                    panelCambiarCantidad.Visible = true;
                    if (SuPlazaPOS.statusModeOperation.CompareTo(SuPlazaPOS.modeOperation.Devolution) == 0)
                    {
                        txtCantidad.Text = SuPlazaPOS.changeQuality.ToString();
                    }
                    break;
                case Options.PorcentLine:
                    base.Size = new Size(260, 200);
                    Text = "Descuento en partida";
                    panelDescuentoOnline.Dock = DockStyle.Fill;
                    base.AcceptButton = btnDescuentoOnline;
                    base.CancelButton = btnCancelDescuentoOnline;
                    panelDescuentoOnline.Visible = true;
                    break;
                case Options.PorcentGlobal:
                    base.Size = new Size(260, 200);
                    Text = "Descuento Global";
                    panelDescuentoGlobal.Dock = DockStyle.Fill;
                    base.AcceptButton = btnDescuentoGlobal;
                    base.CancelButton = btnCancelDescuentoGlobal;
                    panelDescuentoGlobal.Visible = true;
                    break;
                case Options.Reimprimir:
                    base.Size = new Size(260, 200);
                    Text = "Reimpresión de Ticket";
                    panelReimprimir.Dock = DockStyle.Fill;
                    base.AcceptButton = btnReimprimirOK;
                    base.CancelButton = btnReimprimirExit;
                    panelReimprimir.Visible = true;
                    break;
                case Options.Recuperar:
                    base.Size = new Size(400, 310);
                    Text = "Recuperar venta suspendida";
                    panelRecuperarVta.Dock = DockStyle.Fill;
                    base.AcceptButton = btnRecuperarOK;
                    base.CancelButton = btnRecuperarCancel;
                    panelRecuperarVta.Visible = true;
                    getSalesSuspended();
                    break;
                case Options.Devolucion:
                    base.Size = new Size(260, 200);
                    Text = "Devolución de Venta";
                    panelDevolucionVta.Dock = DockStyle.Fill;
                    base.AcceptButton = btnDevolucionOK;
                    base.CancelButton = btnDevolucionCancel;
                    panelDevolucionVta.Visible = true;
                    break;
                case Options.Eliminar:
                case Options.Cancelar:
                case Options.Suspender:
                case Options.CorteX:
                case Options.OpenCashDrawer:
                    break;
            }
        }

        public FunctionsPOS(Options option, string unidad)
            : this(option)
        {
            um = unidad;
        }

        private void getSalesSuspended()
        {
            listVtaSuspendidas.Items.Clear();
            List<SuPlazaPOS35.model.venta_cancelada> ventasSuspendidas = new SuspencionDAO().getVentasSuspendidas();
            btnRecuperarOK.Enabled = ventasSuspendidas != null;
            if (ventasSuspendidas != null)
            {
                listVtaSuspendidas.Items.AddRange(ventasSuspendidas.Select((a) => new ListViewItem(new string[2]
                {
                a.id_venta_cancel.ToString(),
                a.fecha.ToString()
                }.ToArray())).ToArray());
            }
        }

        private void FunctionsPOS_Load(object sender, EventArgs e)
        {
            SuPlazaPOS.codigoBarras = null;
        }

        private void txtFindItem_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                Keys keyCode = e.KeyCode;
                if (keyCode != Keys.Return)
                {
                    return;
                }
                listProducts.Items.Clear();
                string input = txtFindItem.Text.Trim();
                if (input.Length > 0)
                {
                    using (DataClassesPOSDataContext dataClassesPOSDataContext = new DataClassesPOSDataContext())
                    {
                        var list = (from a in dataClassesPOSDataContext.articulo
                                    where a.tipo_articulo != "asociado" && SqlMethods.Like(a.descripcion, $"%{input}%")
                                    select new { a.cod_barras, a.cod_interno, a.descripcion, a.unidad_medida, a.precio_venta }).ToList();
                        if (list.Count > 0)
                        {
                            listProducts.Items.Clear();
                            foreach (var item in list)
                            {
                                listProducts.Items.Add(new ListViewItem(new string[5]
                                {
                                item.cod_barras,
                                item.cod_interno,
                                item.descripcion,
                                item.unidad_medida.descripcion,
                                item.precio_venta.ToString("C2")
                                }));
                            }
                            listProducts.Focus();
                            listProducts.Items[0].Selected = true;
                            return;
                        }
                        txtFindItem.SelectAll();
                        throw new Exception("Artículo no encontrado");
                    }
                }
                throw new Exception("Ingrese la descripción del artículo para iniciar la búsqueda");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Buscar artículo", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void listProducts_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return && listProducts.SelectedItems.Count > 0)
            {
                SuPlazaPOS.codigoBarras = listProducts.SelectedItems[0].Text;
                SuccessExit();
            }
            else if (e.KeyCode == Keys.F1)
            {
                txtFindItem.SelectAll();
            }
        }

        private void btnCambiarPrecio_Click(object sender, EventArgs e)
        {
            try
            {
                SuPlazaPOS.changePrice = decimal.Parse(txtNewPrice.Text);
                SuccessExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cambio de Precio", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtCantidad.SelectAll();
            }
        }

        private void btnCambiarCantidadOK_Click(object sender, EventArgs e)
        {
            try
            {
                if (decimal.Parse(txtCantidad.Text) > 1000000m && um.Equals("Kg"))
                {
                    throw new Exception("La cantidad excede la cantidad de Kg");
                }
                if (decimal.Parse(txtCantidad.Text) > 1000000m && !um.Equals("Kg"))
                {
                    throw new Exception("La cantidad excede la cantidad de Piezas");
                }
                if (decimal.Parse(txtCantidad.Text) == 0m && um.Equals("Kg"))
                {
                    throw new Exception("No Puedes Poner Cantidad CERO");
                }
                if (decimal.Parse(txtCantidad.Text) == 0m && !um.Equals("Kg"))
                {
                    throw new Exception("No Puedes Poner Cantidad CERO");
                }
                if (SuPlazaPOS.statusModeOperation.CompareTo(SuPlazaPOS.modeOperation.Devolution) != 0)
                {
                    SuPlazaPOS.changeQuality = decimal.Parse(txtCantidad.Text);
                }
                else
                {
                    if (!(SuPlazaPOS.changeQuality >= decimal.Parse(txtCantidad.Text)))
                    {
                        MessageBox.Show("La cantidad a devolver excede la cantidad vendida", "Devoluciones", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        return;
                    }
                    SuPlazaPOS.changeQuality = decimal.Parse(txtCantidad.Text);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cambio de Cantidad", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtCantidad.SelectAll();
                return;
            }
            SuccessExit();
        }

        private void txtVerifyItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                verifyPrice(txtVerifyItem.Text);
            }
        }

        protected virtual void verifyPrice(string codigoBarras)
        {
            try
            {
                domain.articulo articulo = new domain.articulo();
                articulo=AutoMapper.Mapper.Map(Builder<ArticuloBusiness>.New.Create().GetArticleByBarCode(codigoBarras), articulo);
                
                #region Codigo Anterior
                //articulo articulo = POS.findItemByCode(codigoBarras);
                #endregion

                if (articulo == null)
                {
                    throw new Exception("El artículo no existe");
                }
                DevicesOPOS.showMessageDisplay(articulo.descripcion, "Precio: " + articulo.precio_venta.ToString("C2"));
                txtVerifyItem.Text = articulo.cod_barras;
                lblDescripcionArticulo.Text = articulo.descripcion;
                lblPrecioArticulo.Text = articulo.precio_venta.ToString("C2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtVerifyItem.Text = "";
                lblDescripcionArticulo.Text = "";
                lblPrecioArticulo.Text = "";
            }
            finally
            {
                txtVerifyItem.SelectAll();
            }
        }

        #region validacion del porcentaje de descuento 
        private void btnDescuentoOnline_Click(object sender, EventArgs e)
        {
            SuPlazaPOS.changePorcent = decimal.Parse(txtPorcentLine.Text) / 100m;
            SuccessExit();
        }
        #endregion
        private void btnDescuentoGlobal_Click(object sender, EventArgs e)
        {
            SuPlazaPOS.changePorcent = decimal.Parse(txtDescuentoGlobal.Text) / 100m;
            SuccessExit();
        }

        private void SuccessExit()
        {
            base.DialogResult = DialogResult.OK;
            Dispose();
        }

        private void acceptDecimalNumber_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (um.Equals("Kg") || um.Equals("Gms"))
            {
                e.Handled = !char.IsNumber(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b';
            }
            else
            {
                e.Handled = !char.IsNumber(e.KeyChar) && e.KeyChar != '\b';
            }
        }

        private void acceptDecimalPrices_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool flag2 = (e.Handled = !char.IsNumber(e.KeyChar) && e.KeyChar != '.' && e.KeyChar != '\b');
        }

        private void txtPayType_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                switch (e.KeyCode)
                {
                    case Keys.F1:
                        txtEfectivo.Focus();
                        txtEfectivo.SelectAll();
                        break;
                    case Keys.F2:
                        txtSpei.Focus();
                        txtSpei.SelectAll();
                        break;
                    case Keys.F3:
                        txtVales.Focus();
                        txtVales.SelectAll();
                        break;
                    case Keys.F4:
                        txtTC.Focus();
                        txtTC.SelectAll();
                        break;
                    case Keys.F5:
                        txtDebito.Focus();
                        txtDebito.SelectAll();
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }



        private void txtPayType_TextChanged(object sender, EventArgs e)
        {
            try
            {
                POS.tp_efectivo = ((txtEfectivo.Text.Trim().Length > 0) ? decimal.Parse(txtEfectivo.Text) : 0m);
                ///se elimina el tp_cheque por que ya no se esatra habilitando
                POS.tp_cheque = ((txtSpei.Text.Trim().Length > 0) ? decimal.Parse(txtSpei.Text) : 0m);
                
                POS.tp_vales = ((txtVales.Text.Trim().Length > 0) ? decimal.Parse(txtVales.Text) : 0m);
                POS.tp_creditCard = ((txtTC.Text.Trim().Length > 0) ? decimal.Parse(txtTC.Text) : 0m);
                
                ///se agrega el calculo para tp_debitoCard
                POS.tp_debitoCard = ((txtDebito.Text.Trim().Length > 0) ? decimal.Parse(txtDebito.Text) : 0m );
                //se agrega el calculo para tp_spei esta funcion estara bloqueada hasta que se defina comose trabajara
                POS.tp_spei = ((txtSpei.Text.Trim().Length > 0) ? decimal.Parse(txtSpei.Text) : 0m);


                ///Aqui se manda llamar la funcion getCambio con las nuevas actualizaciones de agregado spei y tarjeta de debito
                decimal cambio = POS.getCambio(decimal.Parse(lblTotalVta.Text));
                btnCobrarVta.Enabled = cambio >= 0m;
                lblCambio.Text = cambio.ToString("C2");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void btnCobrarVta_Click(object sender, EventArgs e)
        {
            try
            {
                if (!(POS.getCambio(decimal.Parse(lblTotalVta.Text)) < 1000000m))
                {
                    throw new Exception("Verifique nuevamente el monto a cobrar");
                }
                SuccessExit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cobro de Venta", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                txtEfectivo.SelectAll();
            }
        }

        private void btnReimprimirOK_Click(object sender, EventArgs e)
        {
            SuPlazaPOS.ticketNumber = long.Parse(txtNumTicket.Text);
            SuccessExit();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            SuPlazaPOS.codigoBarras = null;
            SuPlazaPOS.ticketNumber = 0L;
            SuPlazaPOS.statusModeOperation = SuPlazaPOS.modeOperation.Sale;
            SuPlazaPOS.changePrice = 0m;
            POS.sale = null;
            base.DialogResult = DialogResult.Cancel;
            Dispose();
        }

        private void btnRecuperarOK_Click(object sender, EventArgs e)
        {
            if (listVtaSuspendidas.SelectedItems.Count > 0)
            {
                int index = listVtaSuspendidas.Items.IndexOf(listVtaSuspendidas.SelectedItems[0]);
                POS.SuspendedSale = new SuspencionDAO().getVentaSuspendida(new Guid(listVtaSuspendidas.Items[index].SubItems[0].Text));
                POS.SaleRecovery = POS.SuspendedSale != null;
                SuccessExit();
            }
        }

        private void btnDevolucionOK_Click(object sender, EventArgs e)
        {
            try
            {
                POS.sale = POS.saleThere(int.Parse(txtNumTktDevolucion.Text));
                if (POS.sale == null)
                {
                    MessageBox.Show("El Ticket indicado no existe.", "Devoluciones", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else if (POS.sale.venta_articulo.Count <= 0)
                {
                    MessageBox.Show("La venta ya no tiene productos a devolver", "Devoluciones", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                else
                {
                    SuccessExit();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Devoluciones", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void txtPorcentLine_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Obtener el valor actual del TextBox
            string currentValue = txtPorcentLine.Text;

            // Verificar si el carácter presionado es un dígito o un punto decimal
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true; // Ignorar el carácter ingresado
                return;
            }

            // Verificar si el carácter presionado es un punto decimal y si ya hay uno en el valor actual
            if (e.KeyChar == '.' && currentValue.Contains("."))
            {
                e.Handled = true; // Ignorar el carácter ingresado
                return;
            }

            // Verificar si el valor actual ya contiene 3 dígitos después del punto decimal
           int decimalIndex = currentValue.IndexOf(".");

            int size = currentValue.Length;

            //Para cuando no hay punto decimal y hay tres caracteres
            if (size == 3 && decimalIndex == -1 && e.KeyChar != '\b')
            {
                e.Handled = true; // Ignorar el carácter ingresado
            }


            if (size == 5 && decimalIndex > -1 && e.KeyChar != '\b')
            {
                e.Handled = true; // Ignorar el carácter ingresado
                return;
            }
        }

        #region componentes
        private IContainer components;

        private Panel panelCobrar;

        private Label label1;

        private TextBox txtTC;

        private Label label4;

        private TextBox txtVales;

        private Label label3;

        private TextBox txtSpei;

        private Label label2;

        private TextBox txtEfectivo;

        //tarjeta de debito
        private TextBox txtDebito;

        //label de debito
        private Label lblTajertaDebito;

         private Label lblCambio;

        private Label label7;

        private Label lblTotalVta;

        private Label label5;

        private GroupBox groupBox1;

        private Button btnCobrarVta;

        private Button btnCancelarVta;

        private SplitContainer panelBuscar;

        private ColumnHeader codigo;

        private ColumnHeader descripcion;

        private ColumnHeader unidad;

        private ColumnHeader precio;

        private GroupBox groupBox2;

        public TextBox txtFindItem;

        private Button btnSalirBuscar;

        private Panel panelCambioPrecio;

        private GroupBox groupBox3;

        private Button btnCambiarPrecio;

        private Button btnCancelarCambio;

        private TextBox txtNewPrice;

        private Panel panelCambiarCantidad;

        private Button btnCancelarCantidad;

        private GroupBox groupBox5;

        private Panel panelDescuentoOnline;

        private Button btnDescuentoOnline;

        private Button btnCancelDescuentoOnline;

        private GroupBox groupBox6;

        private TextBox txtPorcentLine;

        private Label label6;

        private Panel panelDescuentoGlobal;

        private Button btnDescuentoGlobal;

        private Button btnCancelDescuentoGlobal;

        private GroupBox groupBox7;

        private Label label8;

        private TextBox txtDescuentoGlobal;

        private Button btnCambiarCantidadOK;

        private Button btnExitVerify;

        private GroupBox groupBox4;

        private Label label9;

        private Label label10;

        private ListView listProducts;

        private Panel panelVerificar;

        private TextBox txtCantidad;
        
        private TextBox txtVerifyItem;

        private Label lblPrecioArticulo;

        private Label lblDescripcionArticulo;

        private Panel panelReimprimir;

        private Button btnReimprimirOK;

        private Button btnReimprimirExit;

        private GroupBox groupBox8;

        private TextBox txtNumTicket;

        private SplitContainer panelRecuperarVta;

        private ListView listVtaSuspendidas;

        private ColumnHeader columnHeader1;

        private ColumnHeader columnHeader2;

        private Button btnRecuperarCancel;

        private Button btnRecuperarOK;

        private Panel panelDevolucionVta;

        private Button btnDevolucionOK;

        private Button btnDevolucionCancel;

        private GroupBox groupBox9;

        private TextBox txtNumTktDevolucion;

        private ColumnHeader internalCode;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FunctionsPOS));
            this.panelCobrar = new System.Windows.Forms.Panel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnCancelarVta = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnCobrarVta = new System.Windows.Forms.Button();
            this.txtTC = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.lblTotalVta = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.lblCambio = new System.Windows.Forms.Label();
            this.txtEfectivo = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtVales = new System.Windows.Forms.TextBox();
            this.txtSpei = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtDebito = new System.Windows.Forms.TextBox();
            this.lblTajertaDebito = new System.Windows.Forms.Label();
            this.panelBuscar = new System.Windows.Forms.SplitContainer();
            this.listProducts = new System.Windows.Forms.ListView();
            this.codigo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.internalCode = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.descripcion = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.unidad = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.precio = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnSalirBuscar = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtFindItem = new System.Windows.Forms.TextBox();
            this.panelCambioPrecio = new System.Windows.Forms.Panel();
            this.btnCancelarCambio = new System.Windows.Forms.Button();
            this.btnCambiarPrecio = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtNewPrice = new System.Windows.Forms.TextBox();
            this.panelCambiarCantidad = new System.Windows.Forms.Panel();
            this.btnCambiarCantidadOK = new System.Windows.Forms.Button();
            this.btnCancelarCantidad = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.txtCantidad = new System.Windows.Forms.TextBox();
            this.panelDescuentoOnline = new System.Windows.Forms.Panel();
            this.btnDescuentoOnline = new System.Windows.Forms.Button();
            this.btnCancelDescuentoOnline = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtPorcentLine = new System.Windows.Forms.TextBox();
            this.panelDescuentoGlobal = new System.Windows.Forms.Panel();
            this.btnDescuentoGlobal = new System.Windows.Forms.Button();
            this.btnCancelDescuentoGlobal = new System.Windows.Forms.Button();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtDescuentoGlobal = new System.Windows.Forms.TextBox();
            this.panelVerificar = new System.Windows.Forms.Panel();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.lblPrecioArticulo = new System.Windows.Forms.Label();
            this.lblDescripcionArticulo = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.txtVerifyItem = new System.Windows.Forms.TextBox();
            this.btnExitVerify = new System.Windows.Forms.Button();
            this.panelReimprimir = new System.Windows.Forms.Panel();
            this.btnReimprimirOK = new System.Windows.Forms.Button();
            this.btnReimprimirExit = new System.Windows.Forms.Button();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.txtNumTicket = new System.Windows.Forms.TextBox();
            this.panelRecuperarVta = new System.Windows.Forms.SplitContainer();
            this.listVtaSuspendidas = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnRecuperarOK = new System.Windows.Forms.Button();
            this.btnRecuperarCancel = new System.Windows.Forms.Button();
            this.panelDevolucionVta = new System.Windows.Forms.Panel();
            this.btnDevolucionOK = new System.Windows.Forms.Button();
            this.btnDevolucionCancel = new System.Windows.Forms.Button();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.txtNumTktDevolucion = new System.Windows.Forms.TextBox();
            this.panelCobrar.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelBuscar)).BeginInit();
            this.panelBuscar.Panel1.SuspendLayout();
            this.panelBuscar.Panel2.SuspendLayout();
            this.panelBuscar.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panelCambioPrecio.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.panelCambiarCantidad.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.panelDescuentoOnline.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.panelDescuentoGlobal.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.panelVerificar.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.panelReimprimir.SuspendLayout();
            this.groupBox8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelRecuperarVta)).BeginInit();
            this.panelRecuperarVta.Panel1.SuspendLayout();
            this.panelRecuperarVta.Panel2.SuspendLayout();
            this.panelRecuperarVta.SuspendLayout();
            this.panelDevolucionVta.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelCobrar
            // 
            this.panelCobrar.Controls.Add(this.groupBox1);
            this.panelCobrar.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelCobrar.Location = new System.Drawing.Point(426, 158);
            this.panelCobrar.Name = "panelCobrar";
            this.panelCobrar.Size = new System.Drawing.Size(376, 259);
            this.panelCobrar.TabIndex = 0;
            this.panelCobrar.TabStop = true;
            this.panelCobrar.Visible = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnCancelarVta);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnCobrarVta);
            this.groupBox1.Controls.Add(this.txtTC);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.lblTotalVta);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.lblCambio);
            this.groupBox1.Controls.Add(this.txtEfectivo);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtVales);
            this.groupBox1.Controls.Add(this.txtSpei);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtDebito);
            this.groupBox1.Controls.Add(this.lblTajertaDebito);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold);
            this.groupBox1.Location = new System.Drawing.Point(11, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(355, 358);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Forma de pago:";
            // 
            // btnCancelarVta
            // 
            this.btnCancelarVta.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancelarVta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelarVta.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelarVta.Image")));
            this.btnCancelarVta.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelarVta.Location = new System.Drawing.Point(247, 259);
            this.btnCancelarVta.Name = "btnCancelarVta";
            this.btnCancelarVta.Size = new System.Drawing.Size(82, 72);
            this.btnCancelarVta.TabIndex = 6;
            this.btnCancelarVta.Text = "Ca&ncelar cobro";
            this.btnCancelarVta.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelarVta.UseVisualStyleBackColor = false;
            this.btnCancelarVta.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(17, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(134, 32);
            this.label1.TabIndex = 0;
            this.label1.Text = "Efectivo [F1]:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnCobrarVta
            // 
            this.btnCobrarVta.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCobrarVta.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCobrarVta.Image = ((System.Drawing.Image)(resources.GetObject("btnCobrarVta.Image")));
            this.btnCobrarVta.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCobrarVta.Location = new System.Drawing.Point(125, 259);
            this.btnCobrarVta.Name = "btnCobrarVta";
            this.btnCobrarVta.Size = new System.Drawing.Size(82, 72);
            this.btnCobrarVta.TabIndex = 5;
            this.btnCobrarVta.Text = "&Cobrar venta";
            this.btnCobrarVta.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCobrarVta.UseVisualStyleBackColor = false;
            this.btnCobrarVta.Click += new System.EventHandler(this.btnCobrarVta_Click);
            // 
            // txtTC
            // 
            this.txtTC.Location = new System.Drawing.Point(157, 133);
            this.txtTC.MaxLength = 10;
            this.txtTC.Name = "txtTC";
            this.txtTC.Size = new System.Drawing.Size(176, 26);
            this.txtTC.TabIndex = 9;
            this.txtTC.Text = "0.00";
            this.txtTC.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtTC.TextChanged += new System.EventHandler(this.txtPayType_TextChanged);
            this.txtTC.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPayType_KeyDown);
            this.txtTC.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.acceptDecimalPrices_KeyPress);
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(5, 200);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(160, 32);
            this.label5.TabIndex = 0;
            this.label5.Text = "Total de venta:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblTotalVta
            // 
            this.lblTotalVta.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotalVta.Location = new System.Drawing.Point(187, 200);
            this.lblTotalVta.Name = "lblTotalVta";
            this.lblTotalVta.Size = new System.Drawing.Size(151, 32);
            this.lblTotalVta.TabIndex = 0;
            this.lblTotalVta.Text = "0.00";
            this.lblTotalVta.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label7
            // 
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(34, 229);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(91, 32);
            this.label7.TabIndex = 0;
            this.label7.Text = "Cambio:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblCambio
            // 
            this.lblCambio.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCambio.Location = new System.Drawing.Point(161, 229);
            this.lblCambio.Name = "lblCambio";
            this.lblCambio.Size = new System.Drawing.Size(176, 27);
            this.lblCambio.TabIndex = 0;
            this.lblCambio.Text = "0.00";
            this.lblCambio.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtEfectivo
            // 
            this.txtEfectivo.Location = new System.Drawing.Point(157, 28);
            this.txtEfectivo.MaxLength = 10;
            this.txtEfectivo.Name = "txtEfectivo";
            this.txtEfectivo.Size = new System.Drawing.Size(176, 26);
            this.txtEfectivo.TabIndex = 1;
            this.txtEfectivo.Text = "0.00";
            this.txtEfectivo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtEfectivo.TextChanged += new System.EventHandler(this.txtPayType_TextChanged);
            this.txtEfectivo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPayType_KeyDown);
            this.txtEfectivo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.acceptDecimalPrices_KeyPress);
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(17, 130);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(134, 32);
            this.label4.TabIndex = 0;
            this.label4.Text = "T.C. [F4]:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(17, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(134, 32);
            this.label2.TabIndex = 0;
            this.label2.Text = "SPEI [F2]:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtVales
            // 
            this.txtVales.Location = new System.Drawing.Point(157, 98);
            this.txtVales.MaxLength = 10;
            this.txtVales.Name = "txtVales";
            this.txtVales.Size = new System.Drawing.Size(176, 26);
            this.txtVales.TabIndex = 3;
            this.txtVales.Text = "0.00";
            this.txtVales.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtVales.TextChanged += new System.EventHandler(this.txtPayType_TextChanged);
            this.txtVales.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPayType_KeyDown);
            this.txtVales.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.acceptDecimalPrices_KeyPress);
            // 
            // txtSpei
            // 
            this.txtSpei.Location = new System.Drawing.Point(157, 63);
            this.txtSpei.MaxLength = 10;
            this.txtSpei.Name = "txtSpei";
            this.txtSpei.Size = new System.Drawing.Size(176, 26);
            this.txtSpei.TabIndex = 2;
            this.txtSpei.Text = "0.00";
            this.txtSpei.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtSpei.TextChanged += new System.EventHandler(this.txtPayType_TextChanged);
            this.txtSpei.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtPayType_KeyDown);
            this.txtSpei.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.acceptDecimalPrices_KeyPress);
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(17, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(134, 32);
            this.label3.TabIndex = 0;
            this.label3.Text = "Vales [F3]:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // txtDebito
            // 
            this.txtDebito.Location = new System.Drawing.Point(158, 165);
            this.txtDebito.MaxLength = 10;
            this.txtDebito.Name = "txtDebito";
            this.txtDebito.Size = new System.Drawing.Size(176, 26);
            this.txtDebito.TabIndex = 10;
            this.txtDebito.Text = "0.00";
            this.txtDebito.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDebito.TextChanged += new System.EventHandler(this.txtPayType_TextChanged);
            // 
            // lblTajertaDebito
            // 
            this.lblTajertaDebito.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTajertaDebito.Location = new System.Drawing.Point(18, 165);
            this.lblTajertaDebito.Name = "lblTajertaDebito";
            this.lblTajertaDebito.Size = new System.Drawing.Size(134, 32);
            this.lblTajertaDebito.TabIndex = 0;
            this.lblTajertaDebito.Text = "T.Debito [F5]:";
            this.lblTajertaDebito.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // panelBuscar
            // 
            this.panelBuscar.Location = new System.Drawing.Point(26, 12);
            this.panelBuscar.Name = "panelBuscar";
            this.panelBuscar.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // panelBuscar.Panel1
            // 
            this.panelBuscar.Panel1.Controls.Add(this.listProducts);
            // 
            // panelBuscar.Panel2
            // 
            this.panelBuscar.Panel2.Controls.Add(this.btnSalirBuscar);
            this.panelBuscar.Panel2.Controls.Add(this.groupBox2);
            this.panelBuscar.Size = new System.Drawing.Size(105, 74);
            this.panelBuscar.SplitterDistance = 42;
            this.panelBuscar.TabIndex = 0;
            this.panelBuscar.Visible = false;
            // 
            // listProducts
            // 
            this.listProducts.BackColor = System.Drawing.Color.White;
            this.listProducts.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.codigo,
            this.internalCode,
            this.descripcion,
            this.unidad,
            this.precio});
            this.listProducts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listProducts.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listProducts.FullRowSelect = true;
            this.listProducts.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listProducts.HideSelection = false;
            this.listProducts.Location = new System.Drawing.Point(0, 0);
            this.listProducts.MultiSelect = false;
            this.listProducts.Name = "listProducts";
            this.listProducts.Size = new System.Drawing.Size(105, 42);
            this.listProducts.TabIndex = 0;
            this.listProducts.UseCompatibleStateImageBehavior = false;
            this.listProducts.View = System.Windows.Forms.View.Details;
            this.listProducts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listProducts_KeyDown);
            // 
            // codigo
            // 
            this.codigo.Text = "Código";
            this.codigo.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.codigo.Width = 130;
            // 
            // internalCode
            // 
            this.internalCode.Text = "Cod. Int.";
            // 
            // descripcion
            // 
            this.descripcion.Text = "Descripción";
            this.descripcion.Width = 340;
            // 
            // unidad
            // 
            this.unidad.Text = "Unidad";
            this.unidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.unidad.Width = 70;
            // 
            // precio
            // 
            this.precio.Text = "Precio";
            this.precio.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.precio.Width = 70;
            // 
            // btnSalirBuscar
            // 
            this.btnSalirBuscar.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSalirBuscar.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnSalirBuscar.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalirBuscar.Image = ((System.Drawing.Image)(resources.GetObject("btnSalirBuscar.Image")));
            this.btnSalirBuscar.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSalirBuscar.Location = new System.Drawing.Point(566, 20);
            this.btnSalirBuscar.Name = "btnSalirBuscar";
            this.btnSalirBuscar.Size = new System.Drawing.Size(74, 52);
            this.btnSalirBuscar.TabIndex = 4;
            this.btnSalirBuscar.Text = "Salir";
            this.btnSalirBuscar.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSalirBuscar.UseVisualStyleBackColor = false;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtFindItem);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(434, 69);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Ingresar descripción del artículo:";
            // 
            // txtFindItem
            // 
            this.txtFindItem.Location = new System.Drawing.Point(7, 26);
            this.txtFindItem.Name = "txtFindItem";
            this.txtFindItem.Size = new System.Drawing.Size(421, 26);
            this.txtFindItem.TabIndex = 3;
            this.txtFindItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtFindItem_KeyDown);
            // 
            // panelCambioPrecio
            // 
            this.panelCambioPrecio.Controls.Add(this.btnCancelarCambio);
            this.panelCambioPrecio.Controls.Add(this.btnCambiarPrecio);
            this.panelCambioPrecio.Controls.Add(this.groupBox3);
            this.panelCambioPrecio.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelCambioPrecio.Location = new System.Drawing.Point(210, 12);
            this.panelCambioPrecio.Name = "panelCambioPrecio";
            this.panelCambioPrecio.Size = new System.Drawing.Size(102, 66);
            this.panelCambioPrecio.TabIndex = 0;
            this.panelCambioPrecio.Visible = false;
            // 
            // btnCancelarCambio
            // 
            this.btnCancelarCambio.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancelarCambio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelarCambio.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelarCambio.Image")));
            this.btnCancelarCambio.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelarCambio.Location = new System.Drawing.Point(228, 122);
            this.btnCancelarCambio.Name = "btnCancelarCambio";
            this.btnCancelarCambio.Size = new System.Drawing.Size(75, 68);
            this.btnCancelarCambio.TabIndex = 3;
            this.btnCancelarCambio.Text = "Ca&ncelar Cambio";
            this.btnCancelarCambio.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelarCambio.UseVisualStyleBackColor = false;
            this.btnCancelarCambio.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // btnCambiarPrecio
            // 
            this.btnCambiarPrecio.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCambiarPrecio.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCambiarPrecio.Image = ((System.Drawing.Image)(resources.GetObject("btnCambiarPrecio.Image")));
            this.btnCambiarPrecio.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCambiarPrecio.Location = new System.Drawing.Point(147, 122);
            this.btnCambiarPrecio.Name = "btnCambiarPrecio";
            this.btnCambiarPrecio.Size = new System.Drawing.Size(75, 68);
            this.btnCambiarPrecio.TabIndex = 2;
            this.btnCambiarPrecio.Text = "Cambiar &Precio";
            this.btnCambiarPrecio.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCambiarPrecio.UseVisualStyleBackColor = false;
            this.btnCambiarPrecio.Click += new System.EventHandler(this.btnCambiarPrecio_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtNewPrice);
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(291, 100);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Precio:";
            // 
            // txtNewPrice
            // 
            this.txtNewPrice.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNewPrice.Location = new System.Drawing.Point(6, 37);
            this.txtNewPrice.Name = "txtNewPrice";
            this.txtNewPrice.Size = new System.Drawing.Size(279, 29);
            this.txtNewPrice.TabIndex = 1;
            this.txtNewPrice.Text = "0.00";
            this.txtNewPrice.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtNewPrice.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.acceptDecimalPrices_KeyPress);
            // 
            // panelCambiarCantidad
            // 
            this.panelCambiarCantidad.Controls.Add(this.btnCambiarCantidadOK);
            this.panelCambiarCantidad.Controls.Add(this.btnCancelarCantidad);
            this.panelCambiarCantidad.Controls.Add(this.groupBox5);
            this.panelCambiarCantidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelCambiarCantidad.Location = new System.Drawing.Point(318, 11);
            this.panelCambiarCantidad.Name = "panelCambiarCantidad";
            this.panelCambiarCantidad.Size = new System.Drawing.Size(104, 70);
            this.panelCambiarCantidad.TabIndex = 0;
            this.panelCambiarCantidad.Visible = false;
            // 
            // btnCambiarCantidadOK
            // 
            this.btnCambiarCantidadOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCambiarCantidadOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCambiarCantidadOK.Image = ((System.Drawing.Image)(resources.GetObject("btnCambiarCantidadOK.Image")));
            this.btnCambiarCantidadOK.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCambiarCantidadOK.Location = new System.Drawing.Point(76, 85);
            this.btnCambiarCantidadOK.Name = "btnCambiarCantidadOK";
            this.btnCambiarCantidadOK.Size = new System.Drawing.Size(75, 68);
            this.btnCambiarCantidadOK.TabIndex = 3;
            this.btnCambiarCantidadOK.Text = "Cambiar\r\ncantidad";
            this.btnCambiarCantidadOK.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCambiarCantidadOK.UseVisualStyleBackColor = false;
            this.btnCambiarCantidadOK.Click += new System.EventHandler(this.btnCambiarCantidadOK_Click);
            // 
            // btnCancelarCantidad
            // 
            this.btnCancelarCantidad.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancelarCantidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelarCantidad.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelarCantidad.Image")));
            this.btnCancelarCantidad.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelarCantidad.Location = new System.Drawing.Point(157, 85);
            this.btnCancelarCantidad.Name = "btnCancelarCantidad";
            this.btnCancelarCantidad.Size = new System.Drawing.Size(75, 68);
            this.btnCancelarCantidad.TabIndex = 3;
            this.btnCancelarCantidad.Text = "[Esc]\r\nSalir";
            this.btnCancelarCantidad.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelarCantidad.UseVisualStyleBackColor = false;
            this.btnCancelarCantidad.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtCantidad);
            this.groupBox5.Location = new System.Drawing.Point(12, 12);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(220, 66);
            this.groupBox5.TabIndex = 0;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Ingresar cantidad:";
            // 
            // txtCantidad
            // 
            this.txtCantidad.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtCantidad.Location = new System.Drawing.Point(6, 25);
            this.txtCantidad.Name = "txtCantidad";
            this.txtCantidad.Size = new System.Drawing.Size(204, 29);
            this.txtCantidad.TabIndex = 1;
            this.txtCantidad.Text = "1";
            this.txtCantidad.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtCantidad.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.acceptDecimalNumber_KeyPress);
            // 
            // panelDescuentoOnline
            // 
            this.panelDescuentoOnline.Controls.Add(this.btnDescuentoOnline);
            this.panelDescuentoOnline.Controls.Add(this.btnCancelDescuentoOnline);
            this.panelDescuentoOnline.Controls.Add(this.groupBox6);
            this.panelDescuentoOnline.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelDescuentoOnline.Location = new System.Drawing.Point(26, 99);
            this.panelDescuentoOnline.Name = "panelDescuentoOnline";
            this.panelDescuentoOnline.Size = new System.Drawing.Size(79, 57);
            this.panelDescuentoOnline.TabIndex = 0;
            this.panelDescuentoOnline.Visible = false;
            // 
            // btnDescuentoOnline
            // 
            this.btnDescuentoOnline.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDescuentoOnline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDescuentoOnline.Image = ((System.Drawing.Image)(resources.GetObject("btnDescuentoOnline.Image")));
            this.btnDescuentoOnline.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDescuentoOnline.Location = new System.Drawing.Point(64, 84);
            this.btnDescuentoOnline.Name = "btnDescuentoOnline";
            this.btnDescuentoOnline.Size = new System.Drawing.Size(79, 68);
            this.btnDescuentoOnline.TabIndex = 2;
            this.btnDescuentoOnline.Text = "Descuento en línea";
            this.btnDescuentoOnline.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDescuentoOnline.UseVisualStyleBackColor = false;
            this.btnDescuentoOnline.Click += new System.EventHandler(this.btnDescuentoOnline_Click);
            // 
            // btnCancelDescuentoOnline
            // 
            this.btnCancelDescuentoOnline.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancelDescuentoOnline.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelDescuentoOnline.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelDescuentoOnline.Image")));
            this.btnCancelDescuentoOnline.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelDescuentoOnline.Location = new System.Drawing.Point(153, 84);
            this.btnCancelDescuentoOnline.Name = "btnCancelDescuentoOnline";
            this.btnCancelDescuentoOnline.Size = new System.Drawing.Size(79, 68);
            this.btnCancelDescuentoOnline.TabIndex = 3;
            this.btnCancelDescuentoOnline.Text = "Cancelar descuento";
            this.btnCancelDescuentoOnline.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelDescuentoOnline.UseVisualStyleBackColor = false;
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.txtPorcentLine);
            this.groupBox6.Location = new System.Drawing.Point(12, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(220, 66);
            this.groupBox6.TabIndex = 0;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Descuento:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(191, 29);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(24, 20);
            this.label6.TabIndex = 2;
            this.label6.Text = "%";
            // 
            // txtPorcentLine
            // 
            this.txtPorcentLine.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPorcentLine.Location = new System.Drawing.Point(6, 25);
            this.txtPorcentLine.Name = "txtPorcentLine";
            this.txtPorcentLine.Size = new System.Drawing.Size(183, 29);
            this.txtPorcentLine.TabIndex = 1;
            this.txtPorcentLine.Text = "0";
            this.txtPorcentLine.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtPorcentLine.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtPorcentLine_KeyPress);
            // 
            // panelDescuentoGlobal
            // 
            this.panelDescuentoGlobal.Controls.Add(this.btnDescuentoGlobal);
            this.panelDescuentoGlobal.Controls.Add(this.btnCancelDescuentoGlobal);
            this.panelDescuentoGlobal.Controls.Add(this.groupBox7);
            this.panelDescuentoGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelDescuentoGlobal.Location = new System.Drawing.Point(111, 99);
            this.panelDescuentoGlobal.Name = "panelDescuentoGlobal";
            this.panelDescuentoGlobal.Size = new System.Drawing.Size(76, 57);
            this.panelDescuentoGlobal.TabIndex = 0;
            this.panelDescuentoGlobal.Visible = false;
            // 
            // btnDescuentoGlobal
            // 
            this.btnDescuentoGlobal.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDescuentoGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDescuentoGlobal.Image = ((System.Drawing.Image)(resources.GetObject("btnDescuentoGlobal.Image")));
            this.btnDescuentoGlobal.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDescuentoGlobal.Location = new System.Drawing.Point(64, 84);
            this.btnDescuentoGlobal.Name = "btnDescuentoGlobal";
            this.btnDescuentoGlobal.Size = new System.Drawing.Size(79, 68);
            this.btnDescuentoGlobal.TabIndex = 2;
            this.btnDescuentoGlobal.Text = "Descuento global";
            this.btnDescuentoGlobal.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDescuentoGlobal.UseVisualStyleBackColor = false;
            this.btnDescuentoGlobal.Click += new System.EventHandler(this.btnDescuentoGlobal_Click);
            // 
            // btnCancelDescuentoGlobal
            // 
            this.btnCancelDescuentoGlobal.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnCancelDescuentoGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelDescuentoGlobal.Image = ((System.Drawing.Image)(resources.GetObject("btnCancelDescuentoGlobal.Image")));
            this.btnCancelDescuentoGlobal.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnCancelDescuentoGlobal.Location = new System.Drawing.Point(153, 84);
            this.btnCancelDescuentoGlobal.Name = "btnCancelDescuentoGlobal";
            this.btnCancelDescuentoGlobal.Size = new System.Drawing.Size(79, 68);
            this.btnCancelDescuentoGlobal.TabIndex = 3;
            this.btnCancelDescuentoGlobal.Text = "Cancelar descuento";
            this.btnCancelDescuentoGlobal.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnCancelDescuentoGlobal.UseVisualStyleBackColor = false;
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label8);
            this.groupBox7.Controls.Add(this.txtDescuentoGlobal);
            this.groupBox7.Location = new System.Drawing.Point(12, 12);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(220, 66);
            this.groupBox7.TabIndex = 0;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Descuento:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(191, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(24, 20);
            this.label8.TabIndex = 2;
            this.label8.Text = "%";
            // 
            // txtDescuentoGlobal
            // 
            this.txtDescuentoGlobal.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtDescuentoGlobal.Location = new System.Drawing.Point(6, 25);
            this.txtDescuentoGlobal.Name = "txtDescuentoGlobal";
            this.txtDescuentoGlobal.Size = new System.Drawing.Size(183, 29);
            this.txtDescuentoGlobal.TabIndex = 1;
            this.txtDescuentoGlobal.Text = "0";
            this.txtDescuentoGlobal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panelVerificar
            // 
            this.panelVerificar.Controls.Add(this.groupBox4);
            this.panelVerificar.Controls.Add(this.btnExitVerify);
            this.panelVerificar.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelVerificar.Location = new System.Drawing.Point(426, 12);
            this.panelVerificar.Name = "panelVerificar";
            this.panelVerificar.Size = new System.Drawing.Size(102, 66);
            this.panelVerificar.TabIndex = 0;
            this.panelVerificar.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.lblPrecioArticulo);
            this.groupBox4.Controls.Add(this.lblDescripcionArticulo);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.txtVerifyItem);
            this.groupBox4.Location = new System.Drawing.Point(12, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(580, 128);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Ingresar código de barras:";
            // 
            // lblPrecioArticulo
            // 
            this.lblPrecioArticulo.BackColor = System.Drawing.Color.White;
            this.lblPrecioArticulo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPrecioArticulo.Location = new System.Drawing.Point(88, 93);
            this.lblPrecioArticulo.Name = "lblPrecioArticulo";
            this.lblPrecioArticulo.Size = new System.Drawing.Size(224, 26);
            this.lblPrecioArticulo.TabIndex = 3;
            this.lblPrecioArticulo.Text = "$ 0.00";
            this.lblPrecioArticulo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblDescripcionArticulo
            // 
            this.lblDescripcionArticulo.BackColor = System.Drawing.Color.White;
            this.lblDescripcionArticulo.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDescripcionArticulo.Location = new System.Drawing.Point(88, 62);
            this.lblDescripcionArticulo.Name = "lblDescripcionArticulo";
            this.lblDescripcionArticulo.Size = new System.Drawing.Size(486, 26);
            this.lblDescripcionArticulo.TabIndex = 2;
            this.lblDescripcionArticulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 65);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 20);
            this.label9.TabIndex = 1;
            this.label9.Text = "Artículo:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(7, 96);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 20);
            this.label10.TabIndex = 1;
            this.label10.Text = "Precio:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // txtVerifyItem
            // 
            this.txtVerifyItem.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtVerifyItem.Location = new System.Drawing.Point(6, 28);
            this.txtVerifyItem.Name = "txtVerifyItem";
            this.txtVerifyItem.Size = new System.Drawing.Size(279, 29);
            this.txtVerifyItem.TabIndex = 1;
            this.txtVerifyItem.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtVerifyItem.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtVerifyItem_KeyDown);
            // 
            // btnExitVerify
            // 
            this.btnExitVerify.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnExitVerify.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExitVerify.Image = ((System.Drawing.Image)(resources.GetObject("btnExitVerify.Image")));
            this.btnExitVerify.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnExitVerify.Location = new System.Drawing.Point(517, 151);
            this.btnExitVerify.Name = "btnExitVerify";
            this.btnExitVerify.Size = new System.Drawing.Size(75, 68);
            this.btnExitVerify.TabIndex = 4;
            this.btnExitVerify.Text = "Ca&ncelar Cambio";
            this.btnExitVerify.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExitVerify.UseVisualStyleBackColor = false;
            this.btnExitVerify.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // panelReimprimir
            // 
            this.panelReimprimir.Controls.Add(this.btnReimprimirOK);
            this.panelReimprimir.Controls.Add(this.btnReimprimirExit);
            this.panelReimprimir.Controls.Add(this.groupBox8);
            this.panelReimprimir.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelReimprimir.Location = new System.Drawing.Point(534, 12);
            this.panelReimprimir.Name = "panelReimprimir";
            this.panelReimprimir.Size = new System.Drawing.Size(102, 66);
            this.panelReimprimir.TabIndex = 1;
            this.panelReimprimir.Visible = false;
            // 
            // btnReimprimirOK
            // 
            this.btnReimprimirOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnReimprimirOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReimprimirOK.Image = ((System.Drawing.Image)(resources.GetObject("btnReimprimirOK.Image")));
            this.btnReimprimirOK.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReimprimirOK.Location = new System.Drawing.Point(76, 85);
            this.btnReimprimirOK.Name = "btnReimprimirOK";
            this.btnReimprimirOK.Size = new System.Drawing.Size(75, 68);
            this.btnReimprimirOK.TabIndex = 3;
            this.btnReimprimirOK.Text = "Reimprimir\r\nticket";
            this.btnReimprimirOK.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReimprimirOK.UseVisualStyleBackColor = false;
            this.btnReimprimirOK.Click += new System.EventHandler(this.btnReimprimirOK_Click);
            // 
            // btnReimprimirExit
            // 
            this.btnReimprimirExit.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnReimprimirExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReimprimirExit.Image = ((System.Drawing.Image)(resources.GetObject("btnReimprimirExit.Image")));
            this.btnReimprimirExit.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnReimprimirExit.Location = new System.Drawing.Point(157, 85);
            this.btnReimprimirExit.Name = "btnReimprimirExit";
            this.btnReimprimirExit.Size = new System.Drawing.Size(75, 68);
            this.btnReimprimirExit.TabIndex = 3;
            this.btnReimprimirExit.Text = "[Esc]\r\nSalir";
            this.btnReimprimirExit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnReimprimirExit.UseVisualStyleBackColor = false;
            this.btnReimprimirExit.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // groupBox8
            // 
            this.groupBox8.Controls.Add(this.txtNumTicket);
            this.groupBox8.Location = new System.Drawing.Point(12, 12);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(220, 66);
            this.groupBox8.TabIndex = 0;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "# Ticket:";
            // 
            // txtNumTicket
            // 
            this.txtNumTicket.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumTicket.Location = new System.Drawing.Point(6, 25);
            this.txtNumTicket.Name = "txtNumTicket";
            this.txtNumTicket.Size = new System.Drawing.Size(204, 29);
            this.txtNumTicket.TabIndex = 1;
            this.txtNumTicket.Text = "0";
            this.txtNumTicket.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // panelRecuperarVta
            // 
            this.panelRecuperarVta.Location = new System.Drawing.Point(26, 183);
            this.panelRecuperarVta.Name = "panelRecuperarVta";
            this.panelRecuperarVta.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // panelRecuperarVta.Panel1
            // 
            this.panelRecuperarVta.Panel1.Controls.Add(this.listVtaSuspendidas);
            // 
            // panelRecuperarVta.Panel2
            // 
            this.panelRecuperarVta.Panel2.Controls.Add(this.btnRecuperarOK);
            this.panelRecuperarVta.Panel2.Controls.Add(this.btnRecuperarCancel);
            this.panelRecuperarVta.Size = new System.Drawing.Size(133, 111);
            this.panelRecuperarVta.SplitterDistance = 51;
            this.panelRecuperarVta.TabIndex = 2;
            this.panelRecuperarVta.Visible = false;
            // 
            // listVtaSuspendidas
            // 
            this.listVtaSuspendidas.BackColor = System.Drawing.Color.White;
            this.listVtaSuspendidas.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listVtaSuspendidas.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listVtaSuspendidas.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listVtaSuspendidas.FullRowSelect = true;
            this.listVtaSuspendidas.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listVtaSuspendidas.HideSelection = false;
            this.listVtaSuspendidas.Location = new System.Drawing.Point(0, 0);
            this.listVtaSuspendidas.MultiSelect = false;
            this.listVtaSuspendidas.Name = "listVtaSuspendidas";
            this.listVtaSuspendidas.Size = new System.Drawing.Size(133, 51);
            this.listVtaSuspendidas.TabIndex = 0;
            this.listVtaSuspendidas.UseCompatibleStateImageBehavior = false;
            this.listVtaSuspendidas.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "No. Registro";
            this.columnHeader1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader1.Width = 80;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Fecha y Hora de Suspensión";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 300;
            // 
            // btnRecuperarOK
            // 
            this.btnRecuperarOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnRecuperarOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecuperarOK.Image = ((System.Drawing.Image)(resources.GetObject("btnRecuperarOK.Image")));
            this.btnRecuperarOK.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRecuperarOK.Location = new System.Drawing.Point(82, 12);
            this.btnRecuperarOK.Name = "btnRecuperarOK";
            this.btnRecuperarOK.Size = new System.Drawing.Size(79, 68);
            this.btnRecuperarOK.TabIndex = 5;
            this.btnRecuperarOK.Text = "Recuperar\r\nventa";
            this.btnRecuperarOK.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRecuperarOK.UseVisualStyleBackColor = false;
            this.btnRecuperarOK.Click += new System.EventHandler(this.btnRecuperarOK_Click);
            // 
            // btnRecuperarCancel
            // 
            this.btnRecuperarCancel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnRecuperarCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRecuperarCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRecuperarCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnRecuperarCancel.Image")));
            this.btnRecuperarCancel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnRecuperarCancel.Location = new System.Drawing.Point(225, 12);
            this.btnRecuperarCancel.Name = "btnRecuperarCancel";
            this.btnRecuperarCancel.Size = new System.Drawing.Size(79, 68);
            this.btnRecuperarCancel.TabIndex = 4;
            this.btnRecuperarCancel.Text = "Salir de \r\nrecuperar";
            this.btnRecuperarCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnRecuperarCancel.UseVisualStyleBackColor = false;
            this.btnRecuperarCancel.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // panelDevolucionVta
            // 
            this.panelDevolucionVta.Controls.Add(this.btnDevolucionOK);
            this.panelDevolucionVta.Controls.Add(this.btnDevolucionCancel);
            this.panelDevolucionVta.Controls.Add(this.groupBox9);
            this.panelDevolucionVta.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panelDevolucionVta.Location = new System.Drawing.Point(210, 204);
            this.panelDevolucionVta.Name = "panelDevolucionVta";
            this.panelDevolucionVta.Size = new System.Drawing.Size(177, 108);
            this.panelDevolucionVta.TabIndex = 1;
            this.panelDevolucionVta.Visible = false;
            // 
            // btnDevolucionOK
            // 
            this.btnDevolucionOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDevolucionOK.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDevolucionOK.Image = ((System.Drawing.Image)(resources.GetObject("btnDevolucionOK.Image")));
            this.btnDevolucionOK.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDevolucionOK.Location = new System.Drawing.Point(76, 85);
            this.btnDevolucionOK.Name = "btnDevolucionOK";
            this.btnDevolucionOK.Size = new System.Drawing.Size(75, 68);
            this.btnDevolucionOK.TabIndex = 3;
            this.btnDevolucionOK.Text = "Devolver\r\narticulos";
            this.btnDevolucionOK.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDevolucionOK.UseVisualStyleBackColor = false;
            this.btnDevolucionOK.Click += new System.EventHandler(this.btnDevolucionOK_Click);
            // 
            // btnDevolucionCancel
            // 
            this.btnDevolucionCancel.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnDevolucionCancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnDevolucionCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnDevolucionCancel.Image")));
            this.btnDevolucionCancel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnDevolucionCancel.Location = new System.Drawing.Point(157, 85);
            this.btnDevolucionCancel.Name = "btnDevolucionCancel";
            this.btnDevolucionCancel.Size = new System.Drawing.Size(75, 68);
            this.btnDevolucionCancel.TabIndex = 3;
            this.btnDevolucionCancel.Text = "[Esc]\r\nSalir";
            this.btnDevolucionCancel.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnDevolucionCancel.UseVisualStyleBackColor = false;
            this.btnDevolucionCancel.Click += new System.EventHandler(this.btnCancelar_Click);
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.txtNumTktDevolucion);
            this.groupBox9.Location = new System.Drawing.Point(12, 12);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(220, 66);
            this.groupBox9.TabIndex = 0;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "# Ticket:";
            // 
            // txtNumTktDevolucion
            // 
            this.txtNumTktDevolucion.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtNumTktDevolucion.Location = new System.Drawing.Point(6, 25);
            this.txtNumTktDevolucion.Name = "txtNumTktDevolucion";
            this.txtNumTktDevolucion.Size = new System.Drawing.Size(204, 29);
            this.txtNumTktDevolucion.TabIndex = 1;
            this.txtNumTktDevolucion.Text = "0";
            this.txtNumTktDevolucion.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FunctionsPOS
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 504);
            this.Controls.Add(this.panelRecuperarVta);
            this.Controls.Add(this.panelDevolucionVta);
            this.Controls.Add(this.panelReimprimir);
            this.Controls.Add(this.panelCobrar);
            this.Controls.Add(this.panelDescuentoOnline);
            this.Controls.Add(this.panelVerificar);
            this.Controls.Add(this.panelCambioPrecio);
            this.Controls.Add(this.panelBuscar);
            this.Controls.Add(this.panelCambiarCantidad);
            this.Controls.Add(this.panelDescuentoGlobal);
            this.Name = "FunctionsPOS";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FunctionsPOS";
            this.Load += new System.EventHandler(this.FunctionsPOS_Load);
            this.panelCobrar.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.panelBuscar.Panel1.ResumeLayout(false);
            this.panelBuscar.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelBuscar)).EndInit();
            this.panelBuscar.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panelCambioPrecio.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.panelCambiarCantidad.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.panelDescuentoOnline.ResumeLayout(false);
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.panelDescuentoGlobal.ResumeLayout(false);
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.panelVerificar.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.panelReimprimir.ResumeLayout(false);
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.panelRecuperarVta.Panel1.ResumeLayout(false);
            this.panelRecuperarVta.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelRecuperarVta)).EndInit();
            this.panelRecuperarVta.ResumeLayout(false);
            this.panelDevolucionVta.ResumeLayout(false);
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
