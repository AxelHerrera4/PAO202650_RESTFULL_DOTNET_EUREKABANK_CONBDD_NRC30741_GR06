namespace monster.edu.ec.vista
{
    public class CuentaView
    {
        public void MostrarInfoCuenta()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║    INFORMACION DE CUENTA           ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");
        }

        public void MostrarExtracto()
        {
            Console.Clear();
            Console.WriteLine("╔════════════════════════════════════╗");
            Console.WriteLine("║     EXTRACTO DE MOVIMIENTOS        ║");
            Console.WriteLine("╚════════════════════════════════════╝\n");
        }

        public void MostrarOperacionesExitosas(string mensaje)
        {
            Console.WriteLine($"\nExito: {mensaje}");
        }

        public void MostrarOperacionesError(string mensaje)
        {
            Console.WriteLine($"\nError: {mensaje}");
        }

        public void PausarPantalla()
        {
            Console.WriteLine("\nPresiona cualquier tecla para continuar...");
            Console.ReadKey();
        }
    }
}
