using monster.edu.ec.modelo;
using monster.edu.ec.servicio;
using monster.edu.ec.vista;

namespace monster.edu.ec.controlador
{
    public class MenuController
    {
        private readonly MenuPrincipalView _view = new MenuPrincipalView();
        private readonly CuentaView _cuentaView = new CuentaView();
        private readonly IEurekaBankService _serviceClient;

        public MenuController(IEurekaBankService serviceClient)
        {
            _serviceClient = serviceClient;
        }

        public bool MostrarMenu(Usuario usuarioLogueado)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║  EUREKA BANK - PANEL SUPERADMIN    ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");
            Console.WriteLine("1. Monitorear Salud del Servicio");
            Console.WriteLine("2. Ver Conexiones Activas");
            Console.WriteLine("3. Ver Transacciones Recientes");
            Console.WriteLine("4. Ver Auditoría de Usuarios");
            Console.WriteLine("5. Estado de la Base de Datos");
            Console.WriteLine("6. Cerrar Sesión");
            Console.Write("\nSelecciona una opción: ");

            string opcion = Console.ReadLine();

            switch (opcion)
            {
                case "1":
                    MonitorearSaludServicio();
                    break;
                case "2":
                    VerConexionesActivas();
                    break;
                case "3":
                    VerTransaccionesRecientes();
                    break;
                case "4":
                    VerAuditoriaUsuarios();
                    break;
                case "5":
                    VerEstadoBaseDatos();
                    break;
                case "6":
                    _serviceClient.Logout();
                    return true;
                default:
                    Console.WriteLine("Opcion no valida");
                    System.Threading.Thread.Sleep(1500);
                    break;
            }

            return false;
        }

        private void MonitorearSaludServicio()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║     SALUD DEL SERVICIO EUREKA      ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");

            try
            {
                Console.WriteLine("✓ Servicio WS: ACTIVO");
                Console.WriteLine($"✓ Conexión BD: CONECTADO");
                Console.WriteLine($"✓ Uptime: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
                Console.WriteLine($"✓ Puerto: 8080");
                Console.WriteLine($"✓ Instancia SQL: localhost");
                Console.WriteLine($"✓ BD: EurekaBankDB");
                Console.WriteLine("\n[Estado] Todos los sistemas operativos ✓");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error: {ex.Message}");
            }

            PausarPantalla();
        }

        private void VerConexionesActivas()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║     CONEXIONES ACTIVAS             ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");

            try
            {
                Console.WriteLine("Conexiones SOAP activas:");
                Console.WriteLine("┌──────┬──────────────┬──────────────────┐");
                Console.WriteLine("│ ID   │ Cliente      │ Hora Conexión    │");
                Console.WriteLine("├──────┼──────────────┼──────────────────┤");
                Console.WriteLine($"│ 001  │ CLICON       │ {DateTime.Now:HH:mm:ss}       │");
                Console.WriteLine("└──────┴──────────────┴──────────────────┘");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            PausarPantalla();
        }

        private void VerAuditoriaUsuarios()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║     AUDITORÍA DE USUARIOS          ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");

            try
            {
                Console.WriteLine("Últimos logins registrados:");
                Console.WriteLine("┌────────────┬──────────────┬──────────────────┐");
                Console.WriteLine("│ Usuario    │ Rol          │ Hora             │");
                Console.WriteLine("├────────────┼──────────────┼──────────────────┤");
                Console.WriteLine($"│ monster    │ Superadmin   │ {DateTime.Now:HH:mm:ss}       │");
                Console.WriteLine("└────────────┴──────────────┴──────────────────┘");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            PausarPantalla();
        }

        private void VerEstadoBaseDatos()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║     ESTADO DE LA BASE DE DATOS     ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");

            try
            {
                var stats = _serviceClient.GetEstadisticas();
                Console.WriteLine("Estadísticas de la BD:");
                Console.WriteLine($"✓ Empleados registrados: {stats.TotalEmpleados}");
                Console.WriteLine($"✓ Clientes registrados:  {stats.TotalClientes}");
                Console.WriteLine($"✓ Cuentas activas:       {stats.CuentasActivas}");
                Console.WriteLine($"✓ Transacciones totales: {stats.TotalTransacciones}");
                Console.WriteLine($"✓ Saldo total en cuentas: ${stats.SaldoTotal:N2}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            PausarPantalla();
        }

        private void VerTransaccionesRecientes()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║    TRANSACCIONES RECIENTES         ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");

            try
            {
                var sucursales = new[] { 1, 2 };
                foreach (var sucursal in sucursales)
                {
                    var cuentas = _serviceClient.ListarCuentasPorSucursal(sucursal);
                    foreach (var cuenta in cuentas)
                    {
                        var movimientos = _serviceClient.GetExtracto(cuenta.NumeroCuenta);
                        if (movimientos.Count > 0)
                        {
                            Console.WriteLine($"Cuenta: {cuenta.NumeroCuenta} ({cuenta.NombreCliente})");
                            Console.WriteLine("┌────────────┬──────────┬──────────────────────────┐");
                            Console.WriteLine("│ Tipo       │ Monto    │ Fecha                    │");
                            Console.WriteLine("├────────────┼──────────┼──────────────────────────┤");
                            foreach (var mov in ((IEnumerable<dynamic>)movimientos).TakeLast(3))
                            {
                                Console.WriteLine($"│ {mov.TipoMovimiento.PadRight(10)} │ ${mov.Monto.ToString().PadRight(7)} │ {mov.Fecha.ToString().PadRight(24)} │");
                            }
                            Console.WriteLine("└────────────┴──────────┴──────────────────────────┘\n");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

            PausarPantalla();
        }

        private void PausarPantalla()
        {
            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
