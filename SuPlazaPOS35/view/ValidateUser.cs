using System;
using System.ComponentModel;
using System.Windows.Forms;

using SuPlazaPOS35.controller;

namespace SuPlazaPOS35.view
{
    public class ValidateUser : Form
    {
        private string permiso { get; set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        public ValidateUser(string permiso)
        {
            POS.supervisor = null;
            this.permiso = permiso;
            InitializeComponent();
        }

        private void ValidateUser_Load(object sender, EventArgs e)
        {
            txtPassword.Select();
        }

        private void cmdAuth_Click(object sender, EventArgs e)
        {
            if (POS.SupervisorAuthorized(txtPassword.Text, permiso))
            {
                POS.supervisor = POS.getSupervisorAuthorized(txtPassword.Text, permiso);
                base.DialogResult = DialogResult.OK;
                Dispose();
            }
            else
            {
                txtPassword.SelectAll();
                MessageBox.Show("No tiene autorización para realizar la acción requerida", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
        }

        private void cmdExit_Click(object sender, EventArgs e)
        {
            POS.supervisor = null;
            base.DialogResult = DialogResult.Cancel;
            Dispose();
        }

        #region Componentes
        private IContainer components;

        private GroupBox groupBox1;

        private TextBox txtPassword;

        private Label lblPassword;

        private Button cmdExit;

        private Button cmdAuth;
        private void InitializeComponent()
        {
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.cmdExit = new System.Windows.Forms.Button();
            this.cmdAuth = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtPassword);
            this.groupBox1.Controls.Add(this.lblPassword);
            this.groupBox1.Location = new System.Drawing.Point(27, 16);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.groupBox1.Size = new System.Drawing.Size(386, 74);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Ingresar datos de usuario";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(120, 28);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.PasswordChar = '•';
            this.txtPassword.Size = new System.Drawing.Size(244, 26);
            this.txtPassword.TabIndex = 1;
            // 
            // lblPassword
            // 
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(15, 32);
            this.lblPassword.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new System.Drawing.Size(96, 20);
            this.lblPassword.TabIndex = 2;
            this.lblPassword.Text = "Contraseña:";
            // 
            // cmdExit
            // 
            this.cmdExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdExit.Location = new System.Drawing.Point(225, 100);
            this.cmdExit.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmdExit.Name = "cmdExit";
            this.cmdExit.Size = new System.Drawing.Size(112, 35);
            this.cmdExit.TabIndex = 6;
            this.cmdExit.Text = "&Salir";
            this.cmdExit.UseVisualStyleBackColor = true;
            this.cmdExit.Click += new System.EventHandler(this.cmdExit_Click);
            // 
            // cmdAuth
            // 
            this.cmdAuth.Location = new System.Drawing.Point(72, 100);
            this.cmdAuth.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.cmdAuth.Name = "cmdAuth";
            this.cmdAuth.Size = new System.Drawing.Size(112, 35);
            this.cmdAuth.TabIndex = 5;
            this.cmdAuth.Text = "&Ingresar";
            this.cmdAuth.UseVisualStyleBackColor = true;
            this.cmdAuth.Click += new System.EventHandler(this.cmdAuth_Click);
            // 
            // ValidateUser
            // 
            this.AcceptButton = this.cmdAuth;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdExit;
            this.ClientSize = new System.Drawing.Size(426, 152);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.cmdExit);
            this.Controls.Add(this.cmdAuth);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "ValidateUser";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Acceso a supervisor";
            this.Load += new System.EventHandler(this.ValidateUser_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }
        #endregion
    }

}
