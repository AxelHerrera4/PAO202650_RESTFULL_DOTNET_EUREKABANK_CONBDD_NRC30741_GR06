using System;
using System.Drawing;
using System.Windows.Forms;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.vista
{
    public partial class LoginForm : Form
    {
        private TextBox txtUsuario;
        private TextBox txtClave;
        private Button btnIngresar;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Panel leftPanel;
        private Panel rightPanel;

        public LoginForm()
        {
            InitializeComponents();
            ConfigureFrame();
        }

        private void InitializeComponents()
        {
            // Colores corporativos
            Color colorPrimario = Color.FromArgb(0, 102, 204);
            Color colorTexto = Color.FromArgb(51, 51, 51);
            Color colorBlanco = Color.White;
            Color colorFondo = Color.FromArgb(245, 247, 250);

            // Fuentes
            Font fontTitulo = new Font("Segoe UI", 32, FontStyle.Bold);
            Font fontSubtitulo = new Font("Segoe UI", 16, FontStyle.Regular);
            Font fontLabel = new Font("Segoe UI", 14, FontStyle.Bold);
            Font fontInput = new Font("Segoe UI", 15, FontStyle.Regular);
            Font fontBoton = new Font("Segoe UI", 16, FontStyle.Bold);

            // Panel Principal - Split Layout
            Panel mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = colorBlanco
            };

            // --- PANEL IZQUIERDO: FORMULARIO ---
            leftPanel = new Panel
            {
                Dock = DockStyle.Left,
                Width = 500,
                BackColor = colorBlanco
            };

            // Container del formulario
            Panel formContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = colorBlanco,
                AutoScroll = true
            };

            // Título
            lblTitulo = new Label
            {
                Text = "Eureka Bank",
                Font = fontTitulo,
                ForeColor = colorPrimario,
                AutoSize = true,
                Location = new Point(75, 50),
                MaximumSize = new Size(350, 0)
            };

            // Subtítulo
            lblSubtitulo = new Label
            {
                Text = "Acceso al Sistema de Cajeros",
                Font = fontSubtitulo,
                ForeColor = Color.FromArgb(102, 102, 102),
                AutoSize = true,
                Location = new Point(50, 100),
                MaximumSize = new Size(400, 0)
            };

            // Label Usuario
            Label lblUser = new Label
            {
                Text = "Usuario",
                Font = fontLabel,
                ForeColor = colorTexto,
                Location = new Point(75, 170),
                AutoSize = true
            };

            // TextBox Usuario
            txtUsuario = new TextBox
            {
                Font = fontInput,
                Size = new Size(320, 45),
                Location = new Point(75, 195),
                BackColor = Color.White,
                ForeColor = colorTexto,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(10)
            };
            txtUsuario.Text = "";

            // Label Contraseña
            Label lblPass = new Label
            {
                Text = "Contraseña",
                Font = fontLabel,
                ForeColor = colorTexto,
                Location = new Point(75, 265),
                AutoSize = true
            };

            // TextBox Contraseña
            txtClave = new TextBox
            {
                Font = fontInput,
                Size = new Size(320, 45),
                Location = new Point(75, 290),
                BackColor = Color.White,
                ForeColor = colorTexto,
                BorderStyle = BorderStyle.FixedSingle,
                PasswordChar = '•',
                Padding = new Padding(10)
            };

            // Botón
            btnIngresar = new Button
            {
                Text = "INICIAR SESIÓN",
                Font = fontBoton,
                BackColor = colorPrimario,
                ForeColor = colorBlanco,
                Size = new Size(320, 50),
                Location = new Point(75, 380),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnIngresar.FlatAppearance.BorderSize = 0;

            // Agregar componentes al panel izquierdo
            leftPanel.Controls.Add(lblTitulo);
            leftPanel.Controls.Add(lblSubtitulo);
            leftPanel.Controls.Add(lblUser);
            leftPanel.Controls.Add(txtUsuario);
            leftPanel.Controls.Add(lblPass);
            leftPanel.Controls.Add(txtClave);
            leftPanel.Controls.Add(btnIngresar);

            // --- PANEL DERECHO: IMAGEN ---
            rightPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = colorPrimario
            };

            try
            {
                string imagePath = System.IO.Path.Combine(
                    System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Resources", "images", "login_side.jpg");

                if (System.IO.File.Exists(imagePath))
                {
                    Image img = Image.FromFile(imagePath);
                    rightPanel.BackgroundImage = img;
                    rightPanel.BackgroundImageLayout = ImageLayout.Stretch;
                }
                else
                {
                    rightPanel.BackColor = colorPrimario;
                }
            }
            catch (Exception ex)
            {
                rightPanel.BackColor = colorPrimario;
                System.Diagnostics.Debug.WriteLine("Error cargando imagen: " + ex.Message);
            }

            // Agregar paneles al formulario
            mainPanel.Controls.Add(rightPanel);
            mainPanel.Controls.Add(leftPanel);

            Controls.Add(mainPanel);
        }

        private void ConfigureFrame()
        {
            Text = "Eureka Bank - Gestión de Cajeros";
            Size = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterScreen;
            MinimumSize = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.Sizable;
            BackColor = Color.White;
        }

        public string GetUsuario()
        {
            return txtUsuario.Text;
        }

        public string GetClave()
        {
            return txtClave.Text;
        }

        public Button GetBtnIngresar()
        {
            return btnIngresar;
        }

        public void MostrarMensaje(string mensaje, string titulo, MessageBoxIcon tipo)
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, tipo);
        }

        public void LimpiarCampos()
        {
            txtUsuario.Clear();
            txtClave.Clear();
        }
    }
}
