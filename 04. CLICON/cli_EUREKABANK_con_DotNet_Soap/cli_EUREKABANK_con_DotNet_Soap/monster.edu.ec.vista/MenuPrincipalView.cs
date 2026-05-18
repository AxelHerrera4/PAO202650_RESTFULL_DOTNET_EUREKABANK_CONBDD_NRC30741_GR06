using monster.edu.ec.modelo;

namespace monster.edu.ec.vista
{
    public class MenuPrincipalView
    {
        public void MostrarMenuPrincipal(Usuario usuario)
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║  EUREKA BANK - PANEL SUPERADMIN    ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");
            Console.WriteLine("1. Monitorear Salud del Servicio");
            Console.WriteLine("2. Ver Conexiones Activas");
            Console.WriteLine("3. Ver Transacciones Recientes");
            Console.WriteLine("4. Ver Auditoria de Usuarios");
            Console.WriteLine("5. Estado de la Base de Datos");
            Console.WriteLine("6. Cerrar Sesion");
            Console.WriteLine("7. Salir");
            Console.Write("\nSelecciona una opcion: ");
        }
    }
}
