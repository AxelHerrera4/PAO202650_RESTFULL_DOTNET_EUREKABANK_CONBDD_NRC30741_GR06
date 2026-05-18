using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.vista
{
    public partial class CajeroDashboardForm : Form
    {
        private Label lblBienvenida;
        private TextBox txtNumCuentaBusqueda;
        private Button btnBuscarCuenta;
        private Label lblSaldoVal, lblTitularVal, lblEstadoVal;

        private TabControl tabbedPane;
        private TextBox txtMontoDeposito, txtMontoRetiro, txtMontoTransferencia, txtCuentaDestino;
        private Button btnProcesarDeposito, btnProcesarRetiro, btnProcesarTransferencia;

        private DataGridView tblMovimientos;
        private DataGridViewTextBoxColumn colOperacion, colFecha, colTipo, colMonto, colResponsable;

        public CajeroDashboardForm(string nombreCajero)
        {
            InitializeComponents(nombreCajero);
            ConfigureFrame();
        }

        private void InitializeComponents(string nombreCajero)
        {
            Color colorPrimario = Color.FromArgb(0, 102, 204);
            Color colorFondo = Color.FromArgb(245, 247, 250);
            Color colorBlanco = Color.White;

            Font fontLabelHeader = new Font("Segoe UI", 16, FontStyle.Bold);
            Font fontLabelInfo = new Font("Segoe UI", 12, FontStyle.Bold);
            Font fontValorInfo = new Font("Segoe UI", 12, FontStyle.Regular);
            Font fontLabel = new Font("Segoe UI", 10, FontStyle.Regular);
            Font fontBoton = new Font("Segoe UI", 10, FontStyle.Bold);

            // --- PANEL PRINCIPAL ---
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = colorFondo
            };

            // 1. CABECERA
            Panel headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = colorPrimario,
                Padding = new Padding(20)
            };

            lblBienvenida = new Label
            {
                Text = "Cajero: " + nombreCajero,
                Font = fontLabelHeader,
                ForeColor = colorBlanco,
                AutoSize = true,
                Location = new Point(20, 20)
            };

            Label lblLogo = new Label
            {
                Text = "EUREKA BANK - SISTEMA DE CAJEROS",
                Font = new Font("Segoe UI", 12, FontStyle.Italic),
                ForeColor = colorBlanco,
                AutoSize = true,
                Dock = DockStyle.Right,
                TextAlign = ContentAlignment.MiddleRight
            };

            headerPanel.Controls.Add(lblBienvenida);
            headerPanel.Controls.Add(lblLogo);

            // 2. SECCIÓN CENTRAL (BÚSQUEDA Y TRANSACCIONES)
            Panel centerPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = colorFondo,
                AutoScroll = true,
                Padding = new Padding(15)
            };

            // Sub-panel de Búsqueda
            GroupBox searchPanel = new GroupBox
            {
                Text = "Búsqueda de Cuenta",
                Font = fontLabel,
                Dock = DockStyle.Top,
                Height = 180,
                BackColor = colorBlanco,
                ForeColor = colorPrimario,
                Margin = new Padding(0, 0, 0, 10),
                Padding = new Padding(15)
            };

            Label lblNumCuenta = new Label
            {
                Text = "Número de Cuenta:",
                AutoSize = true,
                Font = fontLabel,
                Location = new Point(15, 25)
            };

            txtNumCuentaBusqueda = new TextBox
            {
                Width = 250,
                Height = 28,
                Font = fontLabel,
                Location = new Point(180, 22)
            };

            btnBuscarCuenta = new Button
            {
                Text = "Buscar Cuenta",
                Width = 120,
                Height = 28,
                BackColor = colorPrimario,
                ForeColor = colorBlanco,
                Font = fontBoton,
                FlatStyle = FlatStyle.Flat,
                Location = new Point(445, 22)
            };
            btnBuscarCuenta.FlatAppearance.BorderSize = 0;

            // Info de la cuenta
            Panel infoPanel = new Panel
            {
                Height = 100,
                Location = new Point(15, 75),
                BackColor = Color.Transparent,
                AutoSize = false
            };

            Label lblTitular = new Label { Text = "Titular:", AutoSize = true, Font = fontLabel, Location = new Point(0, 5) };
            lblTitularVal = new Label { Text = "-", AutoSize = true, Font = fontValorInfo, Location = new Point(80, 5), ForeColor = colorPrimario };

            Label lblSaldo = new Label { Text = "Saldo:", AutoSize = true, Font = fontLabel, Location = new Point(0, 30) };
            lblSaldoVal = new Label { Text = "S/ 0.00", AutoSize = true, Font = fontValorInfo, ForeColor = colorPrimario, Location = new Point(80, 30) };

            Label lblEstado = new Label { Text = "Estado:", AutoSize = true, Font = fontLabel, Location = new Point(0, 55) };
            lblEstadoVal = new Label { Text = "-", AutoSize = true, Font = fontValorInfo, Location = new Point(80, 55), ForeColor = colorPrimario };

            infoPanel.Controls.AddRange(new Control[] { lblTitular, lblTitularVal, lblSaldo, lblSaldoVal, lblEstado, lblEstadoVal });

            searchPanel.Controls.Add(lblNumCuenta);
            searchPanel.Controls.Add(txtNumCuentaBusqueda);
            searchPanel.Controls.Add(btnBuscarCuenta);
            searchPanel.Controls.Add(infoPanel);

            // --- Sub-panel de Transacciones (TabbedPane) ---
            tabbedPane = new TabControl
            {
                Dock = DockStyle.Top,
                Height = 250,
                BackColor = colorBlanco,
                Margin = new Padding(0, 0, 0, 10)
            };

            // Pestaña Depósito
            TabPage tabDeposito = new TabPage("Depósito");
            Panel pnlDeposito = CreateTransactionPanel("Procesar Depósito", "Monto a depositar:", out txtMontoDeposito, out btnProcesarDeposito, colorPrimario, fontLabel);
            tabDeposito.Controls.Add(pnlDeposito);
            tabbedPane.TabPages.Add(tabDeposito);

            // Pestaña Retiro
            TabPage tabRetiro = new TabPage("Retiro");
            Panel pnlRetiro = CreateTransactionPanel("Procesar Retiro", "Monto a retirar:", out txtMontoRetiro, out btnProcesarRetiro, colorPrimario, fontLabel);
            tabRetiro.Controls.Add(pnlRetiro);
            tabbedPane.TabPages.Add(tabRetiro);

            // Pestaña Transferencia
            TabPage tabTransferencia = new TabPage("Transferencia");
            Panel pnlTransferencia = new Panel { Dock = DockStyle.Fill, BackColor = colorBlanco, Padding = new Padding(15) };

            Label lblCuentaDestino = new Label { Text = "Cuenta Destino:", AutoSize = true, Location = new Point(15, 20), Font = fontLabel };
            txtCuentaDestino = new TextBox { Location = new Point(150, 17), Width = 250, Height = 28, Font = fontLabel };

            Label lblMontoTransf = new Label { Text = "Monto:", AutoSize = true, Location = new Point(15, 70), Font = fontLabel };
            txtMontoTransferencia = new TextBox { Location = new Point(150, 67), Width = 250, Height = 28, Font = fontLabel };

            btnProcesarTransferencia = new Button
            {
                Text = "Transferir",
                Location = new Point(15, 125),
                Width = 385,
                Height = 40,
                BackColor = colorPrimario,
                ForeColor = colorBlanco,
                Font = fontBoton,
                FlatStyle = FlatStyle.Flat
            };
            btnProcesarTransferencia.FlatAppearance.BorderSize = 0;

            pnlTransferencia.Controls.AddRange(new Control[] { lblCuentaDestino, txtCuentaDestino, lblMontoTransf, txtMontoTransferencia, btnProcesarTransferencia });
            tabTransferencia.Controls.Add(pnlTransferencia);
            tabbedPane.TabPages.Add(tabTransferencia);

            centerPanel.Controls.Add(tabbedPane);
            centerPanel.Controls.Add(searchPanel);

            // 3. SECCIÓN INFERIOR (AUDITORÍA)
            Panel auditPanel = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 280,
                BackColor = colorBlanco,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(15)
            };

            Label lblAuditTitle = new Label
            {
                Text = "Historial de Movimientos",
                Font = fontLabelInfo,
                AutoSize = true,
                ForeColor = colorPrimario,
                Dock = DockStyle.Top,
                Height = 30
            };

            tblMovimientos = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Margin = new Padding(0, 5, 0, 0)
            };

            colOperacion = new DataGridViewTextBoxColumn { HeaderText = "Op. #", Width = 80 };
            colFecha = new DataGridViewTextBoxColumn { HeaderText = "Fecha", Width = 150 };
            colTipo = new DataGridViewTextBoxColumn { HeaderText = "Tipo", Width = 100 };
            colMonto = new DataGridViewTextBoxColumn { HeaderText = "Monto", Width = 100 };
            colResponsable = new DataGridViewTextBoxColumn { HeaderText = "Responsable", Width = 150 };

            tblMovimientos.Columns.AddRange(colOperacion, colFecha, colTipo, colMonto, colResponsable);

            auditPanel.Controls.Add(tblMovimientos);
            auditPanel.Controls.Add(lblAuditTitle);

            // Ensamblar todo
            mainPanel.Controls.Add(auditPanel);
            mainPanel.Controls.Add(centerPanel);
            mainPanel.Controls.Add(headerPanel);

            Controls.Add(mainPanel);
        }

        private Panel CreateTransactionPanel(string labelText, string inputLabel, out TextBox textField, out Button button, Color colorPrimario, Font fontLabel)
        {
            Panel panel = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(15) };

            Label lbl = new Label { Text = inputLabel, AutoSize = true, Location = new Point(15, 20), Font = fontLabel };

            textField = new TextBox { Location = new Point(150, 17), Width = 250, Height = 28, Font = fontLabel };

            button = new Button
            {
                Text = labelText.Split(' ')[1],
                Location = new Point(15, 125),
                Width = 385,
                Height = 40,
                BackColor = colorPrimario,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };
            button.FlatAppearance.BorderSize = 0;

            panel.Controls.Add(lbl);
            panel.Controls.Add(textField);
            panel.Controls.Add(button);

            return panel;
        }

        private void ConfigureFrame()
        {
            Text = "Eureka Bank - Panel de Gestión del Cajero";
            Size = new Size(1200, 900);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor = Color.White;
            MinimumSize = new Size(900, 700);
        }

        // Getters y Setters de UI
        public string GetNumCuentaBusqueda() { return txtNumCuentaBusqueda.Text; }
        public void SetTitular(string val) { lblTitularVal.Text = val; }
        public void SetSaldo(string val) { lblSaldoVal.Text = val; }
        public void SetEstado(string val) { lblEstadoVal.Text = val; }

        public string GetMontoDeposito() { return txtMontoDeposito.Text; }
        public string GetMontoRetiro() { return txtMontoRetiro.Text; }
        public string GetMontoTransferencia() { return txtMontoTransferencia.Text; }
        public string GetCuentaDestino() { return txtCuentaDestino.Text; }

        public Button GetBtnBuscarCuenta() { return btnBuscarCuenta; }
        public Button GetBtnProcesarDeposito() { return btnProcesarDeposito; }
        public Button GetBtnProcesarRetiro() { return btnProcesarRetiro; }
        public Button GetBtnProcesarTransferencia() { return btnProcesarTransferencia; }

        public DataGridView GetTblMovimientos() { return tblMovimientos; }

        public void LimpiarCamposTransaccion()
        {
            txtMontoDeposito.Text = "";
            txtMontoRetiro.Text = "";
            txtMontoTransferencia.Text = "";
            txtCuentaDestino.Text = "";
        }

        public void MostrarMensaje(string mensaje, string titulo, MessageBoxIcon tipo)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, tipo);
        }
    }
}
