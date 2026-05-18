using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.vista;
using cli_EUREKABANK_esc_DotNet_Soap.monster.edu.ec.controlador;

namespace cli_EUREKABANK_esc_DotNet_Soap
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            LoginForm loginForm = new LoginForm();
            new LoginController(loginForm);
            Application.Run(loginForm);
        }
    }
}
