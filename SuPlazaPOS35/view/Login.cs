using System;
using System.ComponentModel;
using System.Windows.Forms;

using NLog;

using SuPlazaPOS35.controller;

namespace SuPlazaPOS35.view
{
    public class Login : Form
    {
        private static readonly NLog.Logger logger = LogManager.GetCurrentClassLogger();

        public Login()
        {
            InitializeComponent();
        }

        private void btnAuth_Click(object sender, EventArgs e)
        {
            try
            {
                if (new POS().validateUser(txtUserName.Text, txtPassword.Text))
                {
                    if (new POS().isRegisterPOS())
                    {
                        Hide();
                        new SuPlazaPOS().ShowDialog();
                        Close();
                    }
                    else if (MessageBox.Show("La aplicación ha iniciado por primera vez.\n¿Desea establecer los parámetros de inicio?", "Configuración inicial", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Hide();
                        new Config().ShowDialog();
                        Close();
                    }
                    else
                    {
                        Application.Exit();
                    }
                }
                else
                {
                    emptyFields();
                    MessageBox.Show("Datos erróneos o sin autorización para acceder al PDV.", "Autenticación incorrecta", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    txtPassword.Focus();
                    txtPassword.SelectAll();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Problemas de acceso a datos, no se pudo finalizar el proceso.");
                MessageBox.Show(ex.Message, ex.Source, MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void btnDisponse_Click(object sender, EventArgs e)
        {
            Dispose();
        }

        private void emptyFields()
        {
            txtUserName.Text = "";
            txtPassword.Text = "";
            txtUserName.Focus();
        }

        private void Login_Activated(object sender, EventArgs e)
        {
            txtUserName.Focus();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private static void ExitAndCloseForm() 
        {
            Application.ExitThread();
            Application.Exit();
        }

        #region Componentes

        private IContainer components;

        private Button btnAuth;

        private Button btnDisponse;

        private Label lblUserName;

        private TextBox txtUserName;

        private GroupBox groupBox1;

        private TextBox txtPassword;

        private Label lblPassword;
        private void InitializeComponent()
        {
            this.btnAuth = new System.Windows.Forms.Button();
            this.btnDisponse = new System.Windows.Forms.Button();
            this.lblUserName = new System.Windows.Forms.Label();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAuth
            // 
            this.btnAuth.Location = new System.Drawing.Point(69, 149);
            this.btnAuth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnAuth.Name = "btnAuth";
            this.btnAuth.Size = new System.Drawing.Size(112, 35);
            this.btnAuth.TabIndex = 2;
            this.btnAuth.Text = "&Ingresar";
            this.btnAuth.UseVisualStyleBackColor = true;
            this.btnAuth.Click += new System.EventHandler(this.btnAuth_Click);
            // 
            // btnDisponse
            // 
            this.btnDisponse.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnDisponse.Location = new System.Drawing.Point(222, 149);
            this.btnDisponse.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnDisponse.Name = "btnDisponse";
            this.btnDisponse.Size = new System.Drawing.Size(112, 35);
            this.btnDisponse.TabIndex = 3;
            this.btnDisponse.Text = "&Salir";
            this.btnDisponse.UseVisualStyleBackColor = true;
            this.btnDisponse.Click += new System.EventHandler(this.btnDisponse_Click);
            // 
            // lblUserName
            // 
            this.lblUserName.AutoSize = true;
            this.lblUserName.Location = new System.Drawing.Point(9, 34);
            this.lblUserName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblUserName.Name = "lblUserName";
            this.lblUserName.Size = new System.Drawing.Size(69, 20);
            this.lblUserName.TabIndex = 2;
            this.lblUserName.Text = "Nombre:";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(114, 29);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(244, 26);
            this.txtUserName.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.txtUserName);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Controls.Add(this.lblUserName);
            this.groupBox1.Location = new System.Drawing.Point(23, 17);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(386, 122);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ingresar datos de usuario";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(114, 69);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.Size = new System.Drawing.Size(244, 26);
            this.txtPassword.TabIndex = 1;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(9, 74);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(96, 20);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Contraseña:";
            // 
            // Login
            // 
            this.AcceptButton = this.btnAuth;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnDisponse;
            this.ClientSize = new System.Drawing.Size(422, 198);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnDisponse);
            this.Controls.Add(this.btnAuth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Identificación del usuario";
            this.Activated += new System.EventHandler(this.Login_Activated);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }
}