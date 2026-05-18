using Microsoft.EntityFrameworkCore;
using monster.edu.ec.modelo;

namespace monster.edu.ec.datos;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Cliente>    Clientes    { get; set; }
    public DbSet<Cuenta>     Cuentas     { get; set; }
    public DbSet<Empleado>   Empleados   { get; set; }
    public DbSet<Movimiento> Movimientos { get; set; }
    public DbSet<Sucursal>   Sucursales  { get; set; }
}
