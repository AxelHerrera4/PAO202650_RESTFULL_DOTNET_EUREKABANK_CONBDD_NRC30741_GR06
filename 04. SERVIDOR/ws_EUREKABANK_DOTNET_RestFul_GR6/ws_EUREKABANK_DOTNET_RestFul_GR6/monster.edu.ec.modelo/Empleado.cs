using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace monster.edu.ec.modelo;

[Table("empleado")]
public class Empleado
{
    [Key]
    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [Column("usuario")]
    public string Usuario { get; set; } = string.Empty;

    [Column("password")]
    public string Password { get; set; } = string.Empty;

    [Column("nombre_completo")]
    public string NombreCompleto { get; set; } = string.Empty;

    [Column("rol")]
    public string Rol { get; set; } = string.Empty;
}
