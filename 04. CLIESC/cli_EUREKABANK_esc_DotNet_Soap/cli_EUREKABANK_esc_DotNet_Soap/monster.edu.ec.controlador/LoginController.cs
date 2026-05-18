using System;
using System.Windows.Forms;
using cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.vista;
using cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.servicio;

namespace cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.controlador
{
    public class LoginController
    {
        private LoginForm vista;
        private EurekaBankServiceClient port;

        public LoginController(LoginForm vista)
        {
            this.vista = vista;
            this.port = new EurekaBankServiceClient();
            vista.GetBtnIngresar().Click += EjecutarLogin;
        }

        private void EjecutarLogin(object sender, EventArgs e)
        {
            string usuario = vista.GetUsuario();
            string clave = vista.GetClave();

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
            {
                vista.MostrarMensaje("Por favor, complete todos los campos.", "Campos vacíos", MessageBoxIcon.Warning);
                return;
            }

            try
            {
                vista.GetBtnIngresar().Enabled = false;

                string respuesta = port.AutenticarUsuario(usuario, clave);

                if (respuesta != null && respuesta.Contains("SUCCESS"))
                {
                    int idEmp = 2;
                    string nomEmp = "Usuario Eureka";

                    try
                    {
                        string[] partes = respuesta.Split(':');

                        if (partes.Length > 1)
                        {
                            string posibleId = partes[1].Trim();
                            if (System.Text.RegularExpressions.Regex.IsMatch(posibleId, @"^\d+$"))
                            {
                                idEmp = int.Parse(posibleId);
                                if (partes.Length > 2)
                                {
                                    nomEmp = partes[2].Trim();
                                }
                            }
                            else
                            {
                                nomEmp = posibleId.Replace("Bienvenido", "").Replace("monster", "").Trim();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Aviso: Error al parsear respuesta, usando valores por defecto: " + ex.Message);
                    }

                    vista.MostrarMensaje("Acceso Concedido\n" + nomEmp, "Eureka Bank", MessageBoxIcon.Information);
                    vista.Hide();

                    CajeroDashboardForm dashView = new CajeroDashboardForm(nomEmp);
                    new CajeroDashboardController(dashView, idEmp, nomEmp);
                    dashView.ShowDialog();

                    vista.Close();
                }
                else
                {
                    string errorMsg = (respuesta != null && respuesta.Contains("ERROR"))
                        ? respuesta.Replace("ERROR:", "").Trim()
                        : "Credenciales inválidas o error de sistema.";
                    vista.MostrarMensaje(errorMsg, "Error de Autenticación", MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                vista.MostrarMensaje("Error de conexión: " + ex.Message, "Error de Red", MessageBoxIcon.Error);
            }
            finally
            {
                vista.GetBtnIngresar().Enabled = true;
            }
        }
    }
}
