using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace monster.edu.ec.modelo;

[Table("movimiento")]
public class Movimiento
{
    [Key]
    [Column("id_movimiento")]
    public int IdMovimiento { get; set; }

    [Column("numero_operacion")]
    public string NumeroOperacion { get; set; } = string.Empty;

    [Column("numero_cuenta")]
    public string NumeroCuenta { get; set; } = string.Empty;

    [Column("id_empleado")]
    public int IdEmpleado { get; set; }

    [Column("tipo")]
    public string Tipo { get; set; } = string.Empty;  // 'Deposito' | 'Retiro'

    [Column("monto")]
    public decimal Monto { get; set; }

    [Column("fecha_hora")]
    public DateTime FechaHora { get; set; }

    [ForeignKey("IdEmpleado")]
    public virtual Empleado? Empleado { get; set; }

    [ForeignKey("NumeroCuenta")]
    public virtual Cuenta? Cuenta { get; set; }
}
