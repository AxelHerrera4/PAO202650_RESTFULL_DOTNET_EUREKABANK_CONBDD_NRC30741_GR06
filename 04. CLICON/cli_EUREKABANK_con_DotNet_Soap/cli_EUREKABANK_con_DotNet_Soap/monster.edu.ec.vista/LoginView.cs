using monster.edu.ec.modelo;

namespace monster.edu.ec.vista
{
    public class LoginView
    {
        public void MostrarMenuLogin()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║      EUREKA BANK - LOGIN           ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");

            Console.Write("Ingresa tu Usuario: ");
        }

        public void MostrarMensajeError(string mensaje)
        {
            Console.WriteLine($"\nError: {mensaje}");
            System.Threading.Thread.Sleep(2000);
        }

        public void MostrarMensajeExito(string mensaje)
        {
            Console.WriteLine($"\nExito: {mensaje}");
            System.Threading.Thread.Sleep(2000);
        }
    }
}
