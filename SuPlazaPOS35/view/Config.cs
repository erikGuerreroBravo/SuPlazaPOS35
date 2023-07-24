using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using SuPlazaPOS35.controller;
using SuPlazaPOS35.DAO;
using SuPlazaPOS35.domain;

using NLog;

namespace SuPlazaPOS35.view
{
    public class Config : Form
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public Config()
        {
            InitializeComponent();
            loadDataConfigurations();
            if (POS.caja != null)
            {
                loadSettings();
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Hide();
            Close();
        }

        private void loadSettings()
        {
            pos_settings caja = POS.caja;
            txtIdPos.Text = caja.id_pos.ToString();
            txtFolio.Text = caja.pos_folio.ToString();
            cboTipoPago.Text = caja.tipo_pago;
            ckbLogoTicket.Checked = caja.pos_log_enable;
            ckbTicketConcentrado.Checked = caja.tck_concentrado;
            ckbInventarioOnline.Checked = caja.inventario_online;
            cboDisplaysOPOS.Text = caja.pos_dsp_name;
            ckbEnableDisplay.Checked = caja.pos_dsp_enable;
            cboPrintersOPOS.Text = caja.pos_ptr_name;
            ckbEnablePrinter.Checked = caja.pos_ptr_enable;
            cboCashDrawersOPOS.Text = caja.pos_csh_name;
            ckbEnableCashDramer.Checked = caja.pos_csh_enable;
            cboScannersOPOS.Text = caja.pos_scn_name;
            ckbScanner.Checked = caja.pos_scn_enable;
            cboPrintersWin.Text = caja.win_ptr_name;
            ckbEnablePrinterWin.Checked = caja.win_ptr_enable;
            cboPuertoCOM.Text = caja.com_name;
            cboBaudRates.Text = caja.com_rate;
            cboDataBits.Text = caja.com_bits.ToString();
            cboParity.Text = caja.com_parity;
            cboStopBits.Text = caja.com_stop;
            ckbEnableCOM.Checked = caja.com_enable;
        }

        private void loadDataConfigurations()
        {
            cboPuertoCOM.Items.AddRange(DevicesWindows.getPortsCOM());
            cboParity.Items.AddRange(DevicesWindows.getParitys());
            cboStopBits.Items.AddRange(DevicesWindows.getStopBits());
            cboPrintersWin.Items.AddRange(DevicesWindows.getPrintersWindows());
            cboDisplaysOPOS.Items.AddRange(DevicesOPOS.getDisplaysOPOS());
            cboPrintersOPOS.Items.AddRange(DevicesOPOS.getPrintersOPOS());
            cboCashDrawersOPOS.Items.AddRange(DevicesOPOS.getCashDrawerOPOS());
            cboScannersOPOS.Items.AddRange(DevicesOPOS.getScannerOPOS());
            cboTipoPago.SelectedIndex = 0;
            cboParity.SelectedIndex = 0;
            cboStopBits.SelectedIndex = 0;
            cboBaudRates.SelectedIndex = 1;
            cboDataBits.SelectedIndex = 1;
            cboStopBits.SelectedIndex = 1;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                pos_settings pos_settings = ((POS.caja == null) ? new pos_settings() : POS.caja);
                pos_settings.id_pos = short.Parse(txtIdPos.Text);
                pos_settings.tipo_pago = cboTipoPago.Text;
                pos_settings.tck_concentrado = ckbTicketConcentrado.Checked;
                pos_settings.inventario_online = ckbInventarioOnline.Checked;
                pos_settings.pos_dsp_name = cboDisplaysOPOS.Text;
                pos_settings.pos_dsp_enable = ckbEnableDisplay.Checked;
                pos_settings.pos_ptr_name = cboPrintersOPOS.Text;
                pos_settings.pos_ptr_enable = ckbEnablePrinter.Checked;
                pos_settings.pos_log_enable = ckbLogoTicket.Checked;
                pos_settings.pos_csh_name = cboCashDrawersOPOS.Text;
                pos_settings.pos_csh_enable = ckbEnableCashDramer.Checked;
                pos_settings.pos_scn_name = cboScannersOPOS.Text;
                pos_settings.pos_scn_enable = ckbScanner.Checked;
                pos_settings.win_ptr_name = cboPrintersWin.Text;
                pos_settings.win_ptr_enable = ckbEnablePrinterWin.Checked;
                pos_settings.com_name = cboPuertoCOM.Text;
                pos_settings.com_rate = cboBaudRates.Text;
                pos_settings.com_bits = short.Parse(cboDataBits.Text);
                pos_settings.com_parity = cboParity.Text;
                pos_settings.com_stop = cboStopBits.Text;
                pos_settings.com_enable = ckbEnableCOM.Checked;
                pos_settings.pos_folio = ((txtFolio.Text.Trim().Length > 0) ? long.Parse(txtFolio.Text.Trim()) : 1);
                if (POS.caja == null)
                {
                    pos_settings.last_corte_z = DateTime.Now;
                }
                new POS(pos_settings);
                if (POS.caja == null)
                {
                    MessageBox.Show("Para comenzar, es necesario vuelva a ejecutar el sistema.", "Registro exitoso", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                    Application.Exit();
                }
                else
                {
                    Dispose();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Faltan datos por definir, verifique.", "Error al registrar", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void Config_Load(object sender, EventArgs e)
        {
            if (POS.caja != null)
            {
                txtIdPos.Text = POS.caja.id_pos.ToString();
                txtFolio.Text = POS.caja.pos_folio.ToString();
                cboTipoPago.Text = POS.caja.tipo_pago;
                ckbTicketConcentrado.Checked = POS.caja.tck_concentrado;
                ckbInventarioOnline.Checked = POS.caja.inventario_online;
                cboDisplaysOPOS.Text = POS.caja.pos_dsp_name;
                ckbEnableDisplay.Checked = POS.caja.pos_dsp_enable;
                cboPrintersOPOS.Text = POS.caja.pos_ptr_name;
                ckbEnablePrinter.Checked = POS.caja.pos_ptr_enable;
                cboCashDrawersOPOS.Text = POS.caja.pos_csh_name;
                ckbEnableCashDramer.Checked = POS.caja.pos_csh_enable;
                cboScannersOPOS.Text = POS.caja.pos_scn_name;
                ckbScanner.Checked = POS.caja.pos_scn_enable;
                cboPrintersWin.Text = POS.caja.win_ptr_name;
                ckbEnablePrinterWin.Checked = POS.caja.win_ptr_enable;
                cboPuertoCOM.Text = POS.caja.com_name;
                cboBaudRates.Text = POS.caja.com_rate;
                cboDataBits.Text = POS.caja.com_bits.ToString();
                cboParity.Text = POS.caja.com_parity;
                cboStopBits.Text = POS.caja.com_stop;
                ckbEnableCOM.Checked = POS.caja.com_enable;
            }
        }
        #region Componentes
        private IContainer components;

        private TabControl tabPOS;

        private TabPage tabPage1;

        private TabPage tabPage2;

        private Panel panel1;

        private Panel panel2;

        private Label label3;

        private Label label2;

        private Label label1;

        private TabPage tabPage3;

        private Label label4;

        private TextBox txtIdPos;

        private CheckBox ckbInventarioOnline;

        private CheckBox ckbTicketConcentrado;

        private ComboBox cboTipoPago;

        private Button btnExit;

        private Button btnSave;

        private ComboBox cboPrintersOPOS;

        private ComboBox cboDisplaysOPOS;

        private Label label7;

        private Label label5;

        private CheckBox ckbEnableCashDramer;

        private CheckBox ckbEnablePrinter;

        private CheckBox ckbEnableDisplay;

        private ComboBox cboCashDrawersOPOS;

        private Label label8;

        private CheckBox ckbScanner;

        private ComboBox cboScannersOPOS;

        private Label label6;

        private Panel panel3;

        private CheckBox ckbEnablePrinterWin;

        private ComboBox cboPrintersWin;

        private Label label9;

        private TabPage tabPage4;

        private Panel panel4;

        private CheckBox ckbEnableCOM;

        private ComboBox cboPuertoCOM;

        private Label label10;

        private ComboBox cboBaudRates;

        private Label label11;

        private ComboBox cboStopBits;

        private Label label14;

        private ComboBox cboParity;

        private Label label13;

        private ComboBox cboDataBits;

        private Label label12;

        private CheckBox ckbLogoTicket;

        private Label label15;

        private TextBox txtFolio;

        private Label label16;

        private Thread threadDownload;

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Config));
            this.tabPOS = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtFolio = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.ckbLogoTicket = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.ckbInventarioOnline = new System.Windows.Forms.CheckBox();
            this.ckbTicketConcentrado = new System.Windows.Forms.CheckBox();
            this.cboTipoPago = new System.Windows.Forms.ComboBox();
            this.txtIdPos = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ckbScanner = new System.Windows.Forms.CheckBox();
            this.ckbEnableCashDramer = new System.Windows.Forms.CheckBox();
            this.ckbEnablePrinter = new System.Windows.Forms.CheckBox();
            this.ckbEnableDisplay = new System.Windows.Forms.CheckBox();
            this.cboScannersOPOS = new System.Windows.Forms.ComboBox();
            this.cboCashDrawersOPOS = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cboPrintersOPOS = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cboDisplaysOPOS = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.panel3 = new System.Windows.Forms.Panel();
            this.ckbEnablePrinterWin = new System.Windows.Forms.CheckBox();
            this.cboPrintersWin = new System.Windows.Forms.ComboBox();
            this.label9 = new System.Windows.Forms.Label();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.panel4 = new System.Windows.Forms.Panel();
            this.ckbEnableCOM = new System.Windows.Forms.CheckBox();
            this.cboStopBits = new System.Windows.Forms.ComboBox();
            this.label14 = new System.Windows.Forms.Label();
            this.cboParity = new System.Windows.Forms.ComboBox();
            this.label13 = new System.Windows.Forms.Label();
            this.cboDataBits = new System.Windows.Forms.ComboBox();
            this.label12 = new System.Windows.Forms.Label();
            this.cboBaudRates = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.cboPuertoCOM = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.btnExit = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.tabPOS.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.panel3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPOS
            // 
            this.tabPOS.Controls.Add(this.tabPage1);
            this.tabPOS.Controls.Add(this.tabPage2);
            this.tabPOS.Controls.Add(this.tabPage3);
            this.tabPOS.Controls.Add(this.tabPage4);
            this.tabPOS.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPOS.Location = new System.Drawing.Point(16, 14);
            this.tabPOS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPOS.Name = "tabPOS";
            this.tabPOS.SelectedIndex = 0;
            this.tabPOS.Size = new System.Drawing.Size(656, 302);
            this.tabPOS.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.panel1);
            this.tabPage1.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPage1.Location = new System.Drawing.Point(4, 26);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage1.Size = new System.Drawing.Size(648, 272);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Datos de caja";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel1.Controls.Add(this.txtFolio);
            this.panel1.Controls.Add(this.label16);
            this.panel1.Controls.Add(this.ckbLogoTicket);
            this.panel1.Controls.Add(this.label15);
            this.panel1.Controls.Add(this.ckbInventarioOnline);
            this.panel1.Controls.Add(this.ckbTicketConcentrado);
            this.panel1.Controls.Add(this.cboTipoPago);
            this.panel1.Controls.Add(this.txtIdPos);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(4, 4);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(640, 264);
            this.panel1.TabIndex = 0;
            // 
            // txtFolio
            // 
            this.txtFolio.Location = new System.Drawing.Point(324, 212);
            this.txtFolio.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFolio.Name = "txtFolio";
            this.txtFolio.Size = new System.Drawing.Size(135, 26);
            this.txtFolio.TabIndex = 16;
            this.txtFolio.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label16
            // 
            this.label16.Location = new System.Drawing.Point(75, 215);
            this.label16.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(242, 21);
            this.label16.TabIndex = 6;
            this.label16.Text = "Folio inicial de Ticket:";
            this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckbLogoTicket
            // 
            this.ckbLogoTicket.AutoSize = true;
            this.ckbLogoTicket.Location = new System.Drawing.Point(324, 102);
            this.ckbLogoTicket.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbLogoTicket.Name = "ckbLogoTicket";
            this.ckbLogoTicket.Size = new System.Drawing.Size(18, 17);
            this.ckbLogoTicket.TabIndex = 13;
            this.ckbLogoTicket.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.Location = new System.Drawing.Point(114, 101);
            this.label15.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(203, 21);
            this.label15.TabIndex = 5;
            this.label15.Text = "Logo en ticket:";
            this.label15.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // ckbInventarioOnline
            // 
            this.ckbInventarioOnline.AutoSize = true;
            this.ckbInventarioOnline.Location = new System.Drawing.Point(324, 178);
            this.ckbInventarioOnline.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbInventarioOnline.Name = "ckbInventarioOnline";
            this.ckbInventarioOnline.Size = new System.Drawing.Size(18, 17);
            this.ckbInventarioOnline.TabIndex = 15;
            this.ckbInventarioOnline.UseVisualStyleBackColor = true;
            // 
            // ckbTicketConcentrado
            // 
            this.ckbTicketConcentrado.AutoSize = true;
            this.ckbTicketConcentrado.Location = new System.Drawing.Point(324, 140);
            this.ckbTicketConcentrado.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbTicketConcentrado.Name = "ckbTicketConcentrado";
            this.ckbTicketConcentrado.Size = new System.Drawing.Size(18, 17);
            this.ckbTicketConcentrado.TabIndex = 14;
            this.ckbTicketConcentrado.UseVisualStyleBackColor = true;
            // 
            // cboTipoPago
            // 
            this.cboTipoPago.FormattingEnabled = true;
            this.cboTipoPago.Items.AddRange(new object[] {
            "Efectivo",
            "Tarjeta de crédito",
            "Vales",
            "Cheque"});
            this.cboTipoPago.Location = new System.Drawing.Point(324, 58);
            this.cboTipoPago.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboTipoPago.Name = "cboTipoPago";
            this.cboTipoPago.Size = new System.Drawing.Size(252, 28);
            this.cboTipoPago.TabIndex = 12;
            // 
            // txtIdPos
            // 
            this.txtIdPos.Location = new System.Drawing.Point(324, 21);
            this.txtIdPos.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtIdPos.Name = "txtIdPos";
            this.txtIdPos.Size = new System.Drawing.Size(71, 26);
            this.txtIdPos.TabIndex = 11;
            this.txtIdPos.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.Location = new System.Drawing.Point(119, 178);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(197, 21);
            this.label4.TabIndex = 0;
            this.label4.Text = "Inventario en línea:";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(114, 139);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(203, 21);
            this.label3.TabIndex = 0;
            this.label3.Text = "Ticket concentrado:";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(53, 62);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(263, 21);
            this.label2.TabIndex = 0;
            this.label2.Text = "Pago por defecto:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(109, 25);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(206, 21);
            this.label1.TabIndex = 0;
            this.label1.Text = "Número de caja:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.panel2);
            this.tabPage2.Location = new System.Drawing.Point(4, 26);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage2.Size = new System.Drawing.Size(648, 272);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Dispositivos OPOS";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel2.Controls.Add(this.ckbScanner);
            this.panel2.Controls.Add(this.ckbEnableCashDramer);
            this.panel2.Controls.Add(this.ckbEnablePrinter);
            this.panel2.Controls.Add(this.ckbEnableDisplay);
            this.panel2.Controls.Add(this.cboScannersOPOS);
            this.panel2.Controls.Add(this.cboCashDrawersOPOS);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.cboPrintersOPOS);
            this.panel2.Controls.Add(this.label8);
            this.panel2.Controls.Add(this.cboDisplaysOPOS);
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel2.Location = new System.Drawing.Point(4, 4);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(640, 264);
            this.panel2.TabIndex = 0;
            // 
            // ckbScanner
            // 
            this.ckbScanner.AutoSize = true;
            this.ckbScanner.Location = new System.Drawing.Point(516, 220);
            this.ckbScanner.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbScanner.Name = "ckbScanner";
            this.ckbScanner.Size = new System.Drawing.Size(90, 24);
            this.ckbScanner.TabIndex = 28;
            this.ckbScanner.Text = "Activar";
            this.ckbScanner.UseVisualStyleBackColor = true;
            // 
            // ckbEnableCashDramer
            // 
            this.ckbEnableCashDramer.AutoSize = true;
            this.ckbEnableCashDramer.Location = new System.Drawing.Point(516, 162);
            this.ckbEnableCashDramer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbEnableCashDramer.Name = "ckbEnableCashDramer";
            this.ckbEnableCashDramer.Size = new System.Drawing.Size(90, 24);
            this.ckbEnableCashDramer.TabIndex = 26;
            this.ckbEnableCashDramer.Text = "Activar";
            this.ckbEnableCashDramer.UseVisualStyleBackColor = true;
            // 
            // ckbEnablePrinter
            // 
            this.ckbEnablePrinter.AutoSize = true;
            this.ckbEnablePrinter.Location = new System.Drawing.Point(516, 100);
            this.ckbEnablePrinter.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbEnablePrinter.Name = "ckbEnablePrinter";
            this.ckbEnablePrinter.Size = new System.Drawing.Size(90, 24);
            this.ckbEnablePrinter.TabIndex = 24;
            this.ckbEnablePrinter.Text = "Activar";
            this.ckbEnablePrinter.UseVisualStyleBackColor = true;
            // 
            // ckbEnableDisplay
            // 
            this.ckbEnableDisplay.AutoSize = true;
            this.ckbEnableDisplay.Location = new System.Drawing.Point(517, 43);
            this.ckbEnableDisplay.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbEnableDisplay.Name = "ckbEnableDisplay";
            this.ckbEnableDisplay.Size = new System.Drawing.Size(90, 24);
            this.ckbEnableDisplay.TabIndex = 22;
            this.ckbEnableDisplay.Text = "Activar";
            this.ckbEnableDisplay.UseVisualStyleBackColor = true;
            // 
            // cboScannersOPOS
            // 
            this.cboScannersOPOS.FormattingEnabled = true;
            this.cboScannersOPOS.Location = new System.Drawing.Point(18, 220);
            this.cboScannersOPOS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboScannersOPOS.Name = "cboScannersOPOS";
            this.cboScannersOPOS.Size = new System.Drawing.Size(491, 28);
            this.cboScannersOPOS.TabIndex = 27;
            // 
            // cboCashDrawersOPOS
            // 
            this.cboCashDrawersOPOS.FormattingEnabled = true;
            this.cboCashDrawersOPOS.Location = new System.Drawing.Point(18, 162);
            this.cboCashDrawersOPOS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboCashDrawersOPOS.Name = "cboCashDrawersOPOS";
            this.cboCashDrawersOPOS.Size = new System.Drawing.Size(491, 28);
            this.cboCashDrawersOPOS.TabIndex = 25;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(21, 197);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 20);
            this.label6.TabIndex = 3;
            this.label6.Text = "Scanner:";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboPrintersOPOS
            // 
            this.cboPrintersOPOS.FormattingEnabled = true;
            this.cboPrintersOPOS.Location = new System.Drawing.Point(18, 100);
            this.cboPrintersOPOS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboPrintersOPOS.Name = "cboPrintersOPOS";
            this.cboPrintersOPOS.Size = new System.Drawing.Size(491, 28);
            this.cboPrintersOPOS.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(21, 138);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(119, 20);
            this.label8.TabIndex = 3;
            this.label8.Text = "CashDrawer:";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboDisplaysOPOS
            // 
            this.cboDisplaysOPOS.FormattingEnabled = true;
            this.cboDisplaysOPOS.Location = new System.Drawing.Point(18, 39);
            this.cboDisplaysOPOS.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboDisplaysOPOS.Name = "cboDisplaysOPOS";
            this.cboDisplaysOPOS.Size = new System.Drawing.Size(491, 28);
            this.cboDisplaysOPOS.TabIndex = 21;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(21, 76);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 20);
            this.label7.TabIndex = 3;
            this.label7.Text = "Printer:";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(21, 16);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 20);
            this.label5.TabIndex = 3;
            this.label5.Text = "Display:";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.panel3);
            this.tabPage3.Location = new System.Drawing.Point(4, 26);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage3.Size = new System.Drawing.Size(648, 272);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Dispositivos Windows";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // panel3
            // 
            this.panel3.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel3.Controls.Add(this.ckbEnablePrinterWin);
            this.panel3.Controls.Add(this.cboPrintersWin);
            this.panel3.Controls.Add(this.label9);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel3.Location = new System.Drawing.Point(4, 4);
            this.panel3.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(640, 264);
            this.panel3.TabIndex = 0;
            // 
            // ckbEnablePrinterWin
            // 
            this.ckbEnablePrinterWin.AutoSize = true;
            this.ckbEnablePrinterWin.Location = new System.Drawing.Point(521, 107);
            this.ckbEnablePrinterWin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbEnablePrinterWin.Name = "ckbEnablePrinterWin";
            this.ckbEnablePrinterWin.Size = new System.Drawing.Size(90, 24);
            this.ckbEnablePrinterWin.TabIndex = 32;
            this.ckbEnablePrinterWin.Text = "Activar";
            this.ckbEnablePrinterWin.UseVisualStyleBackColor = true;
            // 
            // cboPrintersWin
            // 
            this.cboPrintersWin.FormattingEnabled = true;
            this.cboPrintersWin.Location = new System.Drawing.Point(23, 106);
            this.cboPrintersWin.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboPrintersWin.Name = "cboPrintersWin";
            this.cboPrintersWin.Size = new System.Drawing.Size(491, 28);
            this.cboPrintersWin.TabIndex = 31;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(27, 82);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(72, 20);
            this.label9.TabIndex = 6;
            this.label9.Text = "Printer:";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.panel4);
            this.tabPage4.Location = new System.Drawing.Point(4, 26);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabPage4.Size = new System.Drawing.Size(648, 272);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Dispositivo RS232";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.panel4.Controls.Add(this.ckbEnableCOM);
            this.panel4.Controls.Add(this.cboStopBits);
            this.panel4.Controls.Add(this.label14);
            this.panel4.Controls.Add(this.cboParity);
            this.panel4.Controls.Add(this.label13);
            this.panel4.Controls.Add(this.cboDataBits);
            this.panel4.Controls.Add(this.label12);
            this.panel4.Controls.Add(this.cboBaudRates);
            this.panel4.Controls.Add(this.label11);
            this.panel4.Controls.Add(this.cboPuertoCOM);
            this.panel4.Controls.Add(this.label10);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.panel4.Location = new System.Drawing.Point(4, 4);
            this.panel4.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(640, 264);
            this.panel4.TabIndex = 0;
            // 
            // ckbEnableCOM
            // 
            this.ckbEnableCOM.AutoSize = true;
            this.ckbEnableCOM.Location = new System.Drawing.Point(290, 206);
            this.ckbEnableCOM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.ckbEnableCOM.Name = "ckbEnableCOM";
            this.ckbEnableCOM.Size = new System.Drawing.Size(90, 24);
            this.ckbEnableCOM.TabIndex = 46;
            this.ckbEnableCOM.Text = "Activar";
            this.ckbEnableCOM.UseVisualStyleBackColor = true;
            // 
            // cboStopBits
            // 
            this.cboStopBits.FormattingEnabled = true;
            this.cboStopBits.Location = new System.Drawing.Point(290, 170);
            this.cboStopBits.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboStopBits.Name = "cboStopBits";
            this.cboStopBits.Size = new System.Drawing.Size(150, 28);
            this.cboStopBits.TabIndex = 45;
            // 
            // label14
            // 
            this.label14.Location = new System.Drawing.Point(72, 174);
            this.label14.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(210, 21);
            this.label14.TabIndex = 9;
            this.label14.Text = "Stop bits:";
            this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboParity
            // 
            this.cboParity.FormattingEnabled = true;
            this.cboParity.Location = new System.Drawing.Point(290, 133);
            this.cboParity.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboParity.Name = "cboParity";
            this.cboParity.Size = new System.Drawing.Size(150, 28);
            this.cboParity.TabIndex = 44;
            // 
            // label13
            // 
            this.label13.Location = new System.Drawing.Point(72, 137);
            this.label13.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(210, 21);
            this.label13.TabIndex = 9;
            this.label13.Text = "Parity:";
            this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboDataBits
            // 
            this.cboDataBits.FormattingEnabled = true;
            this.cboDataBits.Items.AddRange(new object[] {
            "7",
            "8"});
            this.cboDataBits.Location = new System.Drawing.Point(290, 96);
            this.cboDataBits.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboDataBits.Name = "cboDataBits";
            this.cboDataBits.Size = new System.Drawing.Size(150, 28);
            this.cboDataBits.TabIndex = 43;
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(72, 100);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(210, 21);
            this.label12.TabIndex = 9;
            this.label12.Text = "Data bits:";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboBaudRates
            // 
            this.cboBaudRates.FormattingEnabled = true;
            this.cboBaudRates.Items.AddRange(new object[] {
            "4800",
            "9600",
            "19200",
            "38400",
            "57600",
            "115200",
            "230400"});
            this.cboBaudRates.Location = new System.Drawing.Point(290, 59);
            this.cboBaudRates.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboBaudRates.Name = "cboBaudRates";
            this.cboBaudRates.Size = new System.Drawing.Size(150, 28);
            this.cboBaudRates.TabIndex = 42;
            // 
            // label11
            // 
            this.label11.Location = new System.Drawing.Point(72, 62);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(210, 21);
            this.label11.TabIndex = 9;
            this.label11.Text = "Baud Rates:";
            this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // cboPuertoCOM
            // 
            this.cboPuertoCOM.FormattingEnabled = true;
            this.cboPuertoCOM.Location = new System.Drawing.Point(290, 22);
            this.cboPuertoCOM.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboPuertoCOM.Name = "cboPuertoCOM";
            this.cboPuertoCOM.Size = new System.Drawing.Size(150, 28);
            this.cboPuertoCOM.TabIndex = 41;
            // 
            // label10
            // 
            this.label10.Location = new System.Drawing.Point(72, 26);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(210, 21);
            this.label10.TabIndex = 9;
            this.label10.Text = "Puerto COM:";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnExit.Image = ((System.Drawing.Image)(resources.GetObject("btnExit.Image")));
            this.btnExit.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnExit.Location = new System.Drawing.Point(564, 318);
            this.btnExit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(99, 64);
            this.btnExit.TabIndex = 102;
            this.btnExit.Text = "&Salir";
            this.btnExit.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.btnSave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.btnSave.Location = new System.Drawing.Point(457, 318);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(99, 64);
            this.btnSave.TabIndex = 101;
            this.btnSave.Text = "&Guardar";
            this.btnSave.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // Config
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(688, 388);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.tabPOS);
            this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.Name = "Config";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Panel de configuraciones de POS";
            this.Load += new System.EventHandler(this.Config_Load);
            this.tabPOS.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}
