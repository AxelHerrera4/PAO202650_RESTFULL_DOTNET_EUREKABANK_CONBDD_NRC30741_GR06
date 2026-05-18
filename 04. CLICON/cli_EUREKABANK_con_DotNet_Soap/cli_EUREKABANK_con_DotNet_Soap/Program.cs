using monster.edu.ec.modelo;
using monster.edu.ec.servicio;
using monster.edu.ec.controlador;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    static async Task Main(string[] args)
    {
        // Configurar servicios con inyección de dependencias
        var services = new ServiceCollection();
        services.AddScoped<IEurekaBankService, EurekaBankServiceClient>();
        services.AddScoped<LoginController>();
        services.AddScoped<MenuController>();

        var serviceProvider = services.BuildServiceProvider();

        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Usuario? usuarioLogueado = null;
        bool salir = false;

        while (!salir)
        {
            if (usuarioLogueado == null)
            {
                var loginController = serviceProvider.GetRequiredService<LoginController>();
                usuarioLogueado = loginController.Autenticar();
            }
            else
            {
                var menuController = serviceProvider.GetRequiredService<MenuController>();
                salir = menuController.MostrarMenu(usuarioLogueado);
                if (salir)
                {
                    usuarioLogueado = null;
                }
            }
        }

        Console.WriteLine("\nHasta luego!");
    }
}
